using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ORS3_OS.FileSystem
{
    class FileSystem
    {
        public Folder Root { get; set; }

        public Folder Current { get; set; }

        public FileSystem()
        {
            Root = Folder.AddFolder(null, "");
            Current = Root;
        }

        public bool AddFolder(string name) => Folder.AddFolder(Current, name)!=null;
        public bool AddFile(string name) => Folder.AddFile(Current, name) != null;

        public bool ChangeCurrentFolder (string name)
        {
            if (name == "..")
            {
                Current = Current.ParentFolder;
            }
            else
            {
                Folder newFolder = (Current.Content as HashSet<File>)
                    .Where(elem => elem is Folder)
                    .FirstOrDefault(elem => System.IO.Path.GetDirectoryName(elem.Path).EndsWith("\\"+name)) as Folder;
                if(newFolder is null)
                {
                    return false;
                }
                else
                {
                    Current = newFolder;
                    return true;
                }
            }
            return true;
        }

        public void List() => Console.WriteLine(Current);

    }
}
