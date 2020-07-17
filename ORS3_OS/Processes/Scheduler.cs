using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;
using System.Timers;

namespace ORS3_OS.Processes
{
    class Scheduler
    {
        public List<Process> Ready { get; set; }
        public List<Process> Running { get; set; }
        public List<Process> Blocked { get; set; }
        /*
        public Thread re_ru { get; set; } // Iz Ready u Running
        public Thread ru_re { get; set; } // Iz Running u Ready
        public Thread block { get; set; } // Blokiranje
        public Thread unblock { get; set; } //Odblokiranje
        */

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

            if (p == null)
            {
                Console.WriteLine("No running process.");
            }
            else
            {
                p.Status = "Ready";
                p.CPU_usage++;
                Running.Remove(p);
                Ready.Add(p);
            }
            

            //Thread.Sleep(100);
            //Thread.Yield();
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
                    Console.WriteLine("Process blocked.");
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
                    Console.WriteLine("Process unblocked.");
                }
            }
            Thread.Sleep(1000);
            
        }
        /*
        public void RunThread()
        {
            this.re_ru = new Thread(new ThreadStart(this.ReadyToRunning));
            this.ru_re = new Thread(new ThreadStart(this.RunningToReady));
            this.block = new Thread(new ThreadStart(this.Block));
            this.unblock = new Thread(new ThreadStart(this.Unblock));

            re_ru.Name = "ReadyTORunning";

            re_ru.Start();
            ru_re.Start();
            block.Start();
            unblock.Start();

        }
        */
        public void TaskManager()
        {
            Console.WriteLine("Task Manager!");
            try
            {
                String path = @"C:\Users\Dell\Documents\GitHub\ORS3-OS\ORS3_OS\Processes\TaskManager\file.txt";
                StreamWriter sw = File.CreateText(path);
                String[] info = { "Name", "PID", "Status", "CPU_Usage", "Memory" };
                foreach(String s in info)
                {
                    sw.Write(s + "      ");
                }
                sw.Write("\n");
                sw.Write("-------------------------------------------------------------------");
                sw.Write("\n");
                //sw.WriteLine("Operating system");
                
                foreach(Process p in this.Running)
                {
                    info = p.Info();
                    foreach(String s in info)
                    {
                        sw.Write(s + "        ");
                    }
                    sw.Write("\n");
                }
                foreach (Process p in this.Ready)
                {
                    info = p.Info();
                    foreach (String s in info)
                    {
                        sw.Write(s + "        ");
                    }
                    sw.Write("\n");
                }
                foreach (Process p in this.Blocked)
                {
                    info = p.Info();
                    foreach (String s in info)
                    {
                        sw.Write(s + "        ");
                    }
                    sw.Write("\n");
                }

                sw.Close();

            }
            catch(IOException e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public void NewProcess(String Name)
        {
            Process p = new Process(Name);
            Ready.Add(p);
        }
        /* svake sekunde se poziva funkcija cpu_usage
         * koja ako je proces u stanju running 
         * povecava za 1 njegov atribut CPU_usage
         */
        public void CPUUtilization()
        {
            System.Timers.Timer time = new System.Timers.Timer(1000);
            System.Timers.Timer time1 = new System.Timers.Timer(1000);
            time.AutoReset = true;
            time1.AutoReset = true;
            time.Elapsed += new System.Timers.ElapsedEventHandler(CPU_usage);
            time1.Elapsed += new System.Timers.ElapsedEventHandler(ProcessFinish);
            time.Start();
            time1.Start();

        }
        private void CPU_usage(object sender, System.Timers.ElapsedEventArgs e)
        {
            foreach(Process p in Running)
            {
                p.CPU_usage++;
            }
            Console.WriteLine("Process is using cpu! "+e.SignalTime);
        }

        private void ProcessFinish(object sender, System.Timers.ElapsedEventArgs e)
        {
            foreach(Process p in Running)
            {
                if (p.CPU_usage == 100)
                {
                    Console.WriteLine("Process: "+p.Name+" finished!");
                    Running.Remove(p);
                }
            }
        }

        public void Work()
        {
            ReadyToRunning();
            RunningToReady();
            Block();
            Unblock();
        }

        public void ReadyToRunningStart()
        {
            while (true)
            {
                ReadyToRunning();
                Thread.Sleep(500);
                Thread.Yield();
            }
            
        }
        public void RunningToReadyStart()
        {
            while (true)
            {
                RunningToReady();
                Thread.Sleep(400);
            }
        }

    }
}
