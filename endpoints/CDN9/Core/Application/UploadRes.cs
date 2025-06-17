namespace CDN9.Core.Application;

public record UploadRes(bool IsSuccess, string Msg, string FileName, string PhisicalUploadedPath);