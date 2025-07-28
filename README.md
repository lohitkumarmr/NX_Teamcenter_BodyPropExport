# NX_Teamcenter_BodyPropExport
Exports all Bodies Properties in an assembly to CSV


# NX_Teamcenter_BodyPropExport

A modular NX Open C# journal that works with Teamcenter or local assemblies, 
exports body volume/area/mass data to Excel, and can capture screenshots.

## Project files

- `ComponentTraverser.cs` – Recursively traverses all components (root & children).  
- `BodyPropertyExporter.cs` – Measures volume, area, and mass using MeasureManager.  
- `ScreenshotGenerator.cs` – Captures and crops the NX screenshot based on component geometry bounds.  
- `CsvExporter.cs` – Exports data (including image filename) to CSV.  
- `MainProgram.cs` – Coordinates all the steps: 1) Traverse all components. 2) Measure body properties. 3) Take cropped screenshots. 4) Export to CSV



## Usage

1. Open in Visual Studio with references to `NXOpen.dll` and `Microsoft.Office.Interop.Excel`.
2. Build as .NET Framework (e.g. 4.8).
3. Copy DLL into NX journaling folder or run via Tools → Journal → Play.

## License

MIT
