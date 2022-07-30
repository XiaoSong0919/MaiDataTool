using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Runtime.InteropServices;

namespace MaiDataChangeName
{
    static internal class main
    {
        static int count = 0;
        static string Output_Path,bitRate;
        static bool AudioReCoding = false;
        public static double Total_Count, Finished_Count;
        static ConsoleColor Default = Console.ForegroundColor;
        static List<string> Rule_List = new List<string>();//规则列表
        static List<Target> Target_List = new();//符合规则的谱面列表
        static List<Target> List = new();
        static string version = "1.2.1";
        public struct Target
        {
            public string Path;
            public string Title;
            public string Shortid;
            public string Version;
            public string Genre;
            public string Bpm;
            public string Des;
            public string Cabinet;

        }
        static string Environment
        {
            get
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    return "Linux";
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    return "Windows";
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    return "OSX";
                else if (!RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
                    return "FreeBSD";
                else
                    return "Unknown";                     
            }
        }
        static void Logo()
        {
            Console.WriteLine($"############### MaiDataTool v{version} ##############");
            Console.WriteLine("作者:LeZi");
            Console.WriteLine("框架: .Net Core 6.0");
            Console.WriteLine($"运行环境: {Environment}");
            Console.WriteLine("##############################################\n");
        }
        public static void Main(string[] args)
        {
            Console.Clear();
            Rule_List.Clear();
            Target_List.Clear();
            Logo();
            Console.WriteLine("欢迎使用MaiDataTool!");
            Console.WriteLine("[INFO]请输入目录:");
            List = FileManage.Search(Console.ReadLine());
            ReloadList();
            Set_OutputPath();
            Set_Rule();
            Set_ReCodingMusic();
            Confirm();
            Console.WriteLine("[INFO]正在修改，请稍后...");
            List<Task> task_list = new();            
            Total_Count = Target_List.Count;
            foreach (var path in Target_List)
            {
                Task task = new(() => Change_Directory_Name(path));
                task_list.Add(task);
                task.Start();
            }
            Task.WaitAll(task_list.ToArray());
            Console.WriteLine($"[INFO]修改完毕，共修改乐曲:{count}首，祝您收歌愉快XD");
            Console.ReadKey();
            
        }
        static void Confirm()
        {
            ReConfirm:
            Console.Clear();
            Logo();
            ReloadList();
            Console.WriteLine("要修改的谱面:");
            foreach (var target in Target_List)
                Console.WriteLine(target.Path);
            Console.WriteLine($"符合规则的共有{Target_List.Count}个谱面\n-------------------------------------------");
            Console.WriteLine("规则列表:\n");
            foreach(var rule in Rule_List)
            {
                Console.WriteLine($"规则{Rule_List.BinarySearch(rule)}");
                var type = rule.Split(";")[0];
                var data = rule.Split(";")[1];
                if (type == "Title")
                    Console.WriteLine($"歌曲名称:{data}  \n");
                else if (type == "Shortid")
                    Console.WriteLine($"歌曲ID:{data}  \n");
                else if (type == "Version")
                    Console.WriteLine($"歌曲发行版本:{data} \n");
                else if (type == "Genre")
                    Console.WriteLine($"歌曲类别:{data} \n");
                else if (type == "Bpm")
                    Console.WriteLine($"歌曲BPM:{data} \n");
                else if (type == "Des")
                    Console.WriteLine($"谱师:{data}\n");
                else if (type == "Cabinet")
                    Console.WriteLine($"歌曲版本:{data}\n");              
            }
            Console.WriteLine("-------------------------------------------\n");
            Console.WriteLine($"音频重编码:{AudioReCoding}\n音频码率:{bitRate}Kbps\n多线程编码: Enable");
            Console.WriteLine("-------------------------------------------\n");
            Console.WriteLine("");
            Console.WriteLine("确认以上信息吗?(Y/N)");
            var input = Console.ReadLine();
            if (input != "Y" && input != "y")
            {
                ReSelect:
                Console.WriteLine("请输入你想进行的操作:");
                Console.WriteLine("1.编辑规则");
                Console.WriteLine("2.返回");
                Console.WriteLine("3.退出程序");
                var ReChoose = Console.ReadLine();
                if (ReChoose == "1")
                    Edit_Rule();
                else if (ReChoose == "2")
                    goto ReConfirm;
                else if (ReChoose == "3")
                    System.Environment.Exit(0); 
                else
                {
                    Console.WriteLine("[ERROR]我不是AI，不知道你在想啥");
                    Console.ReadKey();
                    goto ReSelect;
                }
            }
        }
        static void Change_Directory_Name(Target path)
        {
            int c = 0;
            try
            {
                if (Directory.Exists($"{Output_Path}/[{path.Shortid}]{path.Title}"))
                {
                    Directory.Move(path.Path, $"{Output_Path}/[{path.Shortid}]{path.Title}_{c++}");
                    if (AudioReCoding)
                        Audio.ReCoding.MP3($"{Output_Path}/[{path.Shortid}]{path.Title}_{c - 1}", Convert.ToInt32(bitRate));
                }
                else
                {
                    Directory.Move(path.Path, $"{Output_Path}/[{path.Shortid}]{path.Title}");
                    if (AudioReCoding)
                        Audio.ReCoding.MP3($"{Output_Path}/[{path.Shortid}]{path.Title}", Convert.ToInt32(bitRate));
                }
            }
            catch (IOException e)
            {
                Console.WriteLine($"[Thread{Thread.CurrentThread.ManagedThreadId}][ERROR]发生错误，改用ID命名\n歌曲名称：[{path.Shortid}]{path.Title}");
                if (Directory.Exists($"{Output_Path}/[{path.Shortid}]"))
                {
                    Directory.Move(path.Path, $"{Output_Path}/[{path.Shortid}]_{c++}");
                    if (AudioReCoding)
                        Audio.ReCoding.MP3($"{Output_Path}/[{path.Shortid}]_{c - 1}", Convert.ToInt32(bitRate));
                }
                else
                {
                    Directory.Move(path.Path, $"{Output_Path}/[{path.Shortid}]");
                    if (AudioReCoding)
                        Audio.ReCoding.MP3($"{Output_Path}/[{path.Shortid}]", Convert.ToInt32(bitRate));
                }
            }
            Finished_Count++;
            Console.WriteLine($"[Thread{Thread.CurrentThread.ManagedThreadId}]已修改乐曲：[{path.Shortid}]{path.Title}");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[INFO]已完成百分比:{Math.Round(Finished_Count / Total_Count, 2) * 100}%");
            Console.ForegroundColor = Default;
            count++;
            //}
        }
        static bool Filter(string Title, string Shortid, string Version, string Genre, string Bpm, string Des, string Cabinet)
        {
            if (Rule_List.Count == 0)//不存在筛选条件
                return true;
            List<int> list = new List<int>();
            List<int> Version_list = new List<int>();
            List<int> Bpm_list = new List<int>();
            List<int> Des_list = new List<int>();
            List<int> Cabinet_list = new List<int>();
            List<int> Title_list = new List<int>();
            List<int> Shortid_list = new List<int>();
            List<int> Genre_list = new List<int>();
            foreach (var s in Rule_List)
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
        static void Set_OutputPath()//设置输出路径
        {
            Console.Clear();
            Logo();
            Console.WriteLine("[INFO]请输入输出路径:");
            var Input = Console.ReadLine();
            if (Directory.Exists(Input))
                Output_Path = Input;
            else
            {
                try
                {
                    Directory.CreateDirectory(Input);
                    Output_Path = Input;
                }
                catch
                {
                    Console.WriteLine("[ERROR]输出路径有误，请重新输入");
                    Console.ReadKey();
                    Set_OutputPath();
                }
            }             
        }
        static void Set_Rule()//设置筛选条件
        {
            Chose:
            Console.Clear();
            Logo();
            ReloadList();
            Console.WriteLine($"符合规则的共有{Target_List.Count}个谱面\n-------------------------------------------");
            Console.WriteLine("[INFO]请输入想设置的筛选条件(回车以跳过筛选):");
            Console.WriteLine("0.标题");
            Console.WriteLine("1.歌曲ID");
            Console.WriteLine("2.版本");
            Console.WriteLine("3.分类");
            Console.WriteLine("4.BPM");
            Console.WriteLine("5.谱师");
            Console.WriteLine("6.谱面版本(SD/DX)");
            var Input = Console.ReadLine();
            if(Input == "0")
            {

                Console.WriteLine("请输入歌曲标题:");
                Rule_List.Add($"Title;{Console.ReadLine()}");
                goto Chose;
            }
            if (Input == "1")
            {

                Console.WriteLine("请输入歌曲ID:");
                Rule_List.Add($"Shortid;{Console.ReadLine()}");
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
                Rule_List.Add($"Version;{Console.ReadLine()}");
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
                Rule_List.Add($"Genre;{Console.ReadLine()}");
                goto Chose;
            }
            if (Input == "4")
            {

                Console.WriteLine("请输入歌曲BPM:");
                Rule_List.Add($"Bpm;{Console.ReadLine()}");
                goto Chose;
            }
            if (Input == "5")
            {

                Console.WriteLine("请输入谱师昵称:");
                Rule_List.Add($"Des;{Console.ReadLine()}");
                goto Chose;
            }
            if (Input == "6")
            {
                Console.WriteLine("请输入谱面版本(SD/DX):");
                Rule_List.Add($"Cabinet;{Console.ReadLine()}");
                goto Chose;
            }
        }
        static void Set_ReCodingMusic()//音频文件重编码
        {
            Console.Clear();
            Logo();
            Console.WriteLine("[INFO]是否要对音频文件进行重新编码?(Y/N):");
            Console.WriteLine("Tip:这可以解决一些写谱软件(如Majdata)音频无法对齐的问题");
            var Input = Console.ReadLine();
            if (Input == "Y" || Input == "y")
            {
                Console.Clear();
                Logo();
                Console.WriteLine("MP3标准比特率如下:");
                Console.WriteLine("320Kbps\n192Kbps\n128Kbps");
                Console.WriteLine("请输入比特率(Kbps):");
                bitRate = Console.ReadLine();
                AudioReCoding = true;
            }
        }
        static void ReloadList()//刷新Target_List
        {
            Target_List.Clear();
            foreach (var item in List)
            {
                if (Filter(item.Title, item.Shortid, item.Version, item.Genre, item.Bpm, item.Des, item.Cabinet))
                    Target_List.Add(item);
            }
        }
        static void Edit_Rule()
        {
            Console.Clear();
            Logo();
            Console.WriteLine("你想对现有的规则做什么？");
            Console.WriteLine("1.添加规则");
            Console.WriteLine("2.删除规则");
            var Input = Console.ReadLine();
            if (Input == "1")
                Set_Rule();
            else if (Input == "2")
            {
                ReDelete:
                Console.Clear();
                Logo();
                ReloadList();
                Console.WriteLine("规则列表:\n");
                foreach (var rule in Rule_List)
                {
                    Console.WriteLine($"符合规则的共有{Target_List.Count}个谱面\n-------------------------------------------");
                    Console.WriteLine($"规则{Rule_List.BinarySearch(rule)}");
                    var type = rule.Split(";")[0];
                    var data = rule.Split(";")[1];
                    if (type == "Title")
                        Console.WriteLine($"歌曲名称:{data}  \n");
                    else if (type == "Shortid")
                        Console.WriteLine($"歌曲ID:{data}  \n");
                    else if (type == "Version")
                        Console.WriteLine($"歌曲发行版本:{data} \n");
                    else if (type == "Genre")
                        Console.WriteLine($"歌曲类别:{data} \n");
                    else if (type == "Bpm")
                        Console.WriteLine($"歌曲BPM:{data} \n");
                    else if (type == "Des")
                        Console.WriteLine($"谱师:{data}\n");
                    else if (type == "Cabinet")
                        Console.WriteLine($"歌曲版本:{data}\n");
                }
                Console.WriteLine("请输入你想删除的规则(序号):");
                ReInput:
                int number;
                if (Int32.TryParse(Console.ReadLine(), out number))
                {
                    Rule_List.RemoveAt(number);
                    Console.WriteLine($"[INFO]已成功删除规则{number}");
                    Console.WriteLine($"是否继续删除？(Y/N)");
                    var input = Console.ReadLine();
                    if (input == "Y" || input == "y")
                        goto ReDelete;
                    else
                        Confirm();
                }
                else
                {
                    Console.WriteLine("[ERROR]无效输入");
                    Console.ReadKey();
                    goto ReInput;
                }

            }
            else
            {
                Console.WriteLine("[ERROR]我不是AI，不知道你在想啥");
                Console.ReadKey();
                Edit_Rule();
            }
        }
    }
}
