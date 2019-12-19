namespace Messangers
{
	using System;
	using System.Collections.Generic;
	using Telegram.Bot;

	public interface IBotStorage
	{
		int StoreBot(string botToken);

		ITelegramBotClient GetBot(long chatId);

		//TODO temporary endpoints to get available bot chants
		List<long> GetChats(int botId);
	}
}