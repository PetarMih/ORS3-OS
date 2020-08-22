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
            foreach (Process pr in this.Ready)
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
            if (p == null || Ready.Count == 0)
            {
                //Console.WriteLine("No ready process.");
            }
            else
            {
                if (!SwitchContextIfNecessary(p))
                {
                    return;
                }
                p.Status = "Running";
                p.CPU_usage++;
                Ready.Remove(p);
                Running.Add(p);
                //Console.WriteLine("ReadyToRunning");
                //Console.WriteLine("Process status: " + p.Status);
                //Console.WriteLine("Running count: "+Running.Count);
            }

        }
        // prebacujemo onaj proces koji je najvise koristio CPU
        public void RunningToReady()
        {
            int cpu = 0;
            Process p = null;
            foreach (Process pr in this.Running)
            {
                if (pr.CPU_usage >= cpu)
                {
                    cpu = pr.CPU_usage;
                    p = pr;
                }
            }

            if (p == null || Running.Count == 0)
            {
                //Console.WriteLine("No running process.");
            }
            else
            {
                p.Status = "Ready";
                Running.Remove(p);
                Ready.Add(p);
                //Console.WriteLine("RunningTOReady");
            }
        }

        public void BlockProcess(int pid)
        {
            lock (Running)
            {
                foreach(Process p in Running)
                {
                    if (p.Pid == pid)
                    {
                        p.Blocked();
                        Running.Remove(p);
                        Blocked.Add(p);
                    }
                }
            }
        }

        public void Unblock()
        {
            foreach (Process p in this.Blocked)
            {
                if (p.Status == "Ready")
                {
                    this.Blocked.Remove(p);
                    this.Ready.Add(p);
                    Console.WriteLine("Process unblocked.");
                }
            }

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
        
        public bool SwitchContextIfNecessary(Process toRun)
        {
            // ako se proces vec nalazi u RAMu ne diraj nista
            if(MainClass.RAM.Contains(toRun.Name))
            {
                return true;
            }

            // ako fali memorija na RAMu, oslobodi RAM memorije onih procesa koji su u Blocked stanju
            if (MainClass.RAM.Count + toRun.Memory > MainClass.MAX_RAM)
            {
                // proc = getPriority and ready
                foreach(Process proc in this.Ready)
                {
                    if (MainClass.RAM.Contains(proc.Name))
                    {
                        MainClass.RAM.RemoveAll(x => x.Equals(proc.Name));
                        for(int i=0; i < proc.Memory; ++i)
                        {
                            MainClass.HardDriveMemory.Add(proc.Name);
                        }
                    }
                }
            }

            if (MainClass.RAM.Count + toRun.Memory <= MainClass.MAX_RAM)
            {
                // ako proces nije na RAMu skini ga sa Hard diska
                MainClass.HardDriveMemory.RemoveAll(x => x.Equals(toRun.Name));

                for (int i = 0; i < toRun.Memory; ++i)
                {
                    MainClass.RAM.Add(toRun.Name);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public void TaskManager()
        {
            Console.WriteLine("Task Manager!");
            try
            {
                String path = @"C:\Users\Dell\Documents\GitHub\ORS3-OS\ORS3_OS\Processes\TaskManager\file.txt";
                StreamWriter sw = File.CreateText(path);
                String[] info = { "Name", "PID", "Status", "CPU_Usage(%)", "Memory(KB)" };
                foreach (String s in info)
                {
                    sw.Write("      " + s + "        ");
                }
                sw.Write("\n");
                sw.Write("------------------------------------------------------------------------------------------");
                sw.Write("\n");
                //sw.WriteLine("Operating system");

                foreach (Process p in this.Running)
                {
                    info = p.Info();
                    foreach (String s in info)
                    {
                        sw.Write("      " + s + "        ");
                    }
                    sw.Write("\n");
                }
                foreach (Process p in this.Ready)
                {
                    info = p.Info();
                    foreach (String s in info)
                    {
                        sw.Write("      " + s + "        ");
                    }
                    sw.Write("\n");
                }
                foreach (Process p in this.Blocked)
                {
                    info = p.Info();
                    foreach (String s in info)
                    {
                        sw.Write("      " + s + "        ");
                    }
                    sw.Write("\n");
                }

                sw.Close();

            }
            catch (IOException e)
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
            System.Timers.Timer time = new System.Timers.Timer(500);
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
            foreach (Process p in Running)
            {
                p.CPU_usage++;
            }
            //Console.WriteLine("Process is using cpu! "+e.SignalTime);
        }

        private void ProcessFinish(object sender, System.Timers.ElapsedEventArgs e)
        {
            foreach (Process p in Running)
            {
                if (p.CPU_usage == 100)
                {
                    p.Finish();
                    Running.Remove(p);
                    break;
                }
            }
        }

        public void ReadyToRunningStart()
        {
            while (true)
            {
                ReadyToRunning();
                Thread.Sleep(500);
            }

        }
        public void RunningToReadyStart()
        {
            while (true)
            {
                RunningToReady();
                Thread.Sleep(800);
                Thread.Yield();
            }

        }
        public void BlockStart()
        {
            while (true)
            {
                //BlockProcess(int p);
                Thread.Sleep(1000);
            }
        }
        public void UnblockStart()
        {
            Unblock();
            Thread.Sleep(1000);
        }

    }
}
