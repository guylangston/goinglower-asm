// ---------------------------------------------------------------------------
// PrimeCS.cs : Dave's Garage Prime Sieve in C++
// ---------------------------------------------------------------------------
// Source: https://github.com/davepl/Primes/blob/main/PrimeSieveCS/PrimeCS.cs

using System;
using System.Collections;
using System.Collections.Generic;

namespace PrimeSieveCS
{
    class PrimeCS
    {
        class prime_sieve
        {
            private int sieveSize = 0;
            private BitArray bitArray;

            public prime_sieve(int size)
            {
                sieveSize = size;
                bitArray  = new BitArray((int)((this.sieveSize + 1) / 2), true);
            }

            bool GetBit(int index)
            {
                if (index % 2 == 0)
                    return false;
                return bitArray[index / 2];
            }

            void ClearBit(int index)
            {
                if (index % 2 == 0)
                {
                    Console.WriteLine("You are setting even bits, which is sub-optimal");
                    return;
                }
                bitArray[index / 2] = false;
            }

            public void runSieve()
            {
                var factor = 3; // Break-Here
                var q      = (int)Math.Sqrt(sieveSize);

                while (factor < q)
                {
                    for (var num = factor; num <= sieveSize; num++)
                    {
                        if (GetBit(num))
                        {
                            factor = num;
                            break;
                        }
                    }

                    // If marking factor 3, you wouldn't mark 6 (it's a mult of 2) so start with the 3rd instance of this factor's multiple.
                    // We can then step by factor * 2 because every second one is going to be even by definition

                    for (int num = factor * 3; num <= sieveSize; num += factor * 2)
                        ClearBit(num);

                    factor += 2;
                }
            }
        }

#if SamplePrimeSieveCS
        static void Main(string[] args)
        {
            Console.WriteLine("Starting");
            var sieve = new prime_sieve(1000000);
            sieve.runSieve();
            Console.WriteLine("Complete");
        }
#endif
    }
}