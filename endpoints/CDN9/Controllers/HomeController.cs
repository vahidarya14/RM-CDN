using CDN9.Core.Application;
using Microsoft.AspNetCore.Mvc;

namespace CDN9.Controllers;

public class HomeController(IWebHostEnvironment host, FileMgmt fileMgmt, ILogger<HomeController> logger) : Controller
{
    string rootPath => $"{host.WebRootPath}{tenantFolder}";
    public static string tenantFolder = "/tenant1";


    public async Task<IActionResult> IndexAsync()
    {
        return View(fileMgmt.SubDir("", tenantFolder));
    }

    public IActionResult SubDir(string d)
    {
        var dirsFiles = fileMgmt.SubDir(d, tenantFolder);

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
        return SubDir(d);
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


    [RequestSizeLimit(long.MaxValue)]
    public async Task<ActionResult<UploadRes>> UploadAsync([FromForm] string folder, CancellationToken ct)
        => Ok(await fileMgmt.UploadFilesAsync(Request.Form.Files, rootPath, folder, ct));


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
}
