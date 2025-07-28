using System.Collections.Generic;
using System.IO;

public class CsvExporter
{
    public static void Export(string path, List<(string ComponentName, string BodyName, double Volume, double Area, double Mass, string ImagePath)> data)
    {
        using (StreamWriter writer = new StreamWriter(path))
        {
            writer.WriteLine("Component,Body,Volume,Area,Mass,ImagePath");
            foreach (var item in data)
            {
                writer.WriteLine($"{item.ComponentName},{item.BodyName},{item.Volume},{item.Area},{item.Mass},{item.ImagePath}");
            }
        }
    }
}