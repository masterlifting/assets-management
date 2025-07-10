using AM.Portfolio.Infrastructure;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace AM.Portfolio.Api;

public static class Program
{
    public static void Main(string[] args) =>
        Host.CreateDefaultBuilder(args)
        .ConfigureLogging((hostContext, logging) => logging.AddSeq(hostContext.Configuration))
        .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>())
        .Build()
        .Run();
}
