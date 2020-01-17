namespace Messengers
{
	using System;
	using Microsoft.AspNetCore.Hosting;
	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.Extensions.Hosting;
	using Microsoft.Extensions.Logging;

	public class Program
	{
		public static void Main(string[] args) {
			var host = CreateHostBuilder(args).Build();

		using (var serviceScope = host.Services.CreateScope()) {
			var services = serviceScope.ServiceProvider;
			try {
				var telegramBotHandler = services.GetRequiredService<TelegramBotHandler>();
				telegramBotHandler.InitBots();
			} catch (Exception ex) {
				var logger = services.GetRequiredService<ILogger<Program>>();
				logger.LogError(ex, "An error occurred.");
			}
		}

			host.Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder => {
					webBuilder.UseStartup<Startup>();
				});
	}
}
