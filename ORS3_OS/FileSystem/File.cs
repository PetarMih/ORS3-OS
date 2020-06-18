using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace ORS3_OS.FileSystem
{
    class File
    {
        //public string Ime { get; set; }
        public string Path { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public double Size { get; set; }
        public object Content { get; set; }
        public Folder ParentFolder { get; set; }
        
        public File()
        {
        }

        public File(Folder parent, bool isFolder, string name)
        {
            if (parent is null)
            {
                Path = name + (isFolder ? "\\" : "");
            }
            else
            {
                Path = parent.Path + name + (isFolder ? "\\" : "");
            }
            Created = DateTime.Now;
            Modified = DateTime.Now;
            ParentFolder = parent;
        }

        public File(Folder parent, string name, bool isFolder, byte[] content)
        {
            if (parent is null)
            {
                Path = name + (isFolder?"\\":"");
            }
            else
            {
                Path = parent.Path + name + (isFolder ? "\\" : "");
            }
            Created = DateTime.Now;
            Modified = DateTime.Now;
            Content = content;
            Size = content.Length;
            ParentFolder = parent;
        }

        public override bool Equals(object obj)
        {
            return obj is File file &&
                   Path == file.Path;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Path);
        }
    }
}
