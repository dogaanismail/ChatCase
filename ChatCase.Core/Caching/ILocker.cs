using System;

namespace ChatCase.Core.Caching
{
    public interface ILocker
    {
        /// <summary>
        /// Perform some action with exclusive lock
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="expirationTime"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        bool PerformActionWithLock(string resource, TimeSpan expirationTime, Action action);
    }
}
