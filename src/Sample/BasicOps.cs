using System;
using System.Collections.Generic;

namespace Sample
{
    public class BasicOps
    {
        public static int BreakHere()
        {
            var    a = 10;
            var    b = 20;
            var    c = Maths(a, b);
            return c;
        }
        
        public static int Maths(int a, int b)
        {
            a++;
            b--;
            var c1 = a + b;
            var c2 = a - b;
            var c3 = a * b;
            var c4 = a / b;

            var p = Math.Pow(Math.PI, 2);

            var s = 0;
            foreach (var f in Fib(5))
            {
                s += f;
            }
            
            return c1 +  c2 + c3 + c4 + (int)p + s;
        }

        static IEnumerable<int> Fib(int x)
        {
            var r = 1;
            for (int i = 1; i <= x; i++)
            {
                r += i;
                yield return r;
            }
        }
        
        public static int Run()
        {
            // warm up before we set BP
            for (int i = 0; i < 100000; i++)
            {
                var a = 10;
                var b = 20;
                var c = Maths(a, b);    
            }
            BreakHere();
            
            //Console.WriteLine($"Maths({a}, {b}) = {c}");
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

