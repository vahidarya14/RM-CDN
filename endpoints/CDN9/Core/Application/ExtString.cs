namespace CDN9.Core.Application;

public static class ExtString
{
    static readonly string[] sourceArray = [".PDF"];
    static readonly string[] sourceArray0 = [".DOCX", ".DOC"];
    static readonly string[] sourceArray1 = [".AVI", ".MP4", ".DIVX", ".WMV"];
    static readonly string[] sourceArray2 = [".XLSX", ".XLS"];
    static readonly string[] audioExts = [".WAV", ".MID", ".MIDI", ".WMA", ".MP3", ".OGG", ".RMA"];
    static readonly string[] imgExts = [".PNG", ".JPG", ".JPEG", ".BMP", ".GIF", ".WEBP"];
    public static FileType GetFileType(this string fileName)
    {
        string fn = fileName.ToUpperInvariant();
        if (sourceArray.Any(fn.EndsWith))
            return FileType.Pdf;

        if (sourceArray2.Any(fn.EndsWith))
            return FileType.Excel;

        if (sourceArray0.Any(fn.EndsWith))
            return FileType.Doc;

        if (imgExts.Any(fn.EndsWith))
            return FileType.Img;

        if (audioExts.Any(fn.EndsWith))
            return FileType.Audio;

        if (sourceArray1.Any(fn.EndsWith))
            return FileType.Video;

        return FileType.Unknown;
    }

    public static bool EndsWithOneOf(this string str, List<string> lst) => lst.Any(str.EndsWith);
}
public enum FileType
{
    Unknown,
    Video,
    Img,
    Audio,
    Pdf,
    Excel,
    Doc
}
