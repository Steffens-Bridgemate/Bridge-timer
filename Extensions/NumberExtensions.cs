using System;
using System.Collections.Generic;
using System.Text;

namespace BridgeTimer.Extensions
{
    public static class NumberExtensions
    {
        public static bool IsEven(this int number)
        {
            return number % 2 == 0;
        }

        public static bool  IsOdd(this int number)
        {
            return !IsEven(number);
        }
    }
}
