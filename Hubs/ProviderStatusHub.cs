using Microsoft.AspNetCore.SignalR;
using ServiceManagementAPI.Enums;
namespace ServiceManagementAPI.Hubs
{
    public class ProviderStatusHub : Hub
    {
        private readonly ServiceManagementDbContext _context;

        private static readonly Dictionary<string, AvailabilityStatus> _providerStatuses = new();

        public ProviderStatusHub(ServiceManagementDbContext context)
        {
            _context = context;
        }

        public async Task GetStatus(string userId)
        {
            if (_providerStatuses.TryGetValue(userId, out var status))
            {
                await Clients.Caller.SendAsync("ReceiveCurrentStatus", status);
                return;
            }
            await Clients.Caller.SendAsync("ReceiveCurrentStatus", AvailabilityStatus.Offline);
        }

        public async Task UpdateStatus(string userId, AvailabilityStatus status)
        {
            _providerStatuses[userId] = status;
            await Clients.All.SendAsync("ReceiveStatusUpdate", userId, status);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var contextConnectionId = Context.ConnectionId;
            var userId = Context.User?.Identity?.Name;

            if (userId != null && _providerStatuses.ContainsKey(userId))
            {
                _providerStatuses[userId] = AvailabilityStatus.Offline;
                await Clients.All.SendAsync("ReceiveStatusUpdate", userId, AvailabilityStatus.Offline);
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}