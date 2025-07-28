using NXOpen;
using System;
using System.Collections.Generic;
using System.IO;

public class Program
{
    public static void Main(string[] args)
    {
        Session session = Session.GetSession();
        Part workPart = session.Parts.Work;
        string imageFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "NX_Screenshots");
        string csvPath = Path.Combine(imageFolder, "BodyData.csv");

        Directory.CreateDirectory(imageFolder);
        List<(string ComponentName, string BodyName, double Volume, double Area, double Mass, string ImagePath)> dataList = new();

        Component root = workPart.ComponentAssembly.RootComponent;
        List<Component> allComponents = ComponentWalker.GetAllComponents(root);

        foreach (Component comp in allComponents)
        {
            List<Body> bodies = BodyPropertyExtractor.GetBodies(comp);
            foreach (Body body in bodies)
            {
                double vol = body.GetVolume();
                double area = body.GetArea();
                double mass = body.GetMassProperties().Mass;
                string screenshotPath = ScreenshotCropper.CaptureAndCrop(session, comp, imageFolder);
                dataList.Add((comp.DisplayName, body.Name, vol, area, mass, screenshotPath));
            }
        }

        CsvExporter.Export(csvPath, dataList);
        session.ListingWindow.Open();
        session.ListingWindow.WriteLine("Export complete: " + csvPath);
    }
}