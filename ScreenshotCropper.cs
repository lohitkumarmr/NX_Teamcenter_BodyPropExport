using NXOpen;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

public class ScreenshotCropper
{
    public static string CaptureAndCrop(Session session, Component component, string outputFolder)
    {
        View currentView = session.Parts.Display.ModelingViews.FindObject("Isometric");
        session.Parts.Display.ModelingViews.WorkView = currentView;
        currentView.Fit();

        string compName = component.DisplayName.Replace(" ", "_");
        string rawPath = Path.Combine(outputFolder, $"{compName}_raw.bmp");
        string croppedPath = Path.Combine(outputFolder, $"{compName}_cropped.png");

        session.Parts.Display.PartsDisplayManager.CaptureSceneToFile(rawPath, NXOpen.Display.ImageFormat.Bitmap);

        using (Bitmap bmp = new Bitmap(rawPath))
        {
            Rectangle bounds = GetBoundingBox(bmp);
            using (Bitmap cropped = bmp.Clone(bounds, bmp.PixelFormat))
            {
                cropped.Save(croppedPath, ImageFormat.Png);
            }
        }

        File.Delete(rawPath);
        return croppedPath;
    }

    private static Rectangle GetBoundingBox(Bitmap bmp)
    {
        int minX = bmp.Width, minY = bmp.Height, maxX = 0, maxY = 0;
        Color bg = bmp.GetPixel(0, 0);

        for (int y = 0; y < bmp.Height; y++)
        {
            for (int x = 0; x < bmp.Width; x++)
            {
                if (bmp.GetPixel(x, y) != bg)
                {
                    if (x < minX) minX = x;
                    if (x > maxX) maxX = x;
                    if (y < minY) minY = y;
                    if (y > maxY) maxY = y;
                }
            }
        }

        return Rectangle.FromLTRB(minX, minY, maxX, maxY);
    }
}