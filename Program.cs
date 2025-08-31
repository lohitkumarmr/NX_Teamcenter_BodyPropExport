using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpenUI;
using System.Windows.Forms;



namespace NXOpenTest
{
    public class NXOpenTest
    {
        public static void Main()
        {
            NXOpen.Session theSession = NXOpen.Session.GetSession();
            NXOpen.Part workPart = theSession.Parts.Work;
            NXOpen.UI theUI = NXOpen.UI.GetUI();
            NXOpen.ListingWindow lw = theSession.ListingWindow;
            lw.Open();
            
            String partName = workPart.Name;
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string outputFolder = $"{desktopPath}\\{partName}.csv";
            lw.WriteLine(outputFolder);
            string outputCsv = outputFolder;


            AssemblyHelper.ProcessAllDescendants(theSession, outputCsv);
        }
            

            

        public static int GetUnloadOption(string dummy) { return (int)NXOpen.Session.LibraryUnloadOption.Immediately; }
    }

}

    

