using NXOpen;
using NXOpen.Assemblies;
using NXOpen.UF;
using System;
using System.Collections.Generic;

namespace NX_Teamcenter_Export
{
    public static class MyMeasureBodies
    {
        public static List<string> MeasureResults = new List<string>();

        public static void MeasureBodiesInComponent(Component comp)
        {
            Part compPart = comp.Prototype as Part;
            if (compPart == null)
                return;

            foreach (Body body in compPart.Bodies)
            {
                double[] massProps;
                UFSession.GetUFSession().Modl.AskMassProps(body.Tag, out massProps);
                string result = $"Component: {comp.Name}, Body: {body.JournalIdentifier}, Volume: {massProps[0]:F4}";
                MeasureResults.Add(result);
            }
        }
    }
}