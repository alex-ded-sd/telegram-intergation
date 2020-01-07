using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Messangers.Controllers
{
	using Models;

	[Route("api/telegram/")]
	[ApiController]
	public class TelegramController : ControllerBase
	{
		private readonly IBotStorage _storage;
		private readonly TelegramBotHandler _handler;

		public TelegramController(IBotStorage storage, TelegramBotHandler handler) {
			_storage = storage;
			_handler = handler;
		}

		[HttpPost]
		[Route("registerbot")]
		public IActionResult RegisterBot([FromBody]string botToken) {
			try {
				int botId = _storage.StoreBot(botToken);
				return Ok($"Bot successefuly registered. Bot id: {botId}");
			} catch (Exception e) {
				return NotFound("Error");
			}
		}

		[HttpGet]
		[Route("getchats/{botId}")]
		public async Task<List<long>> GetChats(int botId) {
			List<long> chats = await _storage.GetChatsAsync(botId);
			return chats;
		}

		[HttpGet]
		[Route("test")]
		public IActionResult Test() {
			return Ok("ok result");
		}

		[HttpPost]
		[Route("sayhellofrom")]
		public async Task<IActionResult> SayHelloFromAsync([FromBody]SayHello sayHello) {
			string result = await _handler.TellBotAsync(sayHello);
			return Ok(new {message = result});
		}
	}
}