using System;

namespace BridgeTimer.Services
{
    public class TimeProvider : ITimeProvision
    {
        public string GetCurrentTime()
        {
            return System.TimeProvider.System.GetUtcNow().ToString();
            return DateTime.Now.ToLongTimeString();
        }
    }
}
