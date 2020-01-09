using MongoDB.Bson;
namespace Messangers.Models
{
	public class TelegramBotChats
	{

		public ObjectId Id { get; set; }

		public long ChatId { get; set; }

		public TelegramBot TelegramBot { get; set; }
	}
}
