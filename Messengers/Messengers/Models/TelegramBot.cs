using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace Messangers.Models
{
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
