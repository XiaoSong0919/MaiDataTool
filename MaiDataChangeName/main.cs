using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Runtime.InteropServices;

namespace MaiDataTool
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
        static Rule rule = new();
        static string version = "1.2.1";
        public enum NoteType
        {
            OnlyHold,
            OnlyStar,
            OnlyTap,
            OnlyStraightSlide,//2长度圆弧星转直线星
            OnlyBreak,
            OnlyBreakStar,
            OnlyBreakSlide,
            OnlyBreakHold
        }
        public struct Rule
        {
            public List<string> Title;//标题
            public List<string> Shortid;//歌曲ID
            public List<string> Version;//发行版本
            public List<string> Genre;//类别
            public List<string> Bpm;//BPM
            public List<string> Des;//谱师
            public List<string> Cabinet;// SD/DX版本
            public int ConvertBpm;//BPM转换
            public NoteType ConvertNote;
            public bool Easy;
            public bool Basic;
            public bool Advanced;
            public bool Expert;
            public bool Master;
            public bool ReMaster;
            public bool inUse;
            public Rule()
            {
                Title = new();
                Shortid = new();
                Version = new();
                Genre = new();
                Bpm = new();
                Des = new();
                Cabinet = new();
                ConvertBpm = new();
                ConvertNote = new();
                Easy = Basic = Advanced = Expert = Master = ReMaster = false;
                inUse = false;
            }
        }
        public struct Target
        {
            public string Path;//路径
            public string Title;//标题
            public string Shortid;//歌曲ID
            public string Version;//发行版本
            public string Genre;//类别
            public string Bpm;//BPM
            public string Des;//谱师
            public string Cabinet;// SD/DX版本

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
            Console.WriteLine($"框架: .Net {System.Environment.Version}");
            Console.WriteLine($"运行环境: {Environment}");
            Console.WriteLine("##############################################\n");
        }
        public static void Main(string[] args)
        {
            rule = new();
            Console.Clear();
            Rule_List.Clear();
            Target_List.Clear();
            Logo();
            //初始化结束
            Console.WriteLine("欢迎使用MaiDataTool!");
            Console.WriteLine("[INFO]请输入目录:");
            List = FileManage.Search(Console.ReadLine());
            //ReloadList();
            SetOutputPath();
            SetRule();
            SetReCodingMusic();
            Confirm();
            Console.WriteLine("[INFO]正在处理，请稍后...");
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
        static void Confirm()//确认信息
        {
            Console.Clear();
            Logo();
            Information(true);
            Console.WriteLine("确认以上信息吗?(Y/N)");
            var input = Console.ReadLine();
            if (input != "Y" && input != "y")
            {
                Console.WriteLine("请输入你想进行的操作:");
                Console.WriteLine("1.编辑规则");
                Console.WriteLine("2.返回");
                Console.WriteLine("3.退出程序");
                var ReChoose = Console.ReadLine();
                if (ReChoose == "1")
                    EditRule();
                else if (ReChoose == "2")
                    Confirm();
                else if (ReChoose == "3")
                    System.Environment.Exit(0);
                else
                {
                    Console.WriteLine("[ERROR]我不是AI，不知道你在想啥");
                    Console.ReadKey();
                    Confirm();
                }
            }
        }
        static void Change_Directory_Name(Target path)
        {
            string Output;
            string Title = path.Title.Replace("/","").Replace("\\", "").Replace(":", "").Replace("?", "").Replace("*", "").Replace("\"", "").Replace("|", "").Replace("<", "").Replace(">", "");
            if(!Directory.Exists($"{Output_Path}/{path.Genre}"))
            {
                Directory.CreateDirectory($"{Output_Path}/{path.Genre}");
                Output = $"{Output_Path}/{path.Genre}";
            }
            else
            {
                Output = $"{Output_Path}/{path.Genre}";
            }
            //try
            //{
                if (Directory.Exists($"{Output}/[{path.Shortid}]{Title}"))
                {
                    Directory.Delete($"{Output}/[{path.Shortid}]{Title}");
                    Directory.Move(path.Path, $"{Output}/[{path.Shortid}]{Title}");
                    if (AudioReCoding)
                        Audio.ReCoding.ToMP3($"{Output}/[{path.Shortid}]{Title}", Convert.ToInt32(bitRate));
                 }
                else
                {
                    Directory.Move(path.Path, $"{Output}/[{path.Shortid}]{Title}");
                    if (AudioReCoding)
                        Audio.ReCoding.ToMP3($"{Output}/[{path.Shortid}]{Title}", Convert.ToInt32(bitRate));
                }
            //}
            //catch (IOException e)
            //{
            //    Console.WriteLine($"[Thread{Thread.CurrentThread.ManagedThreadId}][ERROR]发生错误，改用ID命名\n歌曲名称：[{path.Shortid}]{path.Title}");
            //    if (Directory.Exists($"{Output_Path}/[{path.Shortid}]"))
            //    {
            //        Directory.Move(path.Path, $"{Output_Path}/[{path.Shortid}]_{c++}");
            //        if (AudioReCoding)
            //            Audio.ReCoding.ToMP3($"{Output_Path}/[{path.Shortid}]_{c - 1}", Convert.ToInt32(bitRate));
            //    }
            //    else
            //    {
            //        Directory.Move(path.Path, $"{Output_Path}/[{path.Shortid}]");
            //        if (AudioReCoding)
            //            Audio.ReCoding.ToMP3($"{Output_Path}/[{path.Shortid}]", Convert.ToInt32(bitRate));
            //    }
            //}
            Finished_Count++;
            Console.WriteLine($"[Thread{Thread.CurrentThread.ManagedThreadId}]已修改乐曲：[{path.Shortid}]{path.Title}");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[INFO]已完成百分比:{Math.Round(Finished_Count / Total_Count, 2) * 100}%");
            Console.ForegroundColor = Default;
            count++;
            //}
        }
        static bool Filter(Target Target)
        {
            var Title = Target.Title;
            var Shortid = Target.Shortid;
            var Version = Target.Version;
            var Genre = Target.Genre;
            var Bpm = Target.Bpm;
            var Des = Target.Des;
            var Cabinet = Target.Cabinet;
            bool Result = true;
            foreach (var Keyword in rule.Title)
            {
                if (!Title.Contains(Keyword) && rule.Title.Count != 0)
                    Result = false;
            }
            if (!rule.Shortid.Contains(Shortid) && rule.Shortid.Count != 0)
                Result = false;
            if (!rule.Version.Contains(Version) && rule.Version.Count != 0)
                Result = false;
            if (!rule.Genre.Contains(Genre) && rule.Genre.Count != 0)
                Result = false;
            if (!rule.Bpm.Contains(Bpm) && rule.Bpm.Count != 0)
                Result = false;
            if (!rule.Des.Contains(Des) && rule.Des.Count != 0)
                Result = false;
            if (!rule.Cabinet.Contains(Cabinet) && rule.Cabinet.Count != 0)
                Result = false;         
            return Result;
        }
        static void SetOutputPath()//设置输出路径
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
                    SetOutputPath();
                }
            }             
        }
        static void SetRule()//设置筛选规则
        {
            Console.Clear();
            Logo();
            if (rule.inUse)
                Information(false);
            Console.WriteLine("[INFO]请输入想设置的筛选条件(输入\"q\"跳过):");
            Console.WriteLine("0.标题");
            Console.WriteLine("1.歌曲ID");
            Console.WriteLine("2.版本");
            Console.WriteLine("3.分类");
            Console.WriteLine("4.BPM");
            Console.WriteLine("5.谱师");
            Console.WriteLine("6.谱面版本(SD/DX)");
            switch(Console.ReadLine())
            {
                case "0":
                    Console.WriteLine("请输入歌曲标题:");
                    rule.Title.Add(Console.ReadLine());
                    rule.inUse = true;
                    break;

                case "1":
                    Console.WriteLine("请输入歌曲ID:");
                    rule.Shortid.Add(Console.ReadLine());
                    rule.inUse = true;
                    break;

                case "2":
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
                    rule.Version.Add(Console.ReadLine());
                    rule.inUse = true;
                    break;

                case "3":
                    Console.WriteLine("maimai");
                    Console.WriteLine("niconicoボーカロイド");
                    Console.WriteLine("POPSアニメ");
                    Console.WriteLine("オンゲキCHUNITHM");
                    Console.WriteLine("ゲームバラエティ");
                    Console.WriteLine("東方Project");
                    Console.WriteLine("请输入歌曲分类(请直接复制粘贴):");
                    rule.Genre.Add(Console.ReadLine());
                    rule.inUse = true;
                    break;

                case "4":
                    Console.WriteLine("请输入歌曲BPM:");
                    rule.Bpm.Add(Console.ReadLine());
                    rule.inUse = true;
                    break;

                case "5":
                    Console.WriteLine("请输入谱师昵称:");
                    rule.Des.Add(Console.ReadLine());
                    rule.inUse = true;
                    break;

                case "6":
                    Console.WriteLine("请输入谱面版本(SD/DX):");
                    rule.Cabinet.Add(Console.ReadLine());
                    rule.inUse = true;
                    break;

                case "q":
                    break;

                default:
                    Console.WriteLine("[ERROR]无效输入，请重新输入");
                    Console.ReadLine();
                    SetRule();
                    break;
            }
            
        }
        static void SetReCodingMusic()//音频文件重编码
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
                if (Filter(item))
                    Target_List.Add(item);
            }
        }
        static void EditRule()//编辑规则
        {
            Console.Clear();
            Logo();
            Information(false);
            Console.WriteLine("你想对现有的规则做什么？");
            Console.WriteLine("1.添加规则");
            Console.WriteLine("2.删除规则");
            Console.WriteLine("3.返回");
            switch(Console.ReadLine())
            {
                case "1":
                    SetRule();
                    break;

                case "2":
                    ReInput:
                    Console.WriteLine("请输入你想删除的规则类型:");
                    Console.WriteLine("[0] 标题");
                    Console.WriteLine("[1] 歌曲id");
                    Console.WriteLine("[2] 分类");
                    Console.WriteLine("[3] 版本");
                    Console.WriteLine("[4] 谱师");
                    Console.WriteLine("[5] 谱面版本");
                    Console.WriteLine("[6] BPM");
                    var Input = Console.ReadLine();
                    Console.WriteLine("请输入你想删除的条目:");
                    var Target = Console.ReadLine();
                    if (Input == "0")
                        if (rule.Title.Contains(Target))
                        {
                            rule.Title.Remove(Target);
                            EditRule();
                        }
                        else
                        {
                            Console.WriteLine($"[ERROR]条目\"{Target}\"不存在");
                            Console.ReadLine();
                            goto ReInput;
                        }
                    if (Input == "1")
                        if (rule.Shortid.Contains(Target))
                        {
                            rule.Shortid.Remove(Target);
                            EditRule();
                        }
                        else
                        {
                            Console.WriteLine($"[ERROR]条目\"{Target}\"不存在");
                            Console.ReadLine();
                            goto ReInput;
                        }
                    if (Input == "2")
                        if (rule.Genre.Contains(Target))
                        {
                            rule.Genre.Remove(Target);
                            EditRule();
                        }
                        else
                        {
                            Console.WriteLine($"[ERROR]条目\"{Target}\"不存在");
                            Console.ReadLine();
                            goto ReInput;
                        }
                    if (Input == "3")
                        if (rule.Version.Contains(Target))
                        {
                            rule.Version.Remove(Target);
                            EditRule();
                        }
                        else
                        {
                            Console.WriteLine($"[ERROR]条目\"{Target}\"不存在");
                            Console.ReadLine();
                            goto ReInput;
                        }
                    if (Input == "4")
                        if (rule.Des.Contains(Target))
                        {
                            rule.Des.Remove(Target);
                            EditRule();
                        }
                        else
                        {
                            Console.WriteLine($"[ERROR]条目\"{Target}\"不存在");
                            Console.ReadLine();
                            goto ReInput;
                        }
                    if (Input == "5")
                        if (rule.Cabinet.Contains(Target))
                        {
                            rule.Cabinet.Remove(Target);
                            EditRule();
                        }
                        else
                        {
                            Console.WriteLine($"[ERROR]条目\"{Target}\"不存在");
                            Console.ReadLine();
                            goto ReInput;
                        }
                    if (Input == "6")
                        if (rule.Bpm.Contains(Target))
                        {
                            rule.Bpm.Remove(Target);
                            EditRule();
                        }
                        else
                        {
                            Console.WriteLine($"[ERROR]条目\"{Target}\"不存在");
                            Console.ReadLine();
                            goto ReInput;
                        }
                    break;

                case "3":
                    Confirm();
                    break;

                default:
                    Console.WriteLine("[ERROR]我不是AI，不知道你在想啥");
                    Console.ReadKey();
                    EditRule();
                    break;
            }
            
        }
        static void Information(bool ShowAudioInfo)//信息预览
        {
            ReloadList();
            Console.WriteLine("-------------------------------------------");
            Console.WriteLine($"符合规则的谱面共有{Target_List.Count}个");
            Console.WriteLine("-------------------------------------------");
            foreach(var item in Target_List)
            {
                Console.WriteLine($"{item.Title}[{item.Cabinet}]--{item.Genre}");
            }
            if(rule.inUse)
            {
                Console.WriteLine("-------------------------------------------");
                Console.WriteLine("已添加以下规则:");

                Console.WriteLine("标题:");
                foreach (var Title in rule.Title)
                    Console.Write($" \"{Title}\" ");

                Console.WriteLine("歌曲id:");
                foreach (var Shortid in rule.Shortid)
                    Console.Write($" \"{Shortid}\" ");

                Console.WriteLine("分类:");
                foreach (var Genre in rule.Genre)
                    Console.Write($" \"{Genre}\" ");

                Console.WriteLine("版本:");
                foreach (var Version in rule.Version)
                    Console.Write($" \"{Version}\" ");

                Console.WriteLine("BPM:");
                foreach (var Bpm in rule.Bpm)
                    Console.Write($" \"{Bpm}\" ");

                Console.WriteLine("谱师:");
                foreach (var Des in rule.Des)
                    Console.Write($" \"{Des}\" ");

                Console.WriteLine("谱面版本:");
                foreach (var Cabinet in rule.Cabinet)
                    Console.Write($" \"{Cabinet}\" ");
                Console.WriteLine("-------------------------------------------");
            }
            else
            {
                Console.WriteLine("-------------------------------------------");
                Console.WriteLine("未设置规则");
                Console.WriteLine("-------------------------------------------");
            }
            if(ShowAudioInfo)
            {
                Console.WriteLine("-------------------------------------------\n");
                Console.WriteLine($"音频重编码:{AudioReCoding}\n音频码率:{bitRate}Kbps\n多线程编码: Enable");
                Console.WriteLine("-------------------------------------------\n");
            }
        }
    }
}
