namespace Messengers.Models
{
	using MongoDB.Bson;

	public class TelegramBotChat
	{
	
		public ObjectId Id { get; set; }
		public long ChatId { get; set; }
		public int BotId { get; set; }
	}
}
