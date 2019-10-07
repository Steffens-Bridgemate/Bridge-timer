using System;
using System.Collections.Generic;
using System.Text;

namespace BridgeTimer.Extensions
{
    public static class TextExtensions
    {
        public static string  PadLeftAndRight(this string text,int desiredTotalLength)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;

            var paddingLength = desiredTotalLength - text.Length;
            if (paddingLength <= 0) return text;

            int padLeft = paddingLength / 2;
            int padRight = padLeft;
            if (paddingLength.IsOdd())
            {
                padLeft--;
            }

            return $"{new string(' ', padLeft)}{text}{new string(' ', padRight)}";
        }
    }
}
