using CDN9.Core.Application;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Options;

namespace CDN9.Controllers;

public class HomeController(IWebHostEnvironment host, FileMgmt fileMgmt, ILogger<HomeController> logger) : Controller
{
    string rootPath => $"{host.WebRootPath}{tenantFolder}";
    public static string tenantFolder = "/tenant1";


    public async Task<IActionResult> IndexAsync()
    {
        return View(fileMgmt.SubDir("", tenantFolder, null));
    }

    public IActionResult SubDir(string d, string? search)
    {
        var dirsFiles = fileMgmt.SubDir(d, tenantFolder, search);

        return Json(new
        {
            dirsFiles.dirs,
            dirsFiles.files
        });
    }

    [HttpPost]
    public IActionResult CreateDir(string d, string name)
    {
        var dirs = Directory.CreateDirectory($"{rootPath}/{d}/{name}");
        return SubDir(d, null);
    }

    [HttpPost]
    public IActionResult RemoveDir(string d)
    {
        try
        {
            Directory.Delete($"{rootPath}/{d}");
            return Ok(new { d });
        }
        catch (Exception)
        {
            throw;
        }
    }

    [HttpPost]
    public IActionResult RemoveFile(string d)
    {
        try
        {
            System.IO.File.Delete($"{host.WebRootPath}/{d}");
            return Ok(new { d });
        }
        catch (Exception)
        {
            throw;
        }
    }

    [HttpPost]
    public IActionResult RenameFile(string d, string newName)
    {
        try
        {
            var subDir = d.Substring(0, d.LastIndexOf('/'));
            System.IO.File.Move($"{host.WebRootPath}/{d}", $"{host.WebRootPath}/{subDir}/{newName}");
            return Ok(new { d });
        }
        catch (Exception)
        {
            throw;
        }
    }

    [RequestSizeLimit(long.MaxValue)]
    public async Task<ActionResult<UploadRes>> UploadAsync([FromForm] string folder,
                                                           [FromForm] bool change_img_format,
                                                           [FromForm] bool add_watermask_to_img,
                                                           [FromForm] bool make_sm_img_too,
                                                           CancellationToken ct)
        => Ok(await fileMgmt.UploadFilesAsync(Request.Form.Files, rootPath, folder, change_img_format, add_watermask_to_img, make_sm_img_too, ct));


    [HttpGet("video")]
    public IActionResult StreamVideo(string f)
        => PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot", f), "video/webm", enableRangeProcessing: true);

    [HttpGet("audio")]
    public IActionResult StreamAudio(string f)
    {
        //if (f.StartsWith('/'))
        //    f = f.Substring(1);
        //var fi = PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot", f), "audio/mpeg", enableRangeProcessing: true);
        //return fi;

        var path = $"{host.WebRootPath}{f}"; // Store audio files separately
        var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
        return File(stream, "audio/mpeg", enableRangeProcessing: true);
    }


    [HttpGet("Infos")]
    public IActionResult Infos([FromServices] IOptionsSnapshot<List<string>> replicas)
    {
        return Ok(new { replicas, version = "1.0.4" });
    }

    [HttpPost("RemoveReplica")]
    public IActionResult RemoveReplica( [FromServices] IWebHostEnvironment env, string url)
    {
        AppSettingsListUpdater.RemoveFromList(env,"Mirrors", url);

        return Ok(new { IsSuccess = true });
    }
    [HttpPost("AddReplica")]
    public IActionResult AddReplica( [FromServices] IWebHostEnvironment env, string url)
    {
        AppSettingsListUpdater.AddToList(env, "Mirrors", url);

        return Ok(new { IsSuccess = true });
    }

    [HttpGet("TotalSize")]
    [OutputCache(PolicyName = "Expire30")]
    public IActionResult TotalSize([FromServices] IOptionsSnapshot<List<string>> replicas)
    {
        long totalSize = 0;
        List<string> sizes = [];
        var x = fileMgmt.TotalSize("", tenantFolder, ref totalSize, ref sizes);
        return Ok(new { totalSize , sizes });
    }
}