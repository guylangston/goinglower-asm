using System;
using System.Globalization;
using System.Linq;

namespace GoingLower.Core.Helpers
{
    public static class ParseHelper
    {
        public static ulong ParseHexWord(string txt)
        {
            var clean = new string(txt.TakeWhile(x => char.IsLetterOrDigit(x)).ToArray());
            try
            {
                return ulong.Parse(clean, NumberStyles.HexNumber);
            }
            catch (Exception e)
            {
                throw new Exception($"Inp: {clean}", e);
            }
            
        }

        public static bool TryParseHexWord(string txt, out ulong res)
        {
            var clean                         = new string(txt.TakeWhile(x => char.IsLetterOrDigit(x)).ToArray());
            if (clean.StartsWith("0x")) clean = clean.Remove(0, 2);
            return ulong.TryParse(clean, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out res);
        } 
            

        public static byte[] ParseHexByteArray(string txt) => Convert.FromHexString(txt);
    }
}