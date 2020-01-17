namespace Messengers.Models
{
	public class ContactMessage
	{
		public string Message {
			get; set;
		}

		public string UserName { get; set; }

		public long ChatId {
			get; set;
		}
	}
}