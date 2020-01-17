namespace Messengers
{
	using System.Threading.Tasks;
	using Models;

	public interface ITypedHubClient
	{
		Task BroadcastMessage(ContactMessage payload);
	}
}