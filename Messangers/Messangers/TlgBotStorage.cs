namespace Messangers
{
	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Linq;
	using Controllers;
	using Microsoft.EntityFrameworkCore.Internal;
	using Telegram.Bot;
	using Telegram.Bot.Args;

	public class TlgBotStorage: IBotStorage
	{
		private static ConcurrentDictionary<long, ITelegramBotClient> _botClientRelation = new ConcurrentDictionary<long, ITelegramBotClient>();
		private static List<ITelegramBotClient> _botStorage = new List<ITelegramBotClient>();
		private static object _lockObject = new object();

		private void StoreBot(ITelegramBotClient client) {
			lock (_lockObject) {
				if (!_botStorage.Any(bot => client.BotId == bot.BotId)) {
					_botStorage.Add(client);
					client.OnMessage += ClientOnOnMessage;
					client.StartReceiving();
				}
			}
		}
		

		private void ClientOnOnMessage(object sender, MessageEventArgs e) {
			ITelegramBotClient client = sender as ITelegramBotClient;
			long chatId = e.Message.Chat.Id;
			bool isExists = _botClientRelation.Any(item => item.Key == chatId && item.Value.BotId == client.BotId);
			if (!isExists) {
				_botClientRelation.TryAdd(chatId, client);
			}
		}

		public int StoreBot(string botToken) {
			TelegramBotClient client = new TelegramBotClient(botToken);
			StoreBot(client);
			return client.BotId;
		}

		public ITelegramBotClient GetBot(long chatId) {
			if (_botClientRelation.TryGetValue(chatId, out ITelegramBotClient botClient)) {
				return botClient;
			}
			return null;
		}

		public List<long> GetChats(int botId) {
			return _botClientRelation.Where(item => item.Value.BotId == botId).Select(item => item.Key).ToList();
		}
	}
}