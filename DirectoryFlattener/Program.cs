using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Delimon.Win32.IO;

namespace DirectoryFlattener
{
    class Program
    {
        static int i = 0;

        static string resultTextFileName = "inst";

        static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("Error,Invalid Parameters");
            }
            else
            {
                string method = args[0];
                string sourceDirectoryPath = args[1];
                string resultDirectoryPath = args[2];
                if (sourceDirectoryPath.Trim() == resultDirectoryPath.Trim())
                {
                    Console.WriteLine("Error,baseDirectoryPath cannot be the same as resultDirectoryPath");
                }
                else
                {
                    if (!Directory.Exists(sourceDirectoryPath))
                    {
                        Console.WriteLine("Error, Source Directory does not exist");
                    }
                    else
                    {
                        if (method.Trim() == "f")
                        {
                            resultDirectoryPath = FlattenDirectory(sourceDirectoryPath, resultDirectoryPath);
                        }
                        else if (method.Trim() == "uf")
                        {
                            resultDirectoryPath = UnFlattenDirectory(sourceDirectoryPath, resultDirectoryPath);
                        }
                    }
                }
            }
        }

        private static string FlattenDirectory(string baseDirectoryPath, string resultDirectoryPath)
        {
            string realBaseDirectoryPath = Utils.GetDirectoryName(baseDirectoryPath);
            if (!Directory.Exists(resultDirectoryPath))
            {
                DirectoryInfo info = new DirectoryInfo(resultDirectoryPath);
                info.Create();
            }
            resultDirectoryPath = resultDirectoryPath + System.IO.Path.DirectorySeparatorChar + DateTime.Now.ToString("ddMMyyhhmmss");
            DirectoryInfo info1 = new DirectoryInfo(resultDirectoryPath);
            info1.Create();
            Queue<IItem> queue = new Queue<IItem>();
            string currentPath = baseDirectoryPath;
            DirectoryItem item1 = new DirectoryItem()
            {
                BasePath = Utils.GetDirectoryName(currentPath),
                Id = GetNextId(),
                Name = Path.GetFileName(baseDirectoryPath),
            };
            List<IItem> itemsList = new List<IItem>();
            queue.Enqueue(item1 as IItem);

            string resultTextFilePath = resultDirectoryPath + System.IO.Path.DirectorySeparatorChar + resultTextFileName;
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(resultTextFilePath))
            {
                while (queue.Count > 0)
                {
                    IItem item = queue.Dequeue();
                    itemsList.Add(item);
                    if (item is DirectoryItem)
                    {
                        file.WriteLine(string.Format("d,{0},{1},{2}", item.Id, RemoveBasePathFromPath(item.BasePath, realBaseDirectoryPath), item.Name));
                        var directories = Directory.GetDirectories(item.GetFullDirectoryPath());
                        foreach (string directory in directories)
                        {
                            DirectoryItem newItem = new DirectoryItem()
                            {
                                BasePath = Utils.GetDirectoryName(directory),
                                Id = GetNextId(),
                                Name = Utils.GetFileName(directory),
                            };
                            queue.Enqueue(newItem as IItem);
                        }
                        var fileNames = Directory.GetFiles(item.GetFullDirectoryPath());
                        foreach (string fileName in fileNames)
                        {
                            FileItem newItem = new FileItem()
                            {
                                BasePath = Utils.GetDirectoryName(fileName),
                                Id = GetNextId(),
                                Name = Utils.GetFileName(fileName),
                            };
                            queue.Enqueue(newItem as IItem);
                        }
                    }
                    else
                    {
                        string sourceFilePath = item.BasePath + System.IO.Path.DirectorySeparatorChar + item.Name; ;
                        string resultFilePath = resultDirectoryPath + System.IO.Path.DirectorySeparatorChar + item.Id;
                        //copy file to result directory
                        //File.Copy(sourceFilePath, resultFilePath);
                        File.Copy(sourceFilePath, resultFilePath,true);
                        file.WriteLine(string.Format("f,{0},{1},{2}", item.Id, RemoveBasePathFromPath(item.BasePath, realBaseDirectoryPath), item.Name));
                    }

                }
                file.Flush();
            }
            return resultDirectoryPath;
        }

        public static string UnFlattenDirectory(string sourceDirectoryPath, string resultDirectoryPath)
        {
            if (!Directory.Exists(resultDirectoryPath))
            {
                DirectoryInfo info = new DirectoryInfo(resultDirectoryPath);
                info.Create();
            }
            resultDirectoryPath = resultDirectoryPath + System.IO.Path.DirectorySeparatorChar + DateTime.Now.ToString("ddMMyyhhmmss");
            DirectoryInfo info1 = new DirectoryInfo(resultDirectoryPath);
            info1.Create();

            string InstructionFilePath = sourceDirectoryPath + System.IO.Path.DirectorySeparatorChar + resultTextFileName;
            int i = 0;
            using (System.IO.StreamReader file = new System.IO.StreamReader(InstructionFilePath))
            {
                while (!file.EndOfStream)
                {
                    i++;
                    string line = file.ReadLine();
                    string[] lineSplitted = line.Split(',');
                    if (lineSplitted.Length != 4)
                    {
                        Console.WriteLine("Error,Instruction file is in bad Format, {0}", i);
                    }
                    else
                    {
                        string type = lineSplitted[0].Trim();
                        string id = lineSplitted[1].Trim();
                        string basePath = lineSplitted[2].Trim();
                        string fileName = lineSplitted[3].Trim();
                        if (type == "d")
                        {
                            string newDirectoryPath = resultDirectoryPath;
                            if(!string.IsNullOrEmpty(basePath))
                            {
                                newDirectoryPath += basePath;
                            }
                            newDirectoryPath += System.IO.Path.DirectorySeparatorChar + fileName;
                            DirectoryInfo dInfo = new DirectoryInfo(newDirectoryPath);
                            dInfo.Create();
                        }
                        if (type == "f")
                        {
                            string sourceFilePath = sourceDirectoryPath + System.IO.Path.DirectorySeparatorChar + id;
                            string resultFilePath = resultDirectoryPath + basePath + System.IO.Path.DirectorySeparatorChar + fileName;
                            if (!File.Exists(sourceFilePath))
                            {
                                Console.WriteLine(string.Format("Error, File {0} is not found on source directory", sourceFilePath));
                            }
                            else
                            {
                                File.Copy(sourceFilePath, resultFilePath, true);
                            }

                        }
                    }
                }
            }


            return resultDirectoryPath;
        }

        public static string GetNextId()
        {
            return (++i).ToString();
        }

        public static string RemoveBasePathFromPath(string fullPath, string basePath)
        {
            int index = fullPath.IndexOf(basePath);
            if (index < 0)
            {
                throw new Exception(string.Format("cannot remove base path,{0} not found in {1}", basePath, fullPath));
            }
            if (fullPath[index] == System.IO.Path.DirectorySeparatorChar)
            {
                index++;
            }
            index = index + basePath.Length;
            string newPath = fullPath.Substring(index, fullPath.Length - index);
            return newPath;
        }


    }
}
