using System;
using System.Diagnostics;

namespace Sample
{
    public class PerfStructSize
    {
        public struct S3
        {
            public int A;
            public int B;
            public int C;
        }

        public int Count = 10;
        
        public int M3()
        {
            var s = new S3()
            {
                A = 10,
                B = 20,
                C = 30
            };
            for (var c = 0; c < Count; c++)
            {
                s.A = s.A + 1;
            }
            return s.A;
        }

        public static int Run()
        {
            var c = new PerfStructSize();
            var r =  c.M3();
            Console.WriteLine($"M3 -> {r}");
            return r;
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

