namespace Messengers
{
	using System.Threading.Tasks;
	using Microsoft.AspNetCore.SignalR;

	public class ChatHub : Hub<ITypedHubClient>
	{
	}
}