using System.Linq;
using System.Text;
using GoingLower.Core.Helpers;
using GoingLower.CPU.Parsers;
using Xunit;
using Xunit.Abstractions;

namespace GoingLower.Tests
{
    public class SimpleLineTokenizer
    {
        private readonly ITestOutputHelper outp;
        
        private string Code = @"// C# dotnet core 5, linux
int ForLoop(int count)
{
    var sum = 0;
    var name = ""Guy int Langston"";
    for (var x = 0;
        x < count;
        x++)
    {
        sum = sum + x;  // inner
    }
    return sum;
}
";

        public SimpleLineTokenizer(ITestOutputHelper outp)
        {
            this.outp = outp;
        }

        [Fact]
        public void CanParse_CSharp()
        {
            var cs = new SourceParser(new SyntaxCSharp());
            var res = cs.Parse(StringHelper.ToLines(Code).ToArray());

            Assert.NotEmpty(res.LineTokens);
            
            var sb = new StringBuilder();
            foreach (var line in res.Walk())
            {
                sb.Append(line.Text);
            }
            
            outp.WriteLine(sb.ToString());
            
            Assert.Equal(Code, sb.ToString());
        }

        void StringRoundTrip(string single)
        {
            var cs           = new SourceParser(new SyntaxCSharp());
            var lines = StringHelper.ToLines(single).ToArray();
            var res          = cs.Parse(lines);

            Assert.NotEmpty(res.LineTokens);


            var sb = new StringBuilder();
            foreach (var line in res.Walk())
            {
                sb.Append(line.Text);
            }
            
            
            outp.WriteLine(sb.ToString());
            
            
            Assert.Equal(single, sb.ToString().TrimEnd('\n'));
        } 
        
        [Fact]
        public void CanParse_CSharp_SingleChar() => StringRoundTrip("{");

        [Fact]
        public void CanParse_CSharp_SingleLineQuote() => StringRoundTrip("xxx\"yyyyy\";zzz");
        
        [Fact]
        public void CanParse_CSharp_SingleLineQuote_InnerInt() => StringRoundTrip("xxx\"yinty\";zzz");
        
        [Fact]
        public void CanParse_CSharp_SingleLineQuote_InnerInt2() => StringRoundTrip("xxx\"yinty\";");
        
        
        [Fact]
        public void MarkDown_URL()
        {
            var cs  = new SourceParser(new SyntaxMarkDown());
            var res = cs.Parse(new []
            {
                "Nothing",
                "- [xname](xurl) B",
                "- ![xname](ximage) B"
            });

            Assert.NotEmpty(res.LineTokens);


            var sb = new StringBuilder();
            foreach (var line in res.Walk())
            {
                sb.Append(line.Text);
            }
            
            
            outp.WriteLine(sb.ToString());
            
            
            Assert.Equal(@"Nothing
A xname B
A xname B", sb.ToString());


        }
    }
}