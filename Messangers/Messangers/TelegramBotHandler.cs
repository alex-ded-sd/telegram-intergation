namespace Messangers
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using Models;
	using Telegram.Bot;
	using Telegram.Bot.Args;
	using Telegram.Bot.Types;

	public class TelegramBotHandler
	{
		private readonly TlgBotStorage _tlgBotStorage;
		private static List<TelegramBot> _botInstances = new List<TelegramBot>();

		public TelegramBotHandler(TlgBotStorage tlgBotStorage) {
			_tlgBotStorage = tlgBotStorage;
		}

		private void ClientOnMessage(object sender, MessageEventArgs e) {
			ITelegramBotClient client = sender as ITelegramBotClient;
			long chatId = e.Message.Chat.Id;
			var chatFilter = Builders<(long chatId, ITelegramBotClient client)>.Filter.Eq("chatId", chatId);
			bool isNotExist = !_botClientRelation.Find(chatFilter).Any();
			if (isNotExist) {
				_botClientRelation.InsertOne((chatId, client));
			}
		}

		public async Task<string> TellBotAsync(SayHello sayHello) {
			ITelegramBotClient client = await _tlgBotStorage.GetBotAsync(sayHello.ChatId);
			if (client != null) {
				Message result = await client.SendTextMessageAsync(sayHello.ChatId, sayHello.Message);
				return $"Message: \"{result.Text}\" was successefuly send to chat with ChatId: {result.Chat.Id}";
			}
			return "Error";
		}

		public void initBots() {
			
		}

		public async Task StoreBotAsync(string botToken) {
			TelegramBot telegramBot = await _tlgBotStorage.GetBotAsync(botToken);
			if (telegramBot == null) {
				TelegramBotClient client = new TelegramBotClient(botToken);
				client.OnMessage += ClientOnMessage;
				telegramBot = new TelegramBot { BotId = client.BotId, BotToken = botToken };
				_tlgBotStorage.StoreBot(telegramBot);
			}
		}

		public Task<List<long>> GetChatsAsync(int botId) {
			return _tlgBotStorage.GetChatsAsync(botId);
		}
	}
}