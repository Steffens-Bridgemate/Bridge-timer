﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BridgeTimer
{
    public static class TimeSpanExtensions
    {
        public static TimeSpan AddMinute(this TimeSpan span)
        {
            return span.Add(TimeSpan.FromMinutes(1));
        }

        public static TimeSpan SubtractSecond(this TimeSpan span)
        {
            return span.Subtract(TimeSpan.FromSeconds(1));
        }
    }
}
