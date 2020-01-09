namespace Messangers
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using Messangers.DAL;
	using Messangers.Models;
	using Microsoft.EntityFrameworkCore.Internal;
	using Microsoft.Extensions.Options;
	using MongoDB.Bson;
	using MongoDB.Driver;
	using Telegram.Bot;
	using Telegram.Bot.Args;

	public class TlgBotStorage
	{
		private readonly IMongoCollection<(long chatId, ITelegramBotClient client)> _botClientRelation;

		private readonly IMongoCollection<TelegramBot> _botStorage;

		private readonly DbStoreSettings _dbSettings;

		public TlgBotStorage(IOptions<DbStoreSettings> dbSettings) {
			this._dbSettings = dbSettings.Value;
			MongoClient client = new MongoClient(_dbSettings.ConnectionString);
			IMongoDatabase database = client.GetDatabase(_dbSettings.DataBaseName);
			_botStorage = database.GetCollection<TelegramBot>(_dbSettings.TlgBotsCollectionName);
			_botClientRelation = database.GetCollection<(long chatId, ITelegramBotClient client)>(_dbSettings.TlgBotClientRelationship);
		}

		public void StoreBot(TelegramBot telegramBot) {
			_botStorage.InsertOne(telegramBot);
		}

		public async Task<ITelegramBotClient> GetBotAsync(long chatId) {
			var relationshipsCursor = await _botClientRelation.FindAsync(botClientRel => botClientRel.chatId == chatId);
			(long chatId, ITelegramBotClient client) record = await relationshipsCursor.FirstOrDefaultAsync();
			return record.client;
		}

		public async Task<TelegramBot> GetBotAsync(string botToken) {
			var filter = Builders<TelegramBot>.Filter.Eq("BotToken", botToken);
			var cursor = await _botStorage.FindAsync(filter);
			return await cursor.FirstOrDefaultAsync();
		}

		public async Task<List<long>> GetChatsAsync(int botId) {
			var chatFilter = Builders<(long chatId, ITelegramBotClient client)>.Filter.Eq("client.chatId", botId);
			var relationshipsCursor = await _botClientRelation.FindAsync(chatFilter);
			List<(long chatId, ITelegramBotClient client)> relationships = await relationshipsCursor.ToListAsync();
			return relationships.Select(item => item.chatId).ToList();
		}
	}
}