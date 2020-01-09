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

		public async Task<TelegramBot> GetBotAsync(long chatId) {
			var relationshipsCursor = await _botClientRelation.FindAsync(botClientRel => botClientRel.ChatId == chatId);
			TelegramBotChats record = await relationshipsCursor.FirstOrDefaultAsync();
			return record.TelegramBot;
		}

		public List<TelegramBot> getBots() {
			return _botStorage.Find<TelegramBot>(bot => true).ToList();
		}

		public async Task<TelegramBot> GetBotAsync(string botToken) {
			var filter = Builders<TelegramBot>.Filter.Eq("BotToken", botToken);
			var cursor = await _botStorage.FindAsync(filter);
			return await cursor.FirstOrDefaultAsync();
		}

		public async Task<List<long>> GetChatsAsync(int botId) {
			var chatFilter = Builders<TelegramBotChats>.Filter.Eq("ChatId", botId);
			var relationshipsCursor = await _botClientRelation.FindAsync(chatFilter);
			List<TelegramBotChats> relationships = await relationshipsCursor.ToListAsync();
			return relationships.Select(item => item.ChatId).ToList();
		}
	}
}