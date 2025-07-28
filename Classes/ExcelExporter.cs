using System;
using System.Collections.Generic;
using System.IO;
using Excel = Microsoft.Office.Interop.Excel;

namespace NX_Teamcenter_Export
{
    public static class ExcelExporter
    {
        public static string ExportResultsToExcel(List<string> results)
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "BodyExport.xlsx");

            var excelApp = new Excel.Application();
            excelApp.Visible = false;
            var workbook = excelApp.Workbooks.Add();
            Excel.Worksheet worksheet = workbook.Sheets[1];

            int row = 1;
            foreach (var line in results)
            {
                worksheet.Cells[row++, 1] = line;
            }

            workbook.SaveAs(path);
            workbook.Close();
            excelApp.Quit();
            return path;
        }
    }
}