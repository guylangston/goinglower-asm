using System.Text;

namespace Animated.CPU.Tests
{
    public class CodeGen
    {
        public static string GenerateCopyConstructor<T>()
        {
            StringBuilder sb = new StringBuilder();
            
            var type = typeof(T);

            foreach (var prop in type.GetProperties())
            {
                sb.AppendLine($"\tthis.{prop.Name} = copy.{prop.Name};");
            }
            return sb.ToString();
        }
        
    }
}