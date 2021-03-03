using System;
using System.Diagnostics;

namespace Sample
{
    public class BasicOps
    {
        public static int Maths(int a, int b)
        {
            a++;
            b--;
            var c1 = a + b;
            var c2 = a - b;
            var c3 = a * b;
            var c4 = a / b;

            return c1 +  c2 + c3 + c4;
        }
        
        public static int Run()
        {
            var a = 10;
            var b = 20;
            var c = Maths(a, b);
            Console.WriteLine($"Maths({a}, {b}) = {c}");
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

