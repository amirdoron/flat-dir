using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DirectoryFlattener
{
    interface IItem
    {
        string Name { get; set; }
        string Id { get; set; }
        string BasePath { get; set; }
        string GetFullDirectoryPath();
    }
}
