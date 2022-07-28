using NAudio.Lame;
using NAudio.Wave;
using System;
using System.IO;

namespace MaiDataChangeName
{
    static internal class Audio
    {
        public static class ReCoding
        {
            public static void MP3(string InputFile,int bitRate = 128)
            {
                try
                {
                    Console.WriteLine("[INFO]正在重编码音频...");
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
                    Console.WriteLine("[INFO]音频重编码完成");
                }
                catch(IOException e)
                {
                    Console.WriteLine($"[ERROR]文件IO错误\n{e}");
                }
                catch(NotSupportedException e)
                {
                    Console.WriteLine($"[ERROR]不支持的音频文件格式\n{e}");
                }
                catch (ArgumentNullException e)
                {
                    Console.WriteLine($"[ERROR]未知错误\n{e}");
                }
                catch (ObjectDisposedException e)
                {
                    Console.WriteLine($"[ERROR]未知错误\n{e}");
                }
            }
        }//重新编码
    }
}
