using Messangers.DAL;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using Telegram.Bot;

namespace Messangers
{
	public class Startup
	{
		private TelegramBotHandler InitilazeTelegramBotHandler() {
			var storage = Configuration.Get<TlgBotStorage>();
			TelegramBotHandler telegramHandler = new TelegramBotHandler(storage);
			telegramHandler.initBots();
			return telegramHandler;
		}

		public Startup(IConfiguration configuration) {
			Configuration = configuration;
		}

		public IConfiguration Configuration {
			get;
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services) {
			BsonClassMap.RegisterClassMap<TelegramBotClient>(cm => {
				cm.MapProperty(tlg => tlg.BotId);
			});
			services.Configure<DbStoreSettings>(Configuration.GetSection(nameof(DbStoreSettings)));
			services.AddSingleton<TlgBotStorage>();
			TelegramBotHandler telegramHandler = InitilazeTelegramBotHandler();
			services.AddSingleton<TelegramBotHandler>(telegramHandler);
			services.AddControllers();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
			if (env.IsDevelopment()) {
				app.UseDeveloperExceptionPage();
			}

			app.UseHttpsRedirection();

			app.UseRouting();

			app.UseAuthorization();

			app.UseEndpoints(endpoints => {
				endpoints.MapControllers();
			});
		}
	}
}
