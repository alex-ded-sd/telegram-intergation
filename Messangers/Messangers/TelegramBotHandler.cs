namespace Messangers
{
	using System;
	using System.Threading.Tasks;
	using Models;
	using Telegram.Bot;
	using Telegram.Bot.Types;

	public class TelegramBotHandler 
	{
		private readonly IBotStorage _tlgBotStorage;

		public TelegramBotHandler(IBotStorage tlgBotStorage) {
			_tlgBotStorage = tlgBotStorage;
		}
		public async Task<string> TellBotAsync(SayHello sayHello) {
			ITelegramBotClient client = _tlgBotStorage.GetBot(sayHello.ChatId);
			if (client != null) {
				Message result = await client.SendTextMessageAsync(sayHello.ChatId, sayHello.Message);
				return $"Message: \"{result.Text}\" was successefuly send to chat with ChatId: {result.Chat.Id}";
			}

			return "Error";
		}
	}
}