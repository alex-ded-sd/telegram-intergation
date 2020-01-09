using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace Messangers.Models
{
	public class TelegramBotChat
	{
	
		public ObjectId Id { get; set; }
		public long ChatId { get; set; }
		public int BotId { get; set; }
	}
}
