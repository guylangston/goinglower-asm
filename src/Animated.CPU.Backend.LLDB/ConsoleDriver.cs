using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Animated.CPU.Backend.LLDB
{
    public abstract class ConsoleDriver
    {
        private Process proc;
        private DateTime start;
        private DateTime update;
        private DateTime lastCmd;
        private volatile string? last = null;
        private List<string> currResult = new List<string>();
        private readonly object locker = new object();
        private bool echo = true;
        private int seq;
        
        public void Open(ProcessStartInfo start)
        {
            if (CapturePath != null)
            {
                WriteStatus("CTL", TimeSpan.Zero, CapturePath);
            }
            
            using (proc = new Process()
            {
                StartInfo           = start,
                EnableRaisingEvents = true,
            })
            {
                proc.OutputDataReceived += ProcOnOutputDataReceived;
                proc.ErrorDataReceived  += ProcOnErrorDataReceived;
                proc.Exited             += ProcOnExited;
                proc.Start();
                
                proc.BeginErrorReadLine();
                proc.BeginOutputReadLine();
                
                this.lastCmd = this.update = this.start = DateTime.Now;

                CaptureResults(1d);
                ProcessOutput();
                proc.WaitForExit(2000 );    
            }
        }
        
        protected bool                  IsReady     { get; private set; }
        protected IReadOnlyList<string> LastResult  { get; private set; }
        public    string                CapturePath { get; set; }
        
        protected abstract void ProcessOutput();

        public void Capture(IEnumerable<string> lines, string name, string ext)
        {
            if (CapturePath is {})
            {
                var fileName = $"S{seq:0000}-{name}.{ext}";
                var fullPath = Path.Combine(CapturePath, fileName);
                File.WriteAllLines(fullPath, lines); 
                WriteStatus("CTL", TimeSpan.Zero, fileName);
                seq++;
            }
            
        }


        private void ProcOnExited(object? sender, EventArgs e)
        {
            Console.WriteLine($"Exiting...");
        }

        private void ProcOnErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine($"ERR|{e.Data}");
        }

        private void ProcOnOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            last   = e.Data;
            update = DateTime.Now;
            var offset = update - start;
            lock (this)
            {
                IsReady = false;
                currResult.Add(e.Data);    
            }
            if (echo)
            {
                WriteStatus("-->", offset, e.Data);    
            }
            
        }   
        
        protected void WriteStatus(string desc, TimeSpan offset, string txt)
        {
            const int m               = 4;
            if (desc.Length > m) desc = desc.Substring(0, m);
            
            var t  = DateTime.Now;
            var dt = t - start; 
            
            Console.WriteLine($"{dt.TotalSeconds,10:#,##0.000} {desc,-m} {txt}");
        }

        
        
        protected void ExecuteCommand(string cmd)
        {
            currResult.Clear();
            lastCmd = DateTime.Now;
            var offset = lastCmd -  update;

            if (echo)
            {
                Console.WriteLine("------------------------------------------------------------");
                WriteStatus("<--", offset, cmd);    
            }
            
            last = null;
            
            proc.StandardInput.WriteLine(cmd);
        }
        
        public IReadOnlyList<string> ExecuteAndWaitForResults(string cmd, double threshold = 0.3d, bool echo = true)
        {
            // Assumes underlying is already READY
            if (!IsReady) throw new Exception("Must be Ready for Cmd");

            this.echo = echo;
            ExecuteCommand(cmd);
            CaptureResults(threshold);
            this.echo = false;

            return LastResult;
        }
        
        private IReadOnlyList<string> CaptureResults(double threshold)
        {
            int cc = 0;
            while (true)
            {
                var t  = DateTime.Now;
                var dt = t - update;
                var di = t - lastCmd;

                if (dt.TotalSeconds > threshold && di.TotalSeconds > threshold)
                {
                    lock (locker)
                    {
                        IsReady = true;
                        return LastResult = currResult.ToImmutableArray();
                    }
                }
                if (dt.TotalSeconds > 10)
                {
                    throw new Exception($"Input Timeout; last={last}, update={update}, cmd={lastCmd}");
                }
                Thread.Sleep(100);
                cc++;
            }
        }
    }
}