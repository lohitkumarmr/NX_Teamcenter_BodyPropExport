using NXOpen;
using NXOpen.Assemblies;
using System;
using System.Collections.Generic;
using System.IO;

public class AssemblyHelper
{
    public static void ProcessAllDescendants(Session theSession, string csvPath)
    {
        Part displayPart = theSession.Parts.Display;
        if (displayPart == null || displayPart.ComponentAssembly == null)
        {
            theSession.ListingWindow.Open();
            theSession.ListingWindow.WriteLine("⚠️ No assembly open.");
            return;
        }

        Component root = displayPart.ComponentAssembly.RootComponent;
        var allResults = new List<ComponentMeasurement>();

        // Keep track of already processed pairs (Parent+Component at same level)
        var seen = new HashSet<string>();

        // Start recursive processing at level 0
        ProcessComponentRecursive(theSession, root, null, 0, allResults, seen);

        // Export results
        ExportToCsv(allResults, csvPath);

        theSession.ListingWindow.WriteLine("✅ Finished recursive processing of all descendants.");
    }

    private static void ProcessComponentRecursive(Session theSession, Component comp, Component parent, int level,
                                                  List<ComponentMeasurement> allResults, HashSet<string> seen)
    {
        try
        {
            string parentName = parent != null ? parent.DisplayName : "ROOT";
            string key = $"{level}|{parentName}|{comp.DisplayName}";

            // Skip if this component was already processed at this level under same parent
            if (seen.Contains(key))
                return;
            seen.Add(key);

            // Switch work part
            theSession.Parts.SetWorkComponent(comp, out PartLoadStatus loadStatus);
            theSession.Parts.Display.ModelingViews.WorkView.Fit();

            Part workPart = theSession.Parts.Work;
            if (workPart != null)
            {
                var bodyResults = BodyMeasurer.GetWorkPartBodiesMeasurements(workPart);


                foreach (var r in bodyResults)
                {
                    allResults.Add(new ComponentMeasurement
                    {
                        ParentName = parentName,
                        ComponentName = new string(' ', level * 2) + comp.DisplayName, // indent with spaces
                        Level = level,
                        BodyName = r.BodyName,
                        Volume = r.Volume,
                        Area = r.Area,
                        Mass = r.Mass,
                        Centroid = r.Centroid,
                        boundingBoxwidth = r.boundingBoxwidth,
                        boundingBoxdepth = r.boundingBoxdepth,
                        boundingBoxheight = r.boundingBoxheight
                    });
                }
            }

            // recurse into children with level+1
            foreach (Component child in comp.GetChildren())
            {
                ProcessComponentRecursive(theSession, child, comp, level + 1, allResults, seen);
            }
        }
        catch (Exception ex)
        {
            theSession.ListingWindow.Open();
            theSession.ListingWindow.WriteLine($"❌ Error with {comp.DisplayName}: {ex.Message}");
        }
    }

    private static void ExportToCsv(List<ComponentMeasurement> allResults, string csvPath)
    {
        using (var sw = new StreamWriter(csvPath))
        {
            sw.WriteLine("Level,Parent,Component,Body,Volume,Area,Mass,CentroidX,CentroidY,CentroidZ,boundingBoxwidth,boundingBoxdepth,boundingBoxheight");

            foreach (var r in allResults)
            {
                sw.WriteLine($"{r.Level},{r.ParentName},{r.ComponentName},{r.BodyName},{r.Volume},{r.Area},{r.Mass}," +
                             $"{r.Centroid.X},{r.Centroid.Y},{r.Centroid.Z}, {r.boundingBoxwidth},{r.boundingBoxdepth},{r.boundingBoxheight}");
            }
        }
    }
}

public class ComponentMeasurement
{
    public int Level { get; set; }
    public string ParentName { get; set; }
    public string ComponentName { get; set; }
    public string BodyName { get; set; }
    public double Volume { get; set; }
    public double Area { get; set; }
    public double Mass { get; set; }
    public Point3d Centroid { get; set; }

    public double boundingBoxwidth { get; set; }

    public double boundingBoxdepth { get; set; }
    public double boundingBoxheight { get; set; }
    
}
