using ChatCase.Business.Services.Chatting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatCase.Framework.Hubs
{
    [Authorize]
    public abstract class ChatDmHub : Hub
    {
        protected UserInfoInMemory _userInfoInMemory;

        protected ChatDmHub(UserInfoInMemory userInfoInMemory)
        {
            _userInfoInMemory = userInfoInMemory;
        }

        protected virtual async Task Leave()
        {
            _userInfoInMemory.Remove(Context.User.Identity.Name);
            await Clients.AllExcept(new List<string> { Context.ConnectionId }).SendAsync("UserLeft", Context.User.Identity.Name);
        }

        protected virtual async Task Join()
        {
            if (!_userInfoInMemory.AddUpdate(Context.User.Identity.Name, Context.ConnectionId))        
                await Clients.AllExcept(new List<string> { Context.ConnectionId }).SendAsync("NewOnlineUser", _userInfoInMemory.GetUserInfo(Context.User.Identity.Name));
            
            await Clients.Client(Context.ConnectionId).SendAsync(
                "Joined",
                _userInfoInMemory.GetUserInfo(Context.User.Identity.Name)
                );

            await Clients.Client(Context.ConnectionId).SendAsync(
                "OnlineUsers",
                _userInfoInMemory.GetAllUsersExceptThis(Context.User.Identity.Name)
            );
        }

        protected virtual Task SendDirectMessage(string message, string targetUserName)
        {
            var userInfoSender = _userInfoInMemory.GetUserInfo(Context.User.Identity.Name);
            var userInfoReciever = _userInfoInMemory.GetUserInfo(targetUserName);
            return Clients.Client(userInfoReciever.ConnectionId).SendAsync("SendDM", message, userInfoSender);
        }
    }
}
