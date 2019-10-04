using System;

namespace BridgeTimer.Services
{
    public class TimeProvider : ITimeProvision
    {
        public string GetCurrentTime()
        {
            return DateTime.Now.ToLongTimeString();
        }
    }
}
