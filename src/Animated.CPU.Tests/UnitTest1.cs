using System;
using System.Text;
using Animated.CPU.Animation;
using Xunit;
using Xunit.Abstractions;

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
    
    public class CodeGenStratch
    {

        private ITestOutputHelper outp;
        public CodeGenStratch(ITestOutputHelper outp)
        {
            this.outp = outp;
        }
        
        [Fact]
        public void CopyConstructor()
        {
            outp.WriteLine(CodeGen.GenerateCopyConstructor<DBlockProps>());
        }
    }
}