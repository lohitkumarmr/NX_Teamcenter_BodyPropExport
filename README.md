# NX_Teamcenter_BodyPropExport
Exports all Bodies Properties in an assembly to CSV


# NX_Teamcenter_BodyPropExport

A modular NX Open C# journal that works with Teamcenter or local assemblies, 
exports body volume/area/mass data to Excel, and can capture screenshots.

## Project files

- `Program.cs` – Main logic, orchestrates export workflow  
- `ComponentWalker.cs` – Recursive traversal of components  
- `BodyMeasurer.cs` – Measures body properties (volume/area/mass)  
- `ScreenshotHelper.cs` – Captures NX window screenshot  
- `ExcelExporter.cs` – Writes data to Excel  

## Usage

1. Open in Visual Studio with references to `NXOpen.dll` and `Microsoft.Office.Interop.Excel`.
2. Build as .NET Framework (e.g. 4.8).
3. Copy DLL into NX journaling folder or run via Tools → Journal → Play.

## License

MIT
