using System.Text;

namespace GoingLower.Core.Helpers
{
    public static class DisplayHelper
    {
        public static string ToHex(ulong v) => v.ToString("X");
        public static string ToHex64(ulong v) => v.ToString("X").PadLeft(64/8*2, '0');

        public static string ToHex(this byte[] arr)
        {
            var sb = new StringBuilder();

            foreach (var b in arr)
            {
                if (sb.Length> 0) sb.Append(":");
                
                sb.Append(b.ToString("X").PadLeft(2, '0'));
                
            }
            return sb.ToString();
        }
    }
}