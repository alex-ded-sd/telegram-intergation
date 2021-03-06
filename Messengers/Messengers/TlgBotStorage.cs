﻿namespace Messangers
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
		private readonly IMongoCollection<TelegramBotChat> _chatStorage;

		private readonly IMongoCollection<TelegramBot> _botStorage;

		private readonly DbStoreSettings _dbSettings;

		public TlgBotStorage(IOptions<DbStoreSettings> dbSettings) {
			this._dbSettings = dbSettings.Value;
			MongoClient client = new MongoClient(_dbSettings.ConnectionString);
			IMongoDatabase database = client.GetDatabase(_dbSettings.DataBaseName);
			_botStorage = database.GetCollection<TelegramBot>(_dbSettings.TlgBotsCollectionName);
			_chatStorage = database.GetCollection<TelegramBotChat>(_dbSettings.TlgBotChats);
		}

		public void StoreBot(TelegramBot telegramBot) {
			_botStorage.InsertOne(telegramBot);
		}

		public TelegramBot GetBot(long chatId) {
			var filter = Builders<TelegramBotChat>.Filter.Eq("ChatId", chatId);
			TelegramBotChat chat = _chatStorage.Find(filter).FirstOrDefault();
			return GetBot(chat.BotId);
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
			var chatFilter = Builders<TelegramBotChat>.Filter.Eq("TelegramBot.ChatId", botId);
			List<TelegramBotChat> chats = _chatStorage.Find(item => item.BotId == botId).ToList();
			return chats.Select(item => item.ChatId).ToList();
		}

		public TelegramBotChat GetChat(long chatId) {
			return _chatStorage.Find(item => item.ChatId == chatId).FirstOrDefault();
		}

		public void StoreChat(TelegramBotChat botChat)
		{
			_chatStorage.InsertOne(botChat);
		}
	}
}