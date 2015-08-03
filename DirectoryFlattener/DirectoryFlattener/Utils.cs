using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace DirectoryFlattener
{
    class Utils
    {
        public static string GetDirectoryName(string fullPath)
        {
            string directoryName = string.Empty;
            int index = fullPath.LastIndexOf(Path.DirectorySeparatorChar);
            if (index < 0)
            {
                directoryName = string.Empty;
            }
            else
            {
                directoryName = fullPath.Substring(0, index);
            }
            return directoryName;
        }

        public static string GetFileName(string fullPath)
        {
            string directoryName = string.Empty;
            int index = fullPath.LastIndexOf(Path.DirectorySeparatorChar);
            if (index < 0)
            {
                directoryName = fullPath;
            }
            else
            {
                index++;
                directoryName = fullPath.Substring(index, fullPath.Length - index);
            }
            return directoryName;
        }
    }
}
