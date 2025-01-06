using System.IO.Abstractions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Shorthand.HttpClientHAR.Tests.Integration;

public class TestWebApplicationFactory : WebApplicationFactory<TestWebApplicationFactory.Startup> {
    protected override void ConfigureWebHost(IWebHostBuilder builder) {
        builder.ConfigureServices(services => {
            var assembly = typeof(TestWebApplicationFactory).Assembly;

            services
                .AddControllers()
                .AddApplicationPart(assembly);
        });
    }

    protected override IHost CreateHost(IHostBuilder builder) {
        var fileSystem = new FileSystem();
        builder.UseContentRoot(fileSystem.Directory.GetCurrentDirectory());
        return base.CreateHost(builder);
    }

    protected override IHostBuilder CreateHostBuilder() {
        return Host.CreateDefaultBuilder()
            .ConfigureLogging((_, logging) => logging.ClearProviders())
            .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
    }

    public class Startup {
        public void Configure(IApplicationBuilder app) {
            app.UseDeveloperExceptionPage();
            app.UseRouting();
            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });
        }
    }
}
