﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using System;
using System.Threading;
using System.Threading.Tasks;
using IM.Service.Company.Analyzer.Services.CalculatorServices;

namespace IM.Service.Company.Analyzer.Services.BackgroundServices
{
    public class CalculatorBackgroundService : BackgroundService
    {
        private readonly IServiceProvider services;
        private bool start = true;
        public CalculatorBackgroundService(IServiceProvider services) => this.services = services;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var delay = new TimeSpan(0, 1, 0);

            while (start)
            {
                start = false;
                await Task.Delay(delay, stoppingToken);

                var scope = services.CreateScope();
                var priceCalculator = scope.ServiceProvider.GetRequiredService<PriceCalculator>();
                var reportCalculator = scope.ServiceProvider.GetRequiredService<ReportCalculator>();
                var ratingCalculator = scope.ServiceProvider.GetRequiredService<RatingCalculator>();

                try
                {
                    if (await priceCalculator.CalculateAsync())
                        if (await reportCalculator.CalculateAsync())
                            await ratingCalculator.CalculateAsync();
                        else
                            await ratingCalculator.CalculateAsync();
                    else if (await reportCalculator.CalculateAsync())
                        await ratingCalculator.CalculateAsync();
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Analyzer exception: {ex.InnerException?.Message ?? ex.Message}");
                    Console.ForegroundColor = ConsoleColor.Gray;
                }

                scope.Dispose();
                start = true;
            }
            // ReSharper disable once FunctionNeverReturns
        }
        public override Task StopAsync(CancellationToken stoppingToken)
        {
            base.StopAsync(stoppingToken);
            return Task.CompletedTask;
        }
    }
}
