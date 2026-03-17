using System.Globalization;

namespace FunClicker.Utils
{
    public static class NumberFormatter
    {
        private static readonly NumberFormatInfo DotGroupFormat = new NumberFormatInfo
        {
            NumberGroupSeparator = ".",
            NumberDecimalDigits = 0
        };

        public static string Format(long value)
        {
            return value.ToString("#,0", DotGroupFormat);
        }
    }
}
