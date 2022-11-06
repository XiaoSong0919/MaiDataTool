using NAudio.Lame;
using NAudio.Wave;
using System;
using System.IO;
using System.Threading;

namespace MaiDataTool
{
    static internal class Audio
    {
        public static class ReCoding
        {
            public static void ToMP3(string InputFile,int bitRate = 128)
            {
                try
                {
                    Console.WriteLine($"[Thread{Thread.CurrentThread.ManagedThreadId}][INFO]正在重编码音频...");
                    var InputReader = new Mp3FileReader($"{InputFile}/track.mp3");
                    var InputWriter = new WaveFileWriter($"{InputFile}/tmpfile.wav", InputReader.WaveFormat);
                    InputReader.CopyTo(InputWriter);
                    InputReader.Close();
                    InputWriter.Close();
                    var OutputReader = new AudioFileReader($"{InputFile}/tmpfile.wav");
                    var OutputWriter = new LameMP3FileWriter($"{InputFile}/track.mp3", OutputReader.WaveFormat, bitRate);
                    OutputReader.CopyTo(OutputWriter);
                    OutputReader.Close();
                    OutputWriter.Close();
                    File.Delete($"{InputFile}/tmpfile.wav");
                    Console.WriteLine($"[Thread{Thread.CurrentThread.ManagedThreadId}][INFO]音频重编码完成");
                }
                catch(IOException e)
                {
                    Console.WriteLine($"[Thread{Thread.CurrentThread.ManagedThreadId}][ERROR]文件IO错误\n{e}");
                }
                catch(NotSupportedException e)
                {
                    Console.WriteLine($"[Thread{Thread.CurrentThread.ManagedThreadId}][ERROR]不支持的音频文件格式\n{e}");
                }
                catch (ArgumentNullException e)
                {
                    Console.WriteLine($"[Thread{Thread.CurrentThread.ManagedThreadId}][ERROR]未知错误\n{e}");
                }
                catch (ObjectDisposedException e)
                {
                    Console.WriteLine($"[Thread{Thread.CurrentThread.ManagedThreadId}][ERROR]未知错误\n{e}");
                }
            }
        }//重新编码
    }
}
