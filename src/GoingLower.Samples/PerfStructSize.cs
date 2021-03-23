using System;
using System.Diagnostics;

namespace GoingLower.Samples
{
    public class PerfStructSize
    {
        public static int Sum(int count)
        {
            var s = 0;
            for (var c = 0; c < count; c++)
            {
                s = s + c;
            }
            return s;
        }

        public static int Run()
        {
            var r =  Sum(10);
            Console.WriteLine($"Sum -> {r}");
            return r;
        }
        
        public struct S3
        {
            public int A;
            public int B;
            public int C;
        }
        
        public int M3()
        {
            var s = new S3()
            {
                A = 10,
                B = 20,
                C = 30
            };
            for (var c = 0; c < 10; c++)
            {
                s.A = s.A + 1;
            }
            return s.A;
        }
    }
    
    #if PerfStructSize
    // Hack to allow many single files and cmd exec
    class Program
    {
        public static int Main() => PerfStructSize.Run();
    }
    #endif
}

