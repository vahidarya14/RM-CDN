using CDN9.Protos;
using Grpc.Core;

namespace CDN9.Core.Application;

public class GrpcUploadFile(IWebHostEnvironment host) : Uploader.UploaderBase
{
    string rootPath => $"{host.WebRootPath}{tenantFolder}";
    public static string tenantFolder = "/tenant1";

    public override async Task<UploaderRepsponse> Upload(FileUploadRequest request, ServerCallContext context)
    {
        var bytes = request.Bytes.ToByteArray();
        var path = $"{rootPath}/{request.Folder}";

        var fileName = request.Name;

        if (bytes == null || bytes.Length <= 0)
        {
            return new UploaderRepsponse { IsSuccess = false,Msg= "length is 0",FileName= fileName,PhisicalUploadedPath="" };
        }

        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        var span = request.Name.AsSpan();
        var fileN = span.Slice(0, span.LastIndexOf('.'));
        var fileExt = span.Slice(span.LastIndexOf('.'));


        var location = $"{path}/{fileName}";
        if (request.ContentType.ToLower().Contains("image/") && !request.ContentType.ToLower().Contains("webp"))
        {
            await using MemoryStream ms = new(bytes);
            fileName = ImageWatermark.AddWatermarkAndSaveAsWebp(ms, $"{host.WebRootPath}/watermask.png", path, fileName);
        }
        else
        {
            var destinationFile = Path.Combine(path, fileName);
            var i = 1;
            while (File.Exists(destinationFile))
            {
                fileName = $"{fileN}{i++}{fileExt}";
                destinationFile = Path.Combine(path, fileName);
            }

            File.WriteAllBytes(destinationFile, bytes);
        }

        return new UploaderRepsponse { IsSuccess = true, FileName = fileName,  };

    }
}
