namespace Messangers
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using Messangers.DAL;
	using Messangers.Models;
	using Microsoft.Extensions.Options;
	using MongoDB.Bson;
	using MongoDB.Driver;
	using Telegram.Bot;
	using Telegram.Bot.Args;

	public class TlgBotStorage
	{
		private readonly IMongoCollection<TelegramBotChats> _botClientRelation;

		private readonly IMongoCollection<TelegramBot> _botStorage;

		private readonly DbStoreSettings _dbSettings;

		public TlgBotStorage(IOptions<DbStoreSettings> dbSettings) {
			this._dbSettings = dbSettings.Value;
			MongoClient client = new MongoClient(_dbSettings.ConnectionString);
			IMongoDatabase database = client.GetDatabase(_dbSettings.DataBaseName);
			_botStorage = database.GetCollection<TelegramBot>(_dbSettings.TlgBotsCollectionName);
			_botClientRelation = database.GetCollection<TelegramBotChats>(_dbSettings.TlgBotChats);
		}

		public void StoreBot(TelegramBot telegramBot) {
			_botStorage.InsertOne(telegramBot);
		}

		public TelegramBot GetBot(long chatId) {
			var filter = Builders<TelegramBotChats>.Filter.Eq("ChatId", chatId);
			TelegramBotChats chat = _botClientRelation.Find(filter).FirstOrDefault();
			return chat?.TelegramBot;
		}

		public List<TelegramBot> GetBots() {
			return _botStorage.Find<TelegramBot>(bot => true).ToList();
		}

		public TelegramBot GetBot(string botToken) {
			var filter = Builders<TelegramBot>.Filter.Eq("BotToken", botToken);
			return _botStorage.Find(filter).FirstOrDefault();
		}

		public TelegramBot GetBot(int botId) {
			var filter = Builders<TelegramBot>.Filter.Eq("BotId", botId);
			return _botStorage.Find(filter).FirstOrDefault();
		}

		public List<long> GetChats(int botId) {
			var chatFilter = Builders<TelegramBotChats>.Filter.Eq("TelegramBot.ChatId", botId);
			List<TelegramBotChats> chats = _botClientRelation.Find(item => item.TelegramBot.BotId == botId).ToList();
			return chats.Select(item => item.ChatId).ToList();
		}

		public TelegramBotChats GetChat(long chatId) {
			return _botClientRelation.Find(item => item.ChatId == chatId).FirstOrDefault();
		}

		public void StoreChat(TelegramBotChats botChat)
		{
			_botClientRelation.InsertOne(botChat);
		}
	}
}