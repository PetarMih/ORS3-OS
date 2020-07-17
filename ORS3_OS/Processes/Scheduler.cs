using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;

namespace ORS3_OS.Processes
{
    class Scheduler
    {
        public List<Process> Ready { get; set; }
        public List<Process> Running { get; set; }
        public List<Process> Blocked { get; set; }
        public Thread re_ru { get; set; } // Iz Ready u Running
        public Thread ru_re { get; set; } // Iz Running u Ready
        public Thread block { get; set; } // Blokiranje
        public Thread unblock { get; set; } //Odblokiranje

        public Scheduler() {
            Ready = new List<Process>();
            Running = new List<Process>();
            Blocked = new List<Process>();
        }

        // prioritet ima onaj proces koji je najmanje korisito CPU (CPU_Utilization 0-100%)
        private Process HighestPriority()
        {
            int cpu = 100;
            Process p = null;
            foreach(Process pr in this.Ready)
            {
                if (pr.CPU_usage <= cpu)
                {
                    cpu = pr.CPU_usage;
                    p = pr;
                }
            }
            return p;
        }

        public void ReadyToRunning()
        {
            Process p = this.HighestPriority();
            p.Status = "Running";
            p.CPU_usage++;
            this.Ready.Remove(p);
            this.Running.Add(p);

            Console.WriteLine("ReadyToRunning");
            //Thread.Sleep(1000);
        }
        // prebacujemo onaj proces koji je najvise koristio CPU
        public void RunningToReady()
        {
            int cpu = 0;
            Process p = null;
            foreach(Process pr in this.Running)
            {
                if (pr.CPU_usage >= cpu)
                {
                    cpu = pr.CPU_usage;
                    p = pr;
                }
            }

            p.Status = "Ready";
            Running.Remove(p);
            Ready.Add(p);
            

            Thread.Sleep(1000);
            Console.WriteLine("RunningTOReady");
        }

        public void Block()
        {
            foreach(Process p in this.Running)
            {
                if (p.Status == "Blocked")
                {
                    this.Blocked.Add(p);
                    this.Running.Remove(p);
                }
            }
            Thread.Sleep(500);
        }

        public void Unblock()
        {
            foreach(Process p in this.Blocked)
            {
                if (p.Status == "Ready")
                {
                    this.Blocked.Remove(p);
                    this.Ready.Add(p);
                }
            }
            Thread.Sleep(1000);
        }
        
        public void RunThread()
        {
            this.re_ru = new Thread(new ThreadStart(this.ReadyToRunning));
            this.ru_re = new Thread(new ThreadStart(this.RunningToReady));
            this.block = new Thread(new ThreadStart(this.Block));
            this.unblock = new Thread(new ThreadStart(this.Unblock));

            re_ru.Start();
            ru_re.Start();
            block.Start();
            unblock.Start();

        }

        public void TaksManager()
        {
            Console.WriteLine("Task Manager!");
            try
            {
                String path = @"C:\Users\Dell\Documents\GitHub\ORS3-OS\ORS3_OS\Processes\TaskManager\file.txt";
                StreamWriter sw = File.CreateText(path);
                String[] head = { "Name", "PID", "Status", "Memory", "CPU_Usage" };
                sw.WriteLine(head);
                //sw.WriteLine("Operating system");
                

                foreach(Process p in this.Running)
                {
                    head[0] = p.Name;
                    head[1] = p.Pid.ToString();
                    head[2] = p.Status;
                    head[3] = p.Memory.ToString();
                    head[4] = p.CPU_usage.ToString();
                    sw.WriteLine(head);
                }
                foreach (Process p in this.Ready)
                {
                    head[0] = p.Name;
                    head[1] = p.Pid.ToString();
                    head[2] = p.Status;
                    head[3] = p.Memory.ToString();
                    head[4] = p.CPU_usage.ToString();
                    sw.WriteLine(head);
                }
                foreach (Process p in this.Blocked)
                {
                    head[0] = p.Name;
                    head[1] = p.Pid.ToString();
                    head[2] = p.Status;
                    head[3] = p.Memory.ToString();
                    head[4] = p.CPU_usage.ToString();
                    sw.WriteLine(head);
                }
                sw.Close();

            }
            catch(IOException e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public void AddProcess(Process p)
        {
            Ready.Add(p);
        }

        public void Monitor()
        {
            while (true)
            {
                foreach(Process p in this.Running)
                {
                    p.Utilization();
                    p.Running();
                    p.Ready();
                    p.Blocked();
                    
                }
            }
        }

    }
}
