using CDN9.Core.Domain;
using CDN9.Core.Infrestrucrure.Persistance;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Threading.Channels;

namespace CDN9.Core.Application;

public class FileMgmt(IWebHostEnvironment host, IOptionsMonitor<List<string>> mirrors,
                      Channel<NewFileUploaded> _notificator,
                      NewFileUploadedRepository newFileUploadedRepository)
{
    public (List<D> dirs, List<D> files) SubDir(string d, string tenantFolder, string? search)
    {

        string rootPath = $"{host.WebRootPath}{tenantFolder}";

        if (!string.IsNullOrEmpty(search))
        {
            List<D> allFiles = [];

            var (allDirs2, allFiles2) = SubDir2($"", tenantFolder);
            allFiles.AddRange(allFiles2.Where(x => x.Path.Contains(search)));
            foreach (var dir in allDirs2)
            {
                var (allDirs3, allFiles3) = SubDir2($"{dir.Path}", tenantFolder);
                allFiles.AddRange(allFiles3.Where(x => x.Path.ToLower().Contains(search.ToLower())));


                foreach (var dir2 in allDirs3)
                {
                    var (allDirs4, allFiles4) = SubDir2($"{dir2.Path}", tenantFolder);
                    allFiles.AddRange(allFiles4.Where(x => x.Path.ToLower().Contains(search.ToLower())));
                }
            }

            return (dirs: allDirs2, files: allFiles);
        }


        return SubDir2(d, tenantFolder);
    }
    (List<D> dirs, List<D> files) SubDir2(string d, string tenantFolder)
    {

        string rootPath = $"{host.WebRootPath}{tenantFolder}";

        var dirs = Directory.GetDirectories($"{rootPath}/{d}").Select(x => x.Replace('\\', '/')).Select(x => new D()
        {
            Path = d + '/' + x.Substring(x.LastIndexOf('/') + 1),
            Text = x.Substring(x.LastIndexOf('/') + 1)
        }).ToList();
        var files = Directory.GetFiles($"{rootPath}/{d}").Select(x => x.Split($"wwwroot{tenantFolder}")[1].Replace('\\', '/')).Select(x =>
        {
            var Info = new FileInfo($"{rootPath}/{x}");
            //if (!Info.Exists) return null;
            return new D()
            {
                Path = $"{tenantFolder}{d}",
                Text = x.Substring(x.LastIndexOf('/') + 1),
                IsReadOnly = Info.IsReadOnly,
                Length = Info.Length < 1024 ? $"{Info.Length}B" : Info.Length < 1_000_000?  $"{Info.Length / 1024}Kb": $"{Info.Length / 1000000}Mb",
                Attributes = Info.Attributes,
                Extension = Info.Extension,
                UnixFileMode = Info.UnixFileMode,
                LinkTarget = Info.LinkTarget,
                CreationTimeUtc = Info.CreationTimeUtc,
                LastWriteTimeUtc = Info.LastWriteTimeUtc,
                LastAccessTimeUtc = Info.LastAccessTimeUtc,

            };
        }).Where(x => x is not null).ToList();

        return (dirs, files);
    }


    public async Task<List<UploadRes>> UploadFilesAsync(IFormFileCollection files, string rootPath, string folder, bool change_img_format, bool add_watermark, CancellationToken ct)
    {

        var path = $"{rootPath}/{folder}";
        var res = new List<UploadRes>();
        foreach (var formFile in files)
        {
            var fileName = formFile.FileName;

            if (formFile == null || formFile.Length <= 0)
            {
                res.Add(new UploadRes(false, "length is 0", fileName, ""));
                continue;// BadRequest("length is 0");
            }

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var span = formFile.FileName.AsSpan();
            var fileN = span.Slice(0, fileName.LastIndexOf('.'));
            var fileExt = span.Slice(fileName.LastIndexOf('.'));

            var isImageButNotWebp = formFile.ContentType.ToLower().Contains("image/") &&
                                   !formFile.ContentType.ToLower().Contains("webp");

            var location = $"{path}/{fileName}";
            if (isImageButNotWebp && change_img_format)
            {
                await using MemoryStream ms = new();
                await formFile.CopyToAsync(ms);
                if (add_watermark)
                    fileName = ImageWatermark.AddWatermarkAndSaveAsWebp(ms, $"{host.WebRootPath}/watermask.png", path, fileName);
                else
                    fileName = ImageWatermark.SaveAsWebp(ms, path, fileName);
            }
            else
            {
                if (isImageButNotWebp && add_watermark)
                {
                    await using MemoryStream ms = new();
                    await formFile.CopyToAsync(ms);
                    fileName = ImageWatermark.AddWatermark(ms, $"{host.WebRootPath}/watermask.png", path, fileName);
                }
                else
                    fileName = await UploadAsOrginal(path, formFile, fileName, fileN.ToString(), fileExt.ToString(), ct);
            }


            mirrors.CurrentValue.ForEach(async x =>
            {
                await _notificator.Writer.WriteAsync(new NewFileUploaded
                {
                    CndHost = x,
                    Folder = folder,
                    Name = $"{fileName}",
                    ContentType = formFile.ContentType
                });
            });

            res.Add(new UploadRes(true, "", fileName, location));
        }

        return res;
    }

    private static async Task<string> UploadAsOrginal(string path, IFormFile formFile, string fileName, string fileN, string fileExt, CancellationToken ct)
    {
        var destinationFile = Path.Combine(path, fileName);
        var i = 1;
        while (File.Exists(destinationFile))
        {
            fileName = $"{fileN}{i++}{fileExt}";
            destinationFile = Path.Combine(path, fileName);
        }

        await using FileStream stream = new(destinationFile, FileMode.OpenOrCreate);
        await formFile.CopyToAsync(stream, ct);
        return fileName;
    }
}
