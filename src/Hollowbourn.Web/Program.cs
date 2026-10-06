using AspNetStatic;

namespace Hollowbourn.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();

            // Configure AspNetStatic with the routes to generate as static files.
            builder.Services.AddSingleton<IStaticResourcesInfoProvider>(
                new StaticResourcesInfoProvider(
                    new ResourceInfoBase[]
                    {
                        new PageResource("/"),
                        new PageResource("/Pantheon"),
                        new PageResource("/Privacy"),
                        new CssResource("/css/site.css"),
                        new JsResource("/js/site.js"),
                        new BinResource("/favicon.ico"),
                    }));

            var app = builder.Build();

            // Only generate the static site when explicitly requested (e.g. "dotnet run -- ssg"),
            // so normal local development (F5 / dotnet run) just serves the Razor Pages app.
            var isSsgRun = args.HasSsgArg();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment() && !isSsgRun)
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // Skip HTTPS redirection during static generation: the generator talks to itself over
            // plain HTTP, and there's no HTTPS endpoint/dev cert available in CI. GitHub Pages
            // terminates TLS itself for the published static files.
            if (!isSsgRun)
            {
                app.UseHttpsRedirection();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapRazorPages()
               .WithStaticAssets();

            if (isSsgRun)
            {
                var destinationRoot = Path.Combine(builder.Environment.ContentRootPath, "..", "..", "docs");
                Directory.CreateDirectory(destinationRoot);

                app.GenerateStaticContent(
                    destinationRoot,
                    exitWhenDone: true,
                    alwaysDefaultFile: true,
                    dontUpdateLinks: true);
            }

            app.Run();
        }
    }
}
