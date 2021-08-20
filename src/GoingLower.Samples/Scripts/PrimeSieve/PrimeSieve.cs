// ---------------------------------------------------------------------------
// PrimeCS.cs : Dave's Garage Prime Sieve in C++
// ---------------------------------------------------------------------------
// Source: https://github.com/davepl/Primes/blob/main/PrimeSieveCS/PrimeCS.cs

using System;
using System.Collections;
using System.Runtime.CompilerServices;

namespace GoingLower.Samples.Scripts.PrimeSieve
{
    class PrimeCS
    {
       public void runSieve()
       {
           var factor = 3; // Break: Target
           var q      = (int)Math.Sqrt(sieveSize);

           while (factor < q)
           {
               for (var num = factor; 
                   num <= sieveSize; 
                   num++)
               {
                   if (GetBit(num))
                   {
                       factor = num;
                       break;
                   }
               }

               for (int num = factor * 3; 
                   num <= sieveSize; 
                   num += factor * 2)
                   ClearBit(num);

               factor += 2;
           }
       }
       
       private int sieveSize;
       private BitArray bitArray;

       public PrimeCS(int size)
       {
           sieveSize = size;
           bitArray  = new BitArray((int)((this.sieveSize + 1) / 2), true);
       }

       [MethodImpl(MethodImplOptions.AggressiveInlining)]  // Avoid calls for ASM 
       bool GetBit(int index)
       {
           if (index % 2 == 0) return false;
           return bitArray[index / 2];
       }

       [MethodImpl(MethodImplOptions.AggressiveInlining)]  // Avoid calls for ASM 
       void ClearBit(int index)
       {
           if (index % 2 == 0) return;
           bitArray[index / 2] = false;
       }

#if SamplePrimeSieveCS
        static void Main(string[] args)
        {
            Console.WriteLine("Starting");  
            var sieve = new PrimeCS(1000);  // Reduced so we can see more ASM
            sieve.runSieve(); 
            Console.WriteLine("Complete");
        }
#endif
    }
}