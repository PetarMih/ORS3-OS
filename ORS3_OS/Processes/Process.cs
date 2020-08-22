using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace ORS3_OS.Processes
{
    class Process
    {
        public int Pid { get; set; }
        public String Status { get; set; }
        public int Memory { get; set; }
        public int CPU_usage { get; set; }
        public String Name { get; set; }
        

        
        public Process (String Name)
        {
            Random rnd = new Random();
            Pid = rnd.Next(1000, 10000);
            this.Memory = new Random().Next(12);
            this.Name = Name;
            CPU_usage = 0;
            Status = "Ready";
            List<string> putOn = null;
            if(MainClass.RAM.Count + this.Memory <= MainClass.MAX_RAM)
            {
                putOn = MainClass.RAM;
            }
            else
            {
                putOn = MainClass.HardDriveMemory;
            }
            for (int i = 0; i < Memory; ++i)
            {
                putOn.Add(Name);
            }
            //Console.WriteLine("Process created.");
        }

        public void Utilization()
        {
            if (this.Status.Equals("Running"))
            {
                CPU_usage++;
            }
            Console.WriteLine("Process"+Name+"have:"+CPU_usage+" CPU usage.");
        }

        public void Blocked()
        {
            Status = "Blocked";
        }
        public void Ready()
        {
            Status = "Ready";
            Console.WriteLine("Process" + this.Name + "is ready.");
        }
        
        public void Finish()
        {
            if(this.CPU_usage == 100)
            {
                Console.WriteLine("Process" + this.Name + "is finished.");
            }
        }
        public void Running()
        {
            if (Status == "Running")
            {
                Console.WriteLine("Process" + this.Name + "is running.");
            }
        }
        public void MemoryAllocation(int m)
        {
            Memory = m;
        }

        public String[] Info()
        {
            String[] res = new String[5];
            res[0] = Name;
            res[1] = Pid.ToString();
            res[2] = Status;
            res[3] = CPU_usage.ToString();
            res[4] = Memory.ToString();
            
            return res;
        }
    }
}
