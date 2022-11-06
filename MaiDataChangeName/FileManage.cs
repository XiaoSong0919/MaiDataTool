using System;
using System.Collections.Generic;
using System.IO;
using static MaiDataTool.main;

namespace MaiDataTool
{
    internal static class FileManage //文件搜索
    {
        static List<Target> Directory_List = new();
        static int Scan_Count;
        public static List<Target> Search(string Path)
        {
            if(!Directory.Exists(Path))
            {
                Console.WriteLine("[ERROR]文件夹不存在，请重新输入");
                Console.ReadKey();
                main.Main(null);
            }
            Directory_List.Clear();
            Scan_Count = 0;
            DirectoryInfo directoryInfo = new(Path);
            Recursion(directoryInfo);
            Console.WriteLine($"[INFO]共搜索到{Directory_List.Count}个谱面文件夹");
            Console.ReadKey();            
            return Set_TargetInfo(Directory_List);
        }
        static void Recursion(DirectoryInfo DirInfo)
        {
            var SubDirList = DirInfo.GetDirectories();
            foreach (var SubDir in SubDirList)
            {
                Target target = new();
                var Path = SubDir.ToString();
                DirectoryInfo SubDirInfo = new(Path) ;
                var list = SubDirInfo.GetDirectories();
                Scan_Count++;
                if (list.Length == 0 && (File.Exists($"{Path}/maidata.txt") || File.Exists($"{Path}/MaiData.txt")) && File.Exists($"{Path}/track.mp3"))
                {
                    target.Path = Path;
                    Directory_List.Add(target);
                    Console.WriteLine($"[INFO]发现谱面文件夹:{Path}");
                }
                else
                {
                    Console.WriteLine($"[INFO]已扫描{Scan_Count}个文件夹");
                    Recursion(SubDirInfo);
                }
            }
        }
        static List<Target> Set_TargetInfo(List <Target> Target_list)
        {
            List<Target> list = new List<Target>();
            foreach(var Target in Target_list)
            {
                var target = Target;
                string[] fileline = File.ReadAllLines($"{target.Path}/maidata.txt");
                foreach (string line in fileline)//获取歌曲名
                {
                    if (line.Contains("&title"))
                    {
                        target.Title = line.Replace("&title=", "").Replace(" ", "");
                    }
                    if (line.Contains("&shortid"))
                    {
                        target.Shortid = line.Replace("&shortid=", "");
                    }
                    if (line.Contains("&version"))
                    {
                        target.Version = line.Replace("&version=", "");
                    }
                    if (line.Contains("&genre"))
                    {
                        target.Genre = line.Replace("&genre=", "");
                    }
                    if (line.Contains("&wholebpm"))
                    {
                        target.Bpm = line.Replace("&wholebpm=", "");
                    }
                    if (line.Contains("&des"))
                    {
                        target.Des = line.Replace("&des=", "");
                    }
                    if (line.Contains("&cabinate"))
                    {
                        target.Cabinet = line.Replace("&cabinate=", "");
                    }
                }
                list.Add(target);
            }
            return list;
        }
    }
}
