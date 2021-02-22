using System;
using System.Collections.Generic;
using System.IO;

namespace Animated.CPU.Model
{
    public static class ExampleCPU
    {
        public static Random Random = new Random();


        public static byte[] RandomBytes(int count)
        {
            var r = new byte[count];
            Random.NextBytes(r);
            return r;
        }

        private static string Source = @"_start:                         
    mov rax, 0x1122334455667788        
    mov rdi, 1                  
    mov rdx, 1                  
    mov rcx, 64   
.loop:                          
    push rax                    
    sub rcx, 4   
    sar rax, cl                 
    and rax, 0xf 
    lea rsi, [codes + rax]
    mov rax, 1
    push rcx                              
    syscall                     
    pop rcx
    pop rax
    test rcx, rcx
    jnz .loop
    mov     rax, 60            ; invoke 'exit' system call
    xor     rdi, rdi
    syscall";
        
        public static MemoryView Build_Print_Rax()
        {
            MemoryView.Segment Build(string comment, int? size , string label = null) => new MemoryView.Segment()
            {
                Raw     = RandomBytes(size ?? Random.Next(1, 6)),
                Comment = comment,
                Label   = label
            };

            IEnumerable<MemoryView.Segment> BuildAll()
            {
                using var r   = new StringReader(Source);
                string    l   = null;
                string    lbl = null;
                while ((l = r.ReadLine()) != null)
                {
                    if (!l.StartsWith(" ") && l.EndsWith(":"))
                    {
                        lbl = l;
                        continue;
                    }
                    
                    if (l.StartsWith(";")) continue;
                    
                    if (string.IsNullOrWhiteSpace(l)) continue;

                    if (l.StartsWith(" ") || l.StartsWith("\t"))
                    {
                        // code
                        var    i       = l.IndexOf(';');
                        string comment = null;
                        if (i > 0)
                        {
                            comment = l[i..^0];
                            l       = l[0..i];
                        }
                        yield return new MemoryView.Segment()
                        {
                            Raw     = RandomBytes(Random.Next(1, 6)),
                            Label   = lbl,
                            Comment = comment?.Trim(),
                            Source  = l?.Trim()
                        };
                    }
                }
            }

            return new MemoryView(BuildAll());
        }
        public static Cpu BuildCPU()
        {
            var cpu = new Cpu();
            cpu.Instructions = Build_Print_Rax();
            cpu.Stack = new MemoryView(new[]
            {
                new MemoryView.Segment()
                {
                    Source = "Hello World",
                    Raw    = ExampleCPU.RandomBytes(10)
                },
                new MemoryView.Segment()
                {
                    Source = "RAX",
                    Raw    = ExampleCPU.RandomBytes(4)
                },
                new MemoryView.Segment()
                {
                    Source = "RBX",
                    Raw = ExampleCPU.RandomBytes(4)
                }
            });
            return cpu;
        }
    }
}