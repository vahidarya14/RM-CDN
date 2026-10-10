using SkiaSharp;

namespace CDN9.Core.Application;

public static class ImageExt
{
    public static async Task ResizeImage(this IFormFile file, string saveTo, int width, int height)
    {
        await using var stream = file.OpenReadStream();
        using var original = SKBitmap.Decode(stream);

        using var resized = original.Resize(new SKImageInfo(width, height), SKSamplingOptions.Default) ?? throw new InvalidOperationException("Image resizing failed.");

        using var image = SKImage.FromBitmap(resized);
        using var data = image.Encode(SKEncodedImageFormat.Jpeg, 90);

        await using var output = File.Create(saveTo);
        data.SaveTo(output);

        //using var original = await Image.LoadAsync(file.OpenReadStream());
        //original.Mutate(x => x.Resize(width, height));
        //await original.SaveAsync(saveTo);
    }

    public static async Task ResizeImage(this IFormFile file, string saveTo, int width)
    {
        await using var stream = file.OpenReadStream();
        using var original = SKBitmap.Decode(stream);

        // Preserve aspect ratio and don't upscale.
        float scale = Math.Min(1f, (float)width / original.Width);

        int newWidth = (int)Math.Round(original.Width * scale);
        int newHeight = (int)Math.Round(original.Height * scale);

        using var resized = original.Resize(new SKImageInfo(newWidth, newHeight), SKSamplingOptions.Default) ?? throw new InvalidOperationException("Image resizing failed.");

        using var image = SKImage.FromBitmap(resized);
        using var data = image.Encode(SKEncodedImageFormat.Jpeg, 90);

        await using var output = File.Create(saveTo);
        data.SaveTo(output);

        //using var original = await Image.LoadAsync(file.OpenReadStream());
        //original.Mutate(x =>
        //    x.Resize(new ResizeOptions
        //    {
        //        Mode = ResizeMode.Max,
        //        Size = new Size(width, int.MaxValue)
        //    })
        //);
        //await original.SaveAsync(saveTo);
    }

    //public static Bitmap ResizeImage(Image imgToResize, int newHeight)
    //{
    //    int sourceWidth = imgToResize.Width;
    //    int sourceHeight = imgToResize.Height;

    //    float nPercentH = ((float)newHeight / (float)sourceHeight);

    //    int destWidth = Math.Max((int)Math.Round(sourceWidth * nPercentH), 1); // Just in case;
    //    int destHeight = newHeight;

    //    Bitmap b = new Bitmap(destWidth, destHeight);
    //    using (Graphics g = Graphics.FromImage((Image)b))
    //    {
    //        g.SmoothingMode = SmoothingMode.HighQuality;
    //        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
    //        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
    //        g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
    //    }

    //    return b;
    //}

    //public async static Task<Image> ResizeImage(this HttpPostedFileBase file, string fullPath, int height)
    //{
    //    using (var img = Image.FromStream(file.InputStream))
    //    {
    //        int width = img.Width * height / img.Height;
    //        var resImg = img.Resize(width, height);
    //        resImg.Save(fullPath, ImageFormat.Jpeg);
    //        return resImg;
    //    }
    //}

    //public async static Task<WebImage> ResizeImage2(this HttpPostedFileBase file, string fullPath, int height)
    //{
    //    WebImage img = new WebImage(file.InputStream);
    //    int width = img.Width * height / img.Height;
    //    img.Resize(width, height);
    //    img.Save(fullPath);
    //    return img;
    //}

    //public async static Task<WebImage> ResizeImage(this HttpPostedFileBase file, string fullPath, int width, int height)
    //{
    //    WebImage img = new WebImage(file.InputStream);
    //    if (img.Width > width)
    //        img.Resize(width, height);
    //    img.Save(fullPath);
    //    return img;
    //}

    //public static Image Resize(this Image image, int width, int height)
    //{
    //    var res = new Bitmap(width, height);
    //    using (var graphic = Graphics.FromImage(res))
    //    {
    //        graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
    //        graphic.SmoothingMode = SmoothingMode.HighQuality;
    //        graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
    //        graphic.CompositingQuality = CompositingQuality.HighQuality;
    //        graphic.DrawImage(image, 0, 0, width, height);
    //    }
    //    return res;
    //}
}