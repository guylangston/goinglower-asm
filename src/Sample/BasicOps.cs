using System;
using System.Diagnostics;

namespace Sample
{
    public class BasicOps
    {
        public static int Run()
        {
            var a = 10;
            var b = 20;
            var c1 = a + b;
            var c2 = a - b;
            var c3 = a * b;
            var c4 = a / b;
            
            Console.WriteLine($"{a} + {b} = {c1}");
            
            
            return 0;
        }
    }
    
    #if BasicOps
    // Hack to allow many single files and cmd exec
    class Program
    {
        public static int Main() => BasicOps.Run();
    }
    #endif
}

