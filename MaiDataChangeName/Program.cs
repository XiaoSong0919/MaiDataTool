using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ConsoleApp1
{
    internal class Program
    {
        static int count = 0;
        static string Output_Path,Path,Classification;
        static List<string> Condition = new List<string>(); 
        static void Main(string[] args)
        {
            Console.WriteLine("###############MaiDataTool v1.0##############");
            Console.WriteLine("作者:LeZi");
            Console.WriteLine("#############################################");
            Console.WriteLine("[INFO]请输入分类文件夹所在目录：");
            Console.WriteLine("(譬如D:/xxxx/xxxx/Output-NoBGA/maimai，输入D:/xxxx/xxxx/Output-NoBGA即可)");
            Path = Get_Path();//获取路径
            Set_OutputPath();
            Set_Filter();
            DirectoryInfo directoryInfo = new DirectoryInfo(Path);
            //string[] dir_name = Directory.GetDirectories(Path);
            foreach(var dir in directoryInfo.GetDirectories())
            {
                string dir_name = dir.FullName.Replace(Path,"").Replace("\\",""); 
                Console.WriteLine($"[INFO]目前修改的分类:{dir_name}");
                if (!Directory.Exists($"{Output_Path}/{dir_name}"))
                    Directory.CreateDirectory($"{Output_Path}/{dir_name}");
                Change_Directory_Name(dir.Name.ToString());
            }
            Console.WriteLine($"[INFO]修改完毕，共修改乐曲:{count}首");
            Console.ReadKey();
            
        }
        static void Change_Directory_Name(string Class)
        {
            int c = 0;
            string[] dir_name = Directory.GetDirectories($"{Path}/{Class}");
            string Title = null,Shortid = null, Version = null, Genre = null, Bpm = null, Des = null, Cabinet = null;
            foreach(string dir in dir_name)
            {
                string path = dir;
                string[] fileline = File.ReadAllLines($"{path}/maidata.txt");
                foreach(string line in fileline)//获取歌曲名
                {
                    if(line.IndexOf("&title=", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        Title = line.Replace("&title=","").Replace(" ","");
                    }
                    if (line.IndexOf("&shortid=", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        Shortid = line.Replace("&shortid=", "");
                    }
                    if (line.IndexOf("&version=", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        Version = line.Replace("&version=", "");
                    }
                    if (line.IndexOf("&genre=", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        Genre = line.Replace("&genre=", "");
                    }
                    if (line.IndexOf("&wholebpm=", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        Bpm = line.Replace("&wholebpm=", "");
                    }
                    if (line.IndexOf("&des=", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        Des = line.Replace("&des=", "");
                    }
                    if (line.IndexOf("&cabinet=", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        Cabinet = line.Replace("&cabinet=", "");
                    }
                }
                try
                {
                    if (!Filter(Title, Shortid, Version, Genre, Bpm, Des, Cabinet))
                        continue;
                    if (Directory.Exists($"{Output_Path}/{Class}/[{Shortid}]{Title}"))
                    {
                        Directory.Move(path, $"{Output_Path}/{Class}/[{Shortid}]{Title}_{c++}");
                    }
                    else
                    {
                        Directory.Move(path,$"{Output_Path}/{Class}/[{Shortid}]{Title}");
                    }

                }
                catch
                {
                    Console.WriteLine($"[ERROR]发生错误，改用ID命名\n歌曲名称：[{Shortid }]{Title}");
                    if (Directory.Exists($"{Output_Path}/{Class}/[{Shortid}]"))
                    {
                        Directory.Move(path, $"{Output_Path}/{Class}/[{Shortid}]_{c++}");
                    }
                    else
                    {
                        Directory.Move(path, $"{Output_Path}/{Class}/[{Shortid}]");
                    }
                }
                
                Console.WriteLine($"已修改乐曲：[{Shortid}]{Title}");
                Console.WriteLine("[INFO]修改完毕，祝您收歌愉快XD");
                count++;
            }

        }
        static bool Filter(string Title, string Shortid, string Version, string Genre, string Bpm, string Des, string Cabinet)
        {
            if (Condition.Count == 0)//不存在筛选条件
                return true;
            List<int> list = new List<int>();
            List<int> Version_list = new List<int>();
            List<int> Bpm_list = new List<int>();
            List<int> Des_list = new List<int>();
            List<int> Cabinet_list = new List<int>();
            List<int> Title_list = new List<int>();
            List<int> Shortid_list = new List<int>();
            List<int> Genre_list = new List<int>();
            foreach (var s in Condition)
            {
                if (s.Split(";")[0] == "Title")
                    if (s.Split(";")[1] != Title)
                        Title_list.Add(0);
                else
                        Title_list.Add(1);
                if (s.Split(";")[0] == "Shortid")
                    if (s.Split(";")[1] != Shortid)
                        Shortid_list.Add(0);
                else
                        Shortid_list.Add(1);
                if (s.Split(";")[0] == "Version")
                    if (s.Split(";")[1] != Version)
                        Version_list.Add(0);
                else
                        Version_list.Add(1);
                if (s.Split(";")[0] == "Genre")
                    if (s.Split(";")[1] != Genre)
                        Genre_list.Add(0);
                else
                        Genre_list.Add(1);
                if (s.Split(";")[0] == "Bpm")
                    if (s.Split(";")[1] != Bpm)
                        Bpm_list.Add(0);
                else
                        Bpm_list.Add(1);
                if (s.Split(";")[0] == "Des")
                    if (s.Split(";")[1] != Des)
                        Des_list.Add(0);
                else
                        Des_list.Add(1);
                if (s.Split(";")[0] == "Cabinet")
                    if (s.Split(";")[1] != Cabinet)
                        Cabinet_list.Add(0);
                else
                        Cabinet_list.Add(1);
            }
            if (Version_list.Count != 0)
                if (!Version_list.Contains(1))
                    list.Add(0);
            if (Bpm_list.Count != 0)
                if (!Bpm_list.Contains(1))
                    list.Add(0);
            if (Des_list.Count != 0)
                if (!Des_list.Contains(1))
                    list.Add(0);
            if (Cabinet_list.Count != 0)
                if (!Cabinet_list.Contains(1))
                    list.Add(0);
            if (Title_list.Count != 0)
                if (!Title_list.Contains(1))
                    list.Add(0);
            if (Shortid_list.Count != 0)
                if (!Shortid_list.Contains(1))
                    list.Add(0);
            if (Genre_list.Count != 0)
                if (!Genre_list.Contains(1))
                    list.Add(0);

            if (list.Contains(0))
                return false;
            else
                return true;
        }
        static string Get_Path()
        {
            string PATH = null;
            PATH = Console.ReadLine();
            while (!Directory.Exists(PATH))
            {
                Console.WriteLine("[ERROR]目录"+ PATH +"不存在！");
                Console.Write("请输入谱面文件所在目录：");
                PATH = Console.ReadLine();
            }
            Console.WriteLine("[INFO]当前工作目录：" + PATH);
            DirectoryInfo directoryInfo = new DirectoryInfo(PATH);
            Console.WriteLine("[INFO]工作目录下存在以下分类:");
            foreach (var s in directoryInfo.GetDirectories())
                Console.WriteLine(s.FullName.Replace(PATH,"").Replace("\\",""));
            Console.WriteLine("[INFO]扫描完毕");
            return PATH;
        }
        static void Set_OutputPath()//设置输出路径
        {
            Console.WriteLine("[INFO]请输入输出路径(直接修改请回车):");
            Output_Path = Console.ReadLine();
            if (Output_Path == "")
                Output_Path = Path;
        }
        static void Set_Filter()//设置筛选条件
        {
            List<string> list = new List<string>();
            Chose:
            Console.WriteLine("0.标题");
            Console.WriteLine("1.歌曲ID");
            Console.WriteLine("2.版本");
            Console.WriteLine("3.分类");
            Console.WriteLine("4.BPM");
            Console.WriteLine("5.谱师");
            Console.WriteLine("6.谱面版本(SD/DX)");
            Console.WriteLine("请输入想设置的筛选条件(回车以跳过筛选):");
            var Input = Console.ReadLine();
            if(Input == "0")
            {

                Console.WriteLine("请输入歌曲标题:");
                Condition.Add($"Title;{Console.ReadLine()}");
                list.Add("0");
                goto Chose;
            }
            if (Input == "1")
            {

                Console.WriteLine("请输入歌曲ID:");
                Condition.Add($"Shortid;{Console.ReadLine()}");
                list.Add("1");
                goto Chose;
            }
            if (Input == "2")
            {

                Console.WriteLine("maimai");
                Console.WriteLine("maimai PLUS");
                Console.WriteLine("maimai GreeN");
                Console.WriteLine("maimai GreeN PLUS");
                Console.WriteLine("maimai ORANGE");
                Console.WriteLine("maimai ORANGE PLUS");
                Console.WriteLine("maimai PiNK");
                Console.WriteLine("maimai PiNK PLUS");
                Console.WriteLine("maimai MURASAKi");
                Console.WriteLine("maimai MURASAKi PLUS");
                Console.WriteLine("maimai MiLK");
                Console.WriteLine("maimai MiLK PLUS");
                Console.WriteLine("maimai FiNALE");
                Console.WriteLine("maimai DX");
                Console.WriteLine("maimai DX PLUS");
                Console.WriteLine("maimai DX Splash");
                Console.WriteLine("maimai DX Splash PLUS");
                Console.WriteLine("maimai DX UNiVERSE");
                Console.WriteLine("maimai DX UNiVERSE PLUS");
                Console.WriteLine("请输入歌曲版本(复制粘贴):");
                Condition.Add($"Version;{Console.ReadLine()}");
                list.Add("2");
                goto Chose;
            }
            if (Input == "3")
            {

                Console.WriteLine("maimai");
                Console.WriteLine("niconicoボーカロイド");
                Console.WriteLine("POPSアニメ");
                Console.WriteLine("オンゲキCHUNITHM");
                Console.WriteLine("ゲームバラエティ");
                Console.WriteLine("東方Project");
                Console.WriteLine("请输入歌曲分类(请直接复制粘贴):");
                Condition.Add($"Genre;{Console.ReadLine()}");
                list.Add("3");
                goto Chose;
            }
            if (Input == "4")
            {

                Console.WriteLine("请输入歌曲BPM:");
                Condition.Add($"Bpm;{Console.ReadLine()}");
                list.Add("4");
                goto Chose;
            }
            if (Input == "5")
            {

                Console.WriteLine("请输入谱师昵称:");
                Condition.Add($"Des;{Console.ReadLine()}");
                list.Add("5");
                goto Chose;
            }
            if (Input == "6")
            {
                Console.WriteLine("请输入谱面版本(SD/DX):");
                Condition.Add($"Cabinet;{Console.ReadLine()}");
                list.Add("6");
                goto Chose;
            }
        }
    }
}
