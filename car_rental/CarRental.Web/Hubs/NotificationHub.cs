using Microsoft.AspNetCore.SignalR;

namespace CarRental.Web.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task SendOrderUpdate(string message, int orderId)
        {
            await Clients.All.SendAsync("ReceiveOrderUpdate", message, orderId);
        }
    }
}
