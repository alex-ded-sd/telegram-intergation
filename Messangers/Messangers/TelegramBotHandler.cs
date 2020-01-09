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
			List<TelegramBot> telegramBots = _tlgBotStorage.GetBots();
			telegramBots.ForEach(bot => {
				ITelegramBotClient client = new TelegramBotClient(bot.BotToken);
				client.OnMessage += ClientOnMessage;
				_botInstances.Add(client);
				client.StartReceiving();
			});
			_initState = true;
		}

		private void ClientOnMessage(object sender, MessageEventArgs e)
		{
			ITelegramBotClient client = sender as ITelegramBotClient;
			long chatId = e.Message.Chat.Id;
			TelegramBotChats telegramBotChat = _tlgBotStorage.GetChat(chatId);
			if (telegramBotChat == null)
			{
				TelegramBot telegramBot = _tlgBotStorage.GetBot(client.BotId);
				telegramBotChat = new TelegramBotChats {
					ChatId = chatId,
					TelegramBot = telegramBot
				};
				_tlgBotStorage.StoreChat(telegramBotChat);
			}

		}


		public async Task<string> TellBotAsync(SayHello sayHello) {
			TelegramBot bot = _tlgBotStorage.GetBot(sayHello.ChatId);
			if (bot != null) {
				var telegramClient = _botInstances.FirstOrDefault(item => item.BotId == bot.BotId);
				Message result = await telegramClient.SendTextMessageAsync(sayHello.ChatId, sayHello.Message);
				return $"Message: \"{result.Text}\" was successefuly send to chat with ChatId: {result.Chat.Id}";
			}
			return "Error";
		}


		public void StoreBot(string botToken)
		{
			TelegramBot telegramBot = _tlgBotStorage.GetBot(botToken);
			if (telegramBot == null)
			{
				ITelegramBotClient client = new TelegramBotClient(botToken);
				client.OnMessage += ClientOnMessage;
				telegramBot = new TelegramBot { BotId = client.BotId, BotToken = botToken };
				_tlgBotStorage.StoreBot(telegramBot);
				_botInstances.Add(client);
				client.StartReceiving();
			}
		}

		public List<long> GetChats(int botId)
		{
			return _tlgBotStorage.GetChats(botId);
		}
	}
}