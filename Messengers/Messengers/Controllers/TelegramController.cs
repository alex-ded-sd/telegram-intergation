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
		private readonly TelegramBotHandler _handler;

		public TelegramController(TelegramBotHandler handler) {
			_handler = handler;
		}

		[HttpPost]
		[Route("registerbot")]
		public IActionResult RegisterBot([FromBody]string botToken) {
			_handler.StoreBot(botToken);
			return Ok($"Bot successefuly registered. Bot token: {botToken}");
		}

		[HttpGet]
		[Route("getchats/{botId}")]
		public List<long> GetChats(int botId) {
			List<long> chats = _handler.GetChats(botId);
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
			return Ok(new { message = result });
		}
	}
}