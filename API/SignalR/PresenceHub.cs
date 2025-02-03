using API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{

    [Authorize]
    public class PresenceHub(PresenceTracker tracker):Hub
    {

        public override async Task OnConnectedAsync()
        {   
            if(Context.User== null) throw new HubException("Unauthorized");
            await tracker.UserConnected(Context.User.GetUsername(), Context.ConnectionId);

            await Clients.Others.SendAsync("UserIsOnline",
                Context.User.GetUsername());

            var onlineUsers = await tracker.GetOnlineUsers();
            await Clients.All.SendAsync("GetOnlineUsers", onlineUsers);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {

            if (Context.User == null) throw new HubException("Unauthorized");
            await tracker.UserDisconnected(Context.User.GetUsername(), Context.ConnectionId);
            await Clients.Others.SendAsync("userIsOffline",Context.User?.GetUsername());


            var onlineUsers = await tracker.GetOnlineUsers();
            await Clients.All.SendAsync("GetOnlineUsers", onlineUsers);

            await base.OnDisconnectedAsync(exception);
        }


    }
}
