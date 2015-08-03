using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace DirectoryFlattener
{
    class DirectoryItem : IItem
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public string BasePath { get; set; }

        public string GetFullDirectoryPath()
        {
            return string.Format("{0}{1}{2}", BasePath, System.IO.Path.DirectorySeparatorChar, Name);
        }
    }
}
