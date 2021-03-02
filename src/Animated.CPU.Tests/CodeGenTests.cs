using System;
using System.Linq;
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

    public class StringHelperTests
    {
        [Fact]
        public void ParseNameValueBlocks()
        {
            var txt =
                @"// Sample NameValue Blocks File
0001
    FirstName: Guy
    LastName: Langston
    Age: 40
    Birthday    : 1976-01-01        
    // Line Above should have whitespace at the end of line

0002
    Hello: World

0003

// Comment Line
0004
    FirstName: Bev";
            var res = StringHelper.ParseNameValueBlocks(StringHelper.ToLines(txt)).ToList();
            
            Assert.Equal("0001",res[0].Name);
            Assert.Equal("FirstName", res[0].Values[0].name);
            Assert.Equal("Guy", res[0].Values[0].val);
        }
    }
}