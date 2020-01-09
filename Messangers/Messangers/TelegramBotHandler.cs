namespace Messangers
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using Models;
	using MongoDB.Driver;
	using Telegram.Bot;
	using Telegram.Bot.Args;
	using Telegram.Bot.Types;

	public class TelegramBotHandler
	{
		private readonly TlgBotStorage _tlgBotStorage;
		private static List<ITelegramBotClient> _botInstances = new List<ITelegramBotClient>();
		private static bool _initState;

		public TelegramBotHandler(TlgBotStorage tlgBotStorage) {
			_tlgBotStorage = tlgBotStorage;
			initBots();
		}

		private void initBots() {
			if (_initState) {
				return;
			}
			List<TelegramBot> telegramBots = _tlgBotStorage.getBots();
			telegramBots.ForEach(bot => {
				ITelegramBotClient client = new TelegramBotClient(bot.BotToken);
				client.OnMessage += ClientOnMessage;
				_botInstances.Add(client);
				client.StartReceiving();
			});
			_initState = true;
		}

		private async void ClientOnMessage(object sender, MessageEventArgs e) {
			long chatId = e.Message.Chat.Id;
			TelegramBot tlgBot = await _tlgBotStorage.GetBotAsync(chatId);
			if (tlgBot == null) {
				var telegramBotChat = new TelegramBotChats {
					ChatId = chatId,
					TelegramBot = tlgBot
				};
				_tlgBotStorage.StoreChat(telegramBotChat);
			}

		}


		public async Task<string> TellBotAsync(SayHello sayHello) {
			TelegramBot bot = await _tlgBotStorage.GetBotAsync(sayHello.ChatId);
			if (bot != null) {
				var telegramClient = _botInstances.FirstOrDefault(item => item.BotId == bot.BotId);

				Message result = await telegramClient.SendTextMessageAsync(sayHello.ChatId, sayHello.Message);
				return $"Message: \"{result.Text}\" was successefuly send to chat with ChatId: {result.Chat.Id}";
			}
			return "Error";
		}


		public async Task StoreBotAsync(string botToken) {
			TelegramBot telegramBot = await _tlgBotStorage.GetBotAsync(botToken);
			if (telegramBot == null) {
				ITelegramBotClient client = new TelegramBotClient(botToken);
				client.OnMessage += ClientOnMessage;
				telegramBot = new TelegramBot { BotId = client.BotId, BotToken = botToken };
				_tlgBotStorage.StoreBot(telegramBot);
				_botInstances.Add(client);
				client.StartReceiving();
			}
		}

		public Task<List<long>> GetChatsAsync(int botId) {
			return _tlgBotStorage.GetChatsAsync(botId);
		}
	}
}