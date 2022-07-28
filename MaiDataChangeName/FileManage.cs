using System;
using System.Collections.Generic;
using System.IO;

namespace MaiDataChangeName
{
    internal static class FileManage //文件搜索
    {
        static List<string> Directory_List = new();
        static int Scan_Count;
        static List<string> GetDirectoryList
        {
            get { return Directory_List; }
        }
        static int GetScanCount
        {
            get { return Scan_Count; }
        }
        public static List<string> Search(string Path)
        {
            Directory_List.Clear();
            Scan_Count = 0;
            DirectoryInfo directoryInfo = new(Path);
            Recursion(directoryInfo);
            Console.WriteLine($"[INFO]共搜索到{Directory_List.Count}个谱面文件夹");
            Console.ReadKey();
            return Directory_List;
        }
        static void Recursion(DirectoryInfo DirInfo)
        {
            var SubDirList = DirInfo.GetDirectories();
            foreach (var SubDir in SubDirList)
            {
                var Path = SubDir.ToString();
                DirectoryInfo SubDirInfo = new(Path) ;
                var list = SubDirInfo.GetDirectories();
                Scan_Count++;
                if (list.Length == 0 && (File.Exists($"{Path}/maidata.txt") || File.Exists($"{Path}/MaiData.txt")) && File.Exists($"{Path}/track.mp3"))
                {
                    Directory_List.Add(Path);
                    Console.WriteLine($"[INFO]发现谱面文件夹:{Path}");
                }
                else
                {
                    Console.WriteLine($"[INFO]已扫描{Scan_Count}个文件夹");
                    Recursion(SubDirInfo);
                }
            }
        }
    }
}
