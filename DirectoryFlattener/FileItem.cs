using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DirectoryFlattener
{
    class FileItem : IItem
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public string BasePath { get; set; }

        public string GetFullDirectoryPath()
        {
            return string.Format("{0}", BasePath);
        }
    }
}
