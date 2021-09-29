using ChatCase.Domain.Models.Chatting;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace ChatCase.Business.Services.Chatting
{
    public class UserInfoInMemory
    {
        #region Fields
        private ConcurrentDictionary<string, AppUserInfo> _onlineUser { get; set; } = new ConcurrentDictionary<string, AppUserInfo>();

        #endregion

        #region Methods

        public bool AddUpdate(string name, string connectionId)
        {
            var userAlreadyExists = _onlineUser.ContainsKey(name);

            var userInfo = new AppUserInfo
            {
                UserName = name,
                ConnectionId = connectionId
            };

            _onlineUser.AddOrUpdate(name, userInfo, (key, value) => userInfo);

            return userAlreadyExists;
        }

        public void Remove(string name)
        {
            AppUserInfo userInfo;
            _onlineUser.TryRemove(name, out userInfo);
        }

        public IEnumerable<AppUserInfo> GetAllUsersExceptThis(string username)
        {
            return _onlineUser.Values.Where(item => item.UserName != username);
        }

        public AppUserInfo GetUserInfo(string username)
        {
            AppUserInfo user;
            _onlineUser.TryGetValue(username, out user);
            return user;
        }

        #endregion
    }
}
