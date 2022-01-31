using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace NerdStore.WebApp.Tests.Config
{
    public class LojaAppFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // Irá utilizar a startup que for enviada no momento da criação
            builder.UseStartup<TStartup>();

            // Irá buscar o appsettings do testing
            builder.UseEnvironment("Testing");
        }
    }
}
