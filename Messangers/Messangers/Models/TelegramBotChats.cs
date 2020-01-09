using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace Messangers.Models
{
	public class TelegramBotChats
	{
	
		public ObjectId Id
		{
		    get; set;
		}
		public long ChatId { get; set; }
		
		[BsonElement]
		public TelegramBot TelegramBot { get; set; }
	}
}
