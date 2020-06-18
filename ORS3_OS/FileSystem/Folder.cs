using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ORS3_OS.FileSystem
{
    class Folder : File
    {

        private Folder(Folder parent, string name) : base(parent, name, true, new byte[0])
        {
            Content = new HashSet<File>();
        }

        public override string ToString()
        {
            return PrintFolderTree(0);
        }

        public static Folder AddFolder(Folder parent, string name)
        {
            if (parent != null)
            {
                HashSet<File> subfolders = parent.Content as HashSet<File>;
                Folder folder = new Folder(parent, name);
                return subfolders.Add(folder) ? folder : null;
            }
            else
            {
                return new Folder(parent, name);
            }
        }

        public static File AddFile(Folder current, string name)
        {
            if (current != null)
            {
                HashSet<File> subfolders = current.Content as HashSet<File>;
                File file = new File(current,false, name);
                return subfolders.Add(file) ? file : null;
            }
            else
            {
                return new Folder(current, name);
            }
        }

        public string PrintFolderTree(int padding)
        {
            string result = "";

            for (int i = 0; i < padding; i++) result += "\t";
            result += System.IO.Path.GetFileName(Path) + "\n";

            foreach (File file in (Content as HashSet<File>))
            {
                if (file is Folder folder)
                {
                    for (int i = 0; i < padding; i++) result += "\t";
                    string name = System.IO.Path.GetFileName(file.Path.Substring(0, file.Path.Length - 1));
                    result += name + "\n";
                    result += folder.PrintFolderTree(padding + 1);
                }
                else
                {
                    for (int i = 0; i < padding; i++) result += "\t";
                    result += System.IO.Path.GetFileName(file.Path) + "\n";
                }
            }
            return result;
        }
    }
}
