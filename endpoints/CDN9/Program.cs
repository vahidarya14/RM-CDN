using CDN9.Core.Application;
using CDN9.Core.Domain;
using CDN9.Core.Infrestrucrure.Persistance;
using Serilog;
using System.Threading.Channels;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Error()
    .WriteTo.File("./_serilog/log.txt", rollingInterval:RollingInterval.Day)
    .CreateLogger();
builder.Services.AddSerilog();

builder.Services.AddGrpc();

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});


builder.Services.AddScoped<NewFileUploadedRepository>();
builder.Services.AddScoped<FileMgmt>();
builder.Services.Configure<List<string>>(builder.Configuration.GetSection("Mirrors"));
builder.Services.AddHostedService<UploadToOtherServersHostedService>();
builder.Services.AddSingleton(x =>
{
    var _myChannel = Channel.CreateBounded<NewFileUploaded>(new BoundedChannelOptions(capacity: 1000)
    {
        FullMode = BoundedChannelFullMode.Wait,
        SingleReader = false,
        SingleWriter = false
    });
    return _myChannel;
});

builder.Services.AddRateLimiter(x =>
{
    x.AddPolicy("policy1", ctx =>
    {
        var token = ctx.Request.Headers["tenant-key"];
        return RateLimitPartition.GetConcurrencyLimiter(partitionKey: token,
            factory: _ => new ConcurrencyLimiterOptions
            {
                PermitLimit=1,
                QueueProcessingOrder=QueueProcessingOrder.NewestFirst,
                //Window=TimeSpan.FromSeconds(1)
            });
    });
});

builder.Services.AddOutputCache(options =>
{
    options.AddBasePolicy(builder =>        builder.Expire(TimeSpan.FromSeconds(10)));
    options.AddPolicy("Expire20", builder =>        builder.Expire(TimeSpan.FromSeconds(20)));
    options.AddPolicy("Expire30", builder =>        builder.Expire(TimeSpan.FromSeconds(30)));
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseCors("AllowAll");

app.UseHttpsRedirection();
app.UseOutputCache();
app.UseRouting();

app.UseRateLimiter();

app.UseAuthorization();

//app.MapStaticAssets();
app.UseStaticFiles();

app.MapGrpcService<GrpcUploadFile>().RequireRateLimiting("policy1");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
