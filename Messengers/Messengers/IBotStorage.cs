namespace Messengers
{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using Telegram.Bot;

	//Temporarily redundant
	public interface IBotStorage
	{
		int StoreBot(string botToken);

		Task<ITelegramBotClient> GetBotAsync(long chatId);

		//TODO temporary endpoints to get available bot chants
		Task<List<long>> GetChatsAsync(int botId);
	}
}