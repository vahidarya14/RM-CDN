using System.Text;
using System.Text.RegularExpressions;

namespace CDN9.Core.Application;

public record D
{
    public string Text { get; set; }
    public string Path { get; set; }

    public bool IsReadOnly { get; set; }
    public string Length { get; set; }
    public FileAttributes Attributes { get; set; }
    public string Extension { get; set; }
    public UnixFileMode UnixFileMode { get; set; }
    public string? LinkTarget { get; set; }
    public DateTime CreationTimeUtc { get; set; }
    public DateTime LastWriteTimeUtc { get; set; }
    public DateTime LastAccessTimeUtc { get; set; }
}