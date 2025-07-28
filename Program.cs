using NXOpen;
using NXOpen.Assemblies;
using System;
using System.Collections.Generic;

namespace NX_Teamcenter_Export
{
    public class Program
    {
        public static Session theSession = Session.GetSession();
        public static ListingWindow lw = theSession.ListingWindow;

        public static void Main(string[] args)
        {
            lw.Open();
            Part workPart = theSession.Parts.BaseWork;
            lw.WriteLine("Running on: " + (theSession.IsTeamcenterActive ? "Teamcenter" : "Local") + " assembly");

            List<Component> allComponents = new List<Component>();
            RootComponentExplorer.LoadAllComponents(workPart, ref allComponents);

            foreach (Component comp in allComponents)
            {
                MyMeasureBodies.MeasureBodiesInComponent(comp);
            }

            string excelPath = ExcelExporter.ExportResultsToExcel(MyMeasureBodies.MeasureResults);
            lw.WriteLine("Exported results to: " + excelPath);

            lw.WriteLine("Done.");
        }
    }
}