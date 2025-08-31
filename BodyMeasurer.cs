using System.Collections.Generic;
using System.Windows.Forms;
using NXOpen;
using static NXOpen.CAE.Post;
using NXOpen.UF;
using System;

public static class BodyMeasurer
{
    public static (string BodyName, double Volume, double Area, double Mass, NXOpen.Point3d Centroid) MeasureBody(NXOpen.Body body, NXOpen.Part part)
    {
        NXOpen.Session theSession = NXOpen.Session.GetSession();
        NXOpen.MeasureManager mm = part.MeasureManager;
        NXOpen.Unit area = part.UnitCollection.GetBase("Area");
        NXOpen.Unit volume = part.UnitCollection.GetBase("Volume");
        NXOpen.Unit mass = part.UnitCollection.GetBase("Mass");
        string journalIdentifier;
        string displayName;
        string featureName;

        NXOpen.ListingWindow lw=theSession.ListingWindow;
        lw.Open();

        displayName = string.Empty;
        NXOpen.Body[] bodies = new Body[] { body };
        NXOpen.MeasureBodies mb = mm.NewMassProperties(new Unit[] { area, volume, mass }, 0.99, bodies);
        NXOpen.Point3d centroid= mb.Centroid;

        NXOpen.Features.Feature[] owningFeature = body.GetFeatures();
        if (owningFeature[0] != null)
        {
            featureName = owningFeature[0].GetFeatureName();   // e.g. "EXTRUDE"
            displayName = owningFeature[0].FeatureType;   // e.g. "EXTRUDE(3)"
            journalIdentifier = owningFeature[0].JournalIdentifier; // stable identifier

            // Get feature number as in model tree
            ; // corresponds to "Extrude(3)" -> 3

            lw.WriteLine($"Body: {body.Name}, Feature: {featureName}, JournalID: {journalIdentifier}");

        }

        return (displayName, mb.Volume, mb.Area, mb.Mass, centroid);
    }

    public static List<(string BodyName, double Volume, double Area, double Mass, NXOpen.Point3d Centroid, double boundingBoxwidth, double boundingBoxdepth, double boundingBoxheight)> GetWorkPartBodiesMeasurements(NXOpen.Part workPart)
    {
        var results = new List<(string BodyName, double Volume, double Area, double Mass, NXOpen.Point3d Centroid, double boundingBoxwidth, double boundingBoxdepth, double boundingBoxheight)>();

        if (workPart == null || workPart.Bodies == null)
            return results;

        foreach (NXOpen.Body body in workPart.Bodies)
        {

            if (body.IsSolidBody == false) continue;
            if (body.IsOccurrence) continue;    // skip occurrence bodies
            if (body.IsBlanked) continue;         // skip hidden
            
            NXOpen.Features.Feature[] feat = body.GetFeatures();

            // Skip if body was created by Mirror, Pattern, Subtract etc.
            string featType = feat.GetType().Name;
            if (featType.Contains("Mirror") || featType.Contains("Pattern"))
                continue;

            string journalIdentifier = feat[0].JournalIdentifier; // stable identifier)
            if (journalIdentifier.Contains("SUBTRACT")) continue;

            var (bodyName, volume, area, mass, centroid) = BodyMeasurer.MeasureBody(body, workPart);
            var (boundingBoxwidth, boundingBoxdepth, boundingBoxheight) = HandleBoundingBox(workPart, body, centroid);
            //var (boundingBoxwidth, boundingBoxdepth, boundingBoxheight) = (0, 0, 0);
            // Optional: show popup
            System.Windows.Forms.MessageBox.Show($"Body: {bodyName}\nVolume: {volume}\nArea: {area}\nMass: {mass}");

            results.Add((bodyName, volume, area, mass, centroid, boundingBoxwidth, boundingBoxdepth, boundingBoxheight));
        }

        return results;
    }
	
	public static (double width, double depth, double height) HandleBoundingBox(NXOpen.Part part, Body body, Point3d center)
	{
		Session theSession = Session.GetSession();
		UFSession theUFSession = UFSession.GetUFSession();
		ListingWindow lw = theSession.ListingWindow;

		NXOpen.Part workPart = theSession.Parts.Work;


        NXOpen.Features.ToolingBoxBuilder toolingBoxBuilder = workPart.Features.ToolingFeatureCollection.CreateToolingBoxBuilder(null);
        
        toolingBoxBuilder.Type = NXOpen.Features.ToolingBoxBuilder.Types.BoundedBlock;
        toolingBoxBuilder.BoxPosition = center;
        toolingBoxBuilder.ReferenceCsysType = NXOpen.Features.ToolingBoxBuilder.RefCsysType.SelectedCsys;
        toolingBoxBuilder.NonAlignedMinimumBox = true;
        
        SelectionIntentRule rule = workPart.ScRuleFactory.CreateRuleBodyDumb(new Body[] { body }, true);
        
        toolingBoxBuilder.BoundedObject.ReplaceRules(new SelectionIntentRule[] { rule }, false);
        
        toolingBoxBuilder.SetSelectedOccurrences(new NXObject[] { body }, Array.Empty<NXObject>());
        
        toolingBoxBuilder.CalculateBoxSize();
        
        NXObject featureObj = toolingBoxBuilder.Commit();
        
        toolingBoxBuilder.Destroy();
        
        Body bboxBody = null;
        foreach (Body b in ((NXOpen.Features.Feature)featureObj).GetBodies())
        {
            bboxBody = b;
            
            break;
        }

        if (bboxBody == null)
        {
            lw.WriteLine("Bounding box body not created.");
            
            return (0, 0, 0);
        }
        
		// Get bounding box dimensions
		double[] minCorner = new double[3];
		double[,] directions = new double[3, 3];
		double[] distances = new double[3];
        
        theUFSession.Modl.AskBoundingBoxExact(bboxBody.Tag, Tag.Null, minCorner, directions, distances);

		double width = distances[0];
		double depth = distances[1];
		double height = distances[2];

		double[] dimensions = new double[] { width, depth, height };
		Array.Sort(dimensions);

		// Delete the bounding box feature
		UFSession.GetUFSession().Modl.DeleteFeature(new Tag[] { ((NXOpen.Features.Feature)featureObj).Tag });

		return (dimensions[0], dimensions[1], dimensions[2]);
	}


}
