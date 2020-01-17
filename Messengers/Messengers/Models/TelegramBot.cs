namespace Messengers.Models
{
	using MongoDB.Bson;

	public class TelegramBot
	{
		public ObjectId Id {
			get; set;
		}
		public int BotId {
			get; set;
		}

		public string BotToken {
			get; set;
		}
	}
}
