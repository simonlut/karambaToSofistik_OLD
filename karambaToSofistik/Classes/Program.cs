using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            // The code provided will print ‘Hello World’ to the console.
            // Press Ctrl+F5 (or go to Debug > Start Without Debugging) to run your app.
            string path = "C:/Users/simon/Downloads/so/sof.dat";
            string targetPath = System.IO.Path.GetFullPath(path);

            if(Directory.Exists(@"C:/Program Files/SOFiSTiK/2018/SOFiSTiK 2018"))
            {
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.FileName = "cmd.exe";
                startInfo.Arguments = "/k cd C:/Program Files/SOFiSTiK/2018/SOFiSTiK 2018/ & sps -B " + targetPath;
                process.StartInfo = startInfo;
                process.Start();
            }

            else
            {
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.FileName = "cmd.exe";
                startInfo.Arguments = "/k echo Directory for Sofistik 2018 not found. Set the directory path manually to your Sofistik main directory using a string. "
                process.StartInfo = startInfo;
                process.Start();
            }

            // Go to http://aka.ms/dotnet-get-started-console to continue learning how to build a console app! 
        }
    }
}
