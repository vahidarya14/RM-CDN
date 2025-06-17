namespace CDN9.Core.Domain;

public class NewFileUploaded
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string ContentType { get; set; }
    public string Folder { get; set; }
    public string CndHost { get; set; }
    public int TryCount { get; set; }
    public string? Exception { get; set; }
}
