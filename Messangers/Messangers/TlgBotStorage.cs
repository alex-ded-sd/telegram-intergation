namespace Messangers
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using Messangers.DAL;
	using Microsoft.EntityFrameworkCore.Internal;
	using MongoDB.Driver;
	using Telegram.Bot;
	using Telegram.Bot.Args;

	public class TlgBotStorage: IBotStorage
	{
		private readonly IMongoCollection<(long chatId, ITelegramBotClient client)> _botClientRelation;

		private readonly IMongoCollection<ITelegramBotClient> _botStorage;

		private readonly DbStoreSettings _dbSettings;

		public TlgBotStorage(DbStoreSettings dbSettings)
		{
			this._dbSettings = dbSettings;
			MongoClient client = new MongoClient(dbSettings.ConnectionString);
			IMongoDatabase database = client.GetDatabase(dbSettings.DataBaseName);
			_botStorage = database.GetCollection<ITelegramBotClient>(dbSettings.TlgBotStorageCollectionName);
			_botClientRelation = database.GetCollection<(long botId, ITelegramBotClient client)>	
				(dbSettings.TlgBotClientRelationship);
		}

		private void StoreBot(ITelegramBotClient client) {
			if (!_botStorage.AsQueryable().Any(bot => client.BotId == bot.BotId)) {
					_botStorage.InsertOne(client);
					client.OnMessage += ClientOnOnMessage;
					client.StartReceiving();
				}
		}
		

		private void ClientOnOnMessage(object sender, MessageEventArgs e) {
			ITelegramBotClient client = sender as ITelegramBotClient;
			long chatId = e.Message.Chat.Id;
			bool isExists = _botClientRelation.AsQueryable()
				.Any(item => item.chatId == chatId && item.client.BotId == client.BotId);
			if (!isExists) {
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
			IAsyncCursor<(long chatId, ITelegramBotClient client)> relationshipsCursor = await _botClientRelation
				.FindAsync(botClientRel => botClientRel.client.BotId == botId);
			List<(long chatId, ITelegramBotClient client)> relationships = await relationshipsCursor.ToListAsync();
			return relationships.Select(item => item.chatId).ToList();
		}
	}
}