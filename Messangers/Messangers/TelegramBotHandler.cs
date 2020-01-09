namespace Messangers
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using Models;
	using Telegram.Bot;
	using Telegram.Bot.Types;

	public class TelegramBotHandler
	{
		private readonly TlgBotStorage _tlgBotStorage;

		public TelegramBotHandler(TlgBotStorage tlgBotStorage) {
			_tlgBotStorage = tlgBotStorage;
		}
		public async Task<string> TellBotAsync(SayHello sayHello) {
			ITelegramBotClient client = await _tlgBotStorage.GetBotAsync(sayHello.ChatId);
			if (client != null) {
				Message result = await client.SendTextMessageAsync(sayHello.ChatId, sayHello.Message);
				return $"Message: \"{result.Text}\" was successefuly send to chat with ChatId: {result.Chat.Id}";
			}
			return "Error";
		}

		public int StoreBot(string botToken) {
			return _tlgBotStorage.StoreBot(botToken);
		}

		public Task<List<long>> GetChatsAsync(int botId) {
			return _tlgBotStorage.GetChatsAsync(botId);
		}
	}
}