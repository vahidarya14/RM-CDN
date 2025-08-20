using System.Drawing;
using System.Drawing.Imaging;

namespace CDN9.Core.Application;

public static class ImageWatermark
{
    public static void AddWatermark(string sourceImageFilePath, string watermarkImageFilePath, string outputImageFilePath)
    {
        using Image sourceImage = Image.FromFile(sourceImageFilePath);
        using Image watermarkImage = Image.FromFile(watermarkImageFilePath);
        using Bitmap watermarkedImage = new Bitmap(sourceImage.Width, sourceImage.Height);
        using Graphics g = Graphics.FromImage(watermarkedImage);
        // Clear the bitmap
        g.Clear(Color.White);

        // Draw the source image
        g.DrawImage(sourceImage, 0, 0);

        // Calculate watermark position (center)
        int x = (watermarkedImage.Width - watermarkImage.Width) / 2;
        int y = (watermarkedImage.Height - watermarkImage.Height) / 2;

        // Draw the watermark image
        g.DrawImage(watermarkImage, x, y);

        // Save the watermarked image
        watermarkedImage.Save(outputImageFilePath, ImageFormat.Webp); // or any desired format
    }

    public static string AddWatermarkAndSaveAsWebp(Stream sourceImageFilePath, string watermarkImageFilePath, string outputImageFilePath,string outputImageFileName)
    {
        using Image sourceImage = Image.FromStream(sourceImageFilePath);
        using Image watermarkImage = Image.FromFile(watermarkImageFilePath);
        using Bitmap watermarkedImage = new Bitmap(sourceImage.Width, sourceImage.Height);
        using Graphics g = Graphics.FromImage(watermarkedImage);
        // Clear the bitmap
        g.Clear(Color.White);

        // Draw the source image
        g.DrawImage(sourceImage, 0, 0,sourceImage.Width, sourceImage.Height);

        //// Calculate watermark position (center)
        //int x = (watermarkedImage.Width - watermarkImage.Width) /20;
        //int y = (watermarkedImage.Height - watermarkImage.Height) /20;

        ////Calculate watermark position (bottom right)
        int x = watermarkedImage.Width - watermarkImage.Width - (watermarkImage.Width/2);
        int y = watermarkedImage.Height - watermarkImage.Height - (watermarkImage.Height/2);

        // Draw the watermark image
        g.DrawImage(watermarkImage, x, y);

        var newExt = outputImageFileName.ToLowerInvariant().EndsWith(".webp") ? "" : ".webp";
        var destinationFile = $"{outputImageFilePath}/{outputImageFileName}{newExt}";
        var span = destinationFile.AsSpan();
        var filePath = span.Slice(0, span.LastIndexOf('.'));
        var fileExt = span.Slice(span.LastIndexOf('.'));
        var i = 1;
        while (File.Exists($"{outputImageFilePath}/{outputImageFileName}{newExt}"))
        {
            outputImageFileName = $"{i++}{fileExt}";
            //destinationFile = $"{filePath}{outputImageFileName}";
        }

        // Save the watermarked image
        watermarkedImage.Save($"{outputImageFilePath}/{outputImageFileName}{newExt}", ImageFormat.Webp); // or any desired format

        return $"{outputImageFileName}{newExt}";
    }
}