using Lamar;
using Lamar.Microsoft.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using Yeti.Core.Config;
using Yeti.Core.Service;
using Yeti.Db;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseLamar();

builder.Services.AddRazorPages();

builder.Services.AddDbContextPool<WriterContext>(
    options => options.UseNpgsql(builder.Configuration.GetConnectionString("WriterContext")));

// Full-text search: the reader search page queries the Rust search-api via IndexClient.
builder.Services.AddConfigurable<IndexOptions>();
builder.Services.AddHttpClient<IndexClient>((serviceProvider, client) =>
{
    var url = serviceProvider.GetRequiredService<IOptions<IndexOptions>>().Value.Url;
    client.BaseAddress = new(url);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services
    .AddAuthentication("Cookie")
    .AddCookie("Cookie", options =>
    {
        options.LoginPath = "/login";
        options.AccessDeniedPath = "/login";
    });
builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

// The author SPA is mounted at /author and served from wwwroot/author
// (the production build of yeti-vue, configured with base '/author/').
app.MapFallbackToFile("/author/{*path}", "author/index.html");

app.Run();
