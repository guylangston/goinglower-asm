using System;
using Animated.CPU.Animation;
using Xunit;
using Xunit.Abstractions;

namespace Animated.CPU.Tests
{
    public class CodeGenTests
    {

        private ITestOutputHelper outp;
        public CodeGenTests(ITestOutputHelper outp)
        {
            this.outp = outp;
        }
        
        [Fact]
        public void CopyConstructor()
        {
            outp.WriteLine(CodeGen.GenerateCopyConstructor<DBorderStyled>());
        }
    }
}