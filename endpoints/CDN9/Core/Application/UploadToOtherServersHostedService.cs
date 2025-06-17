using CDN9.Controllers;
using CDN9.Core.Domain;
using CDN9.Core.Infrestrucrure.Persistance;
using CDN9.Protos;
using Google.Protobuf;
using Grpc.Net.Client;
using System.Threading.Channels;

namespace CDN9.Core.Application;

public class UploadToOtherServersHostedService(IServiceProvider services, ILogger<UploadToOtherServersHostedService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<NewFileUploadedRepository>();
        var host = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
        var channel = scope.ServiceProvider.GetRequiredService<Channel<NewFileUploaded>>();
        await foreach (var newFileUploded in channel.Reader.ReadAllAsync())
        {
            var dbfileUploded = repo.FindOne(x => x.Folder == newFileUploded.Folder && x.Name == newFileUploded.Name && x.TryCount < 5);

            try
            {
                using var grpcChannel = GrpcChannel.ForAddress(newFileUploded.CndHost);//newFileUploded.CndHost
                var client = new Uploader.UploaderClient(grpcChannel);
                var reply = await client.UploadAsync(new FileUploadRequest
                {
                    Name = newFileUploded.Name,
                    Folder = newFileUploded.Folder,
                    ContentType = newFileUploded.ContentType,
                    Bytes = await ByteString.FromStreamAsync(new MemoryStream(File.ReadAllBytes($"{host.WebRootPath}/{HomeController.tenantFolder}/{newFileUploded.Folder}/{newFileUploded.Name}")))
                });
                if (reply.IsSuccess)
                {
                    if (dbfileUploded is not null)
                        repo.Delete(dbfileUploded);
                    continue;
                }
                else
                {
                    ProblemCallBack(dbfileUploded, newFileUploded, 500.ToString(), repo);
                }
            }
            catch (Exception ex)
            {
                ProblemCallBack(dbfileUploded, newFileUploded, ex.Message, repo);
            }


            //try
            //{
            //    var httpClient = new HttpClient
            //    {
            //        BaseAddress = new Uri(newFileUploded.CndHost)
            //    };
            //    await using var fs = new MemoryStream(File.ReadAllBytes($"{host.WebRootPath}/{HomeController.tenantFolder}/{newFileUploded.Folder}/{newFileUploded.Name}"));
            //    var pdfContent = new ByteArrayContent(fs.ToArray());
            //    pdfContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/pdf");

            //    using var form = new MultipartFormDataContent
            //            {
            //                { new StringContent(newFileUploded.Folder), "folder" },
            //                { pdfContent, "formFile", newFileUploded.Name }
            //            };

            //    var response = await httpClient.PostAsync("home/Upload", form);

            //    if (response.IsSuccessStatusCode)
            //    {
            //        var aa = await response.Content.ReadAsStringAsync();
            //        //var res = JsonConvert.DeserializeObject<UploadRes>(aa);

            //        if (dbfileUploded is not null)
            //            repo.Delete(dbfileUploded);
            //    }
            //    else
            //    {
            //        if (response.StatusCode != System.Net.HttpStatusCode.OK)
            //            ProblemCallBack(dbfileUploded, newFileUploded, response.StatusCode.ToString(), repo);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    ProblemCallBack(dbfileUploded, newFileUploded, ex.Message, repo);
            //}
        }
    }

    void ProblemCallBack(NewFileUploaded? dbfileUploded, NewFileUploaded? newFileUploded, string ex, NewFileUploadedRepository repo)
    {
        if (dbfileUploded != null)
        {
            dbfileUploded.Exception = ex;
            dbfileUploded.TryCount++;
            repo.Update(dbfileUploded);
        }
        else
        {
            repo.Insert(newFileUploded);
        }
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        await base.StopAsync(stoppingToken);
    }
}