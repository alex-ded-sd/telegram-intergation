namespace Messangers
{
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

		private readonly IMongoCollection<TelegramBotClient> _botStorage;

		private readonly DbStoreSettings _dbSettings;

		public TlgBotStorage(IOptions<DbStoreSettings> dbSettings)
		{
			this._dbSettings = dbSettings.Value;
			MongoClient client = new MongoClient(_dbSettings.ConnectionString);
			IMongoDatabase database = client.GetDatabase(_dbSettings.DataBaseName);
			_botStorage = database.GetCollection<TelegramBotClient>(_dbSettings.TlgBotStorageCollectionName);
			_botClientRelation = database.GetCollection<(long chatId, ITelegramBotClient client)>(_dbSettings.TlgBotClientRelationship);
		}

		private void StoreBot(TelegramBotClient client) {
			var filter = Builders<TelegramBotClient>.Filter.Eq("BotId", client.BotId);
			bool isNotExist = !_botStorage.Find(filter).Any();
			if (isNotExist) {
				_botStorage.InsertOne(client);
				client.OnMessage += ClientOnOnMessage;
				client.StartReceiving();
			}
		}
		

		private void ClientOnOnMessage(object sender, MessageEventArgs e) {
			ITelegramBotClient client = sender as ITelegramBotClient;
			long chatId = e.Message.Chat.Id;
			var chatFilter = Builders<(long chatId, ITelegramBotClient client)>.Filter.Eq("chatId", chatId);
			bool isNotExist = !_botClientRelation.Find(chatFilter).Any();
			if (isNotExist) {
				_botClientRelation.InsertOne((chatId, client));
			}
		}

		public int StoreBot(string botToken) {
			TelegramBotClient client = new TelegramBotClient(botToken);
			StoreBot(client);
			return client.BotId;
		}

		public async Task<ITelegramBotClient> GetBotAsync(long chatId) {
			IAsyncCursor<(long chatId, ITelegramBotClient client)> relationshipsCursor = await _botClientRelation
				.FindAsync(botClientRel => botClientRel.chatId == chatId);
			(long chatId, ITelegramBotClient client) record = await relationshipsCursor.FirstOrDefaultAsync();
			return record.client;
		}

		public async Task<List<long>> GetChatsAsync(int botId) {
			var chatFilter = Builders<(long chatId, ITelegramBotClient client)>.Filter.Eq("client.chatId", botId);
			var relationshipsCursor = await _botClientRelation.FindAsync(chatFilter);
			List<(long chatId, ITelegramBotClient client)> relationships = await relationshipsCursor.ToListAsync();
			return relationships.Select(item => item.chatId).ToList();
		}
	}
}