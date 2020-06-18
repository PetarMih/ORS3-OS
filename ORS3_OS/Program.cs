using FS = ORS3_OS.FileSystem.FileSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ORS3_OS.FileSystem;

namespace ORS3_OS
{
    class Program
    {
        static void Main(string[] args)
        {
            FS fs = new FS();
            for (int i = 0; i < 5; i++)
            {
                fs.AddFolder("Folder" + i);
                if (!fs.ChangeCurrentFolder("Folder"+i))
                    Console.WriteLine("Nepostojeci folder");
                fs.AddFolder("Folder" + i+i);
                if (!fs.ChangeCurrentFolder(".."))
                    Console.WriteLine("Nepostojeci folder");
            }
            if (!fs.ChangeCurrentFolder("Folder0"))
                Console.WriteLine("Nepostojeci folder");
            fs.AddFolder("folder11");
            if (!fs.ChangeCurrentFolder("folder11"))
                Console.WriteLine("Nepostojeci folder");
            fs.AddFolder("folder111");
            fs.AddFile("test.txt");
            if (!fs.ChangeCurrentFolder(".."))
                Console.WriteLine("Nepostojeci folder");
            if (!fs.ChangeCurrentFolder(".."))
                Console.WriteLine("Nepostojeci folder");
            fs.List();
        }
    }
}
