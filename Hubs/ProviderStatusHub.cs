using Microsoft.AspNetCore.SignalR;
using ServiceManagementAPI.Enums;
namespace ServiceManagementAPI.Hubs
{
    public class ProviderStatusHub : Hub
    {
        private readonly ServiceManagementDbContext _context;
        // Using a static dictionary to store in-memory status since your Status property is [NotMapped]
        private static readonly Dictionary<string, AvailabilityStatus> _providerStatuses = new();

        public ProviderStatusHub(ServiceManagementDbContext context)
        {
            _context = context;
        }

        public async Task GetStatus(string userId)
        {
            // Try to get from in-memory dictionary first
            if (_providerStatuses.TryGetValue(userId, out var status))
            {
                await Clients.Caller.SendAsync("ReceiveCurrentStatus", status);
                return;
            }

            // Fall back to default status
            await Clients.Caller.SendAsync("ReceiveCurrentStatus", AvailabilityStatus.Offline);
        }

        public async Task UpdateStatus(string userId, AvailabilityStatus status)
        {
            // Update the in-memory dictionary
            _providerStatuses[userId] = status;
            Console.WriteLine("Updating status");
            // Send the update to all clients
            await Clients.All.SendAsync("ReceiveStatusUpdate", userId, status);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            // Optional: You could set the user to offline when they disconnect
            var contextConnectionId = Context.ConnectionId;
            var userId = Context.User?.Identity?.Name; // If you're using authentication

            if (userId != null && _providerStatuses.ContainsKey(userId))
            {
                _providerStatuses[userId] = AvailabilityStatus.Offline;
                await Clients.All.SendAsync("ReceiveStatusUpdate", userId, AvailabilityStatus.Offline);
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}