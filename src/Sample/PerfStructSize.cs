using System;
using System.Diagnostics;

namespace Sample
{
    public class PerfStructSize
    {
        public struct S5
        {
            public int A;
            public int B;
            public int C;
            public int D;
            public int E;
        }

        public struct S3
        {
            public int A;
            public int B;
            public int C;
        }

        public int Count = 1000;
        
        public int M5()
        {
            var s = new S5();
            for (var c = 0; c < Count; c++)
            {
                s.A = s.A + 1;
            }
            return s.A;
        }

        public int M3()
        {
            var s = new S3();
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

            r = c.M5();
            Console.WriteLine($"M5 -> {r}");
            return r;
        }
    }
    
    #if SINGLE_FILE
    // Hack to allow many single files and cmd exec
    class Program
    {
        public static int Main() => PerfStructSize.Run();
    }
    #endif
}

