﻿namespace Messangers
{
	using System;
	using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
	using Models;
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

		private void ClientOnMessage(object sender, MessageEventArgs e) {
			var x = 10;
			//ITelegramBotClient client = sender as ITelegramBotClient;
			//long chatId = e.Message.Chat.Id;
			//var chatFilter = Builders<(long chatId, ITelegramBotClient client)>.Filter.Eq("chatId", chatId);
			//bool isNotExist = !_botClientRelation.Find(chatFilter).Any();
			//if (isNotExist) {
			//	_botClientRelation.InsertOne((chatId, client));
			//}
		}

		public async Task<string> TellBotAsync(SayHello sayHello) {
			ITelegramBotClient client = await _tlgBotStorage.GetBotAsync(sayHello.ChatId);
			if (client != null) {
				Message result = await client.SendTextMessageAsync(sayHello.ChatId, sayHello.Message);
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