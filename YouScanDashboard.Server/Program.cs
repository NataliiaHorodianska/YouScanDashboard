using System.Text;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using YouScanDashboard.Server.Charts;
using YouScanDashboard.Server.Data;
using YouScanDashboard.Server.Importing;
using YouScanDashboard.Server.Widgets;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Dashboard")
    ?? throw new InvalidOperationException("Connection string 'Dashboard' is not configured.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention());


builder.Services.AddSingleton<CellFormatter>();
builder.Services.AddSingleton<ITableFileReader, ExcelTableFileReader>();
builder.Services.AddSingleton<ChartColumnResolver>();
builder.Services.AddSingleton<ChartTypeSelector>();
builder.Services.AddSingleton<ChartDataBuilder>();
builder.Services.AddSingleton<DatasetImportFactory>();
builder.Services.AddSingleton(Random.Shared);
builder.Services.AddSingleton<RandomDatasetGenerator>();
builder.Services.AddScoped<DatasetSeeder>();
builder.Services.AddScoped<DataUploadService>();
builder.Services.AddScoped<WidgetService>();

builder.Services
    .AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Every error response uses the RFC 7807 ProblemDetails format.
builder.Services.AddProblemDetails();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    await scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.MigrateAsync();
    await scope.ServiceProvider.GetRequiredService<DatasetSeeder>().SeedAsync();
}

app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseDefaultFiles();
app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

// Unknown API routes return 404 instead of falling through to the SPA's index.html.
app.MapFallback("/api/{**path}", () => Results.Problem(statusCode: StatusCodes.Status404NotFound));
app.MapFallbackToFile("/index.html");

await app.RunAsync();