using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Xamarin.Essentials;
using Xamarin.Forms;
using System.Diagnostics;
using System.Threading;
using Ffmpegkit.Droid;

[assembly: Dependency(typeof(Diplom2.Droid.Subtitles))]
namespace Diplom2.Droid
{
    public class Subtitles : ISubtitels
    {
        public string Work(string inputPath)
        {
            string outputPath = "/storage/emulated/0/Download/output.wav";
            if(File.Exists(outputPath))
            {
                File.Delete(outputPath);
            }
            var command = $"-i \"{inputPath}\" -vn -ac 1 -ar 16000 \"{outputPath}\"";
            FFmpegKitConfig.IgnoreSignal(Ffmpegkit.Droid.Signal.Sigxcpu);
            FFmpegSession session = FFmpegKit.ExecuteAsync(command, new FFmpegCallback());
            return outputPath;
        }
    }
    public class FFmpegCallback : Java.Lang.Object, IFFmpegSessionCompleteCallback
    {
        public void Apply(FFmpegSession session)
        {
            var returnCode = session.ReturnCode;
            if (ReturnCode.IsSuccess(returnCode))
            {
                Console.WriteLine("Конвертация успешна!");
            }
            else
            {
                Console.WriteLine($"Ошибка: {session.FailStackTrace}");
            }
        }
    }
}