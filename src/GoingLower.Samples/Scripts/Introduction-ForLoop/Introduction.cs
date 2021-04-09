using System;
using System.Diagnostics;

namespace GoingLower.Samples
{
    public class Introduction
    {
        public static int ForLoop(int count)
        {
            var sum = 0;  // Break-Here
            for (var x = 0; 
                x < count; 
                x++)
            {
                sum = sum + x;
            }
            return sum;
        }
        
        public static int WhileLoop(int count)
        {
            var sum = 0;
            var x = 0;
            while(x<count)
            {
                sum = sum + x;
                x++;
            }
            return sum;
        }
    }
    
    #if SampleIntroduction
    class Program
    {
        public static int Main()
        {
            var a = Introduction.ForLoop(5);  
            var b = Introduction.WhileLoop(5);
            return 0;
        }
    }
    #endif
}

