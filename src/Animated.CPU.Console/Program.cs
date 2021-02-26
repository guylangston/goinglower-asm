﻿using System;
using Animated.CPU.Backend.LLDB;

namespace Animated.CPU.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var p = new DebugDriverLldb();
            p.Start();
        }
    }
} 