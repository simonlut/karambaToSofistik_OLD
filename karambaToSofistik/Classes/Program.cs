using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;       //importing DLLS

namespace ConsoleApp1
{
    public static class AccesSofData
    {

        // In this example 64bit dlls are used (Visual Studio Platform 64bit)

        // sof_cdb_init
        [DllImport("cdb_w50_x64.dll", CallingConvention = CallingConvention.Cdecl)]
        // [DllImport("cdb_w33_x64.lib")]
        public static extern int sof_cdb_init(
            string name_,
            int initType_
        );

        // sof_cdb_close
        [DllImport("cdb_w50_x64.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void sof_cdb_close(
            int index_);

        // sof_cdb_status
        [DllImport("cdb_w50_x64.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int sof_cdb_status(
            int index_);

        // sof_cdb_flush
        [DllImport("cdb_w50_x64.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int sof_cdb_flush(
            int index_);

        // sof_cdb_flush
        [DllImport("cdb_w50_x64.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int sof_cdb_free(
            int kwh_,
            int kwl_);

        // sof_cdb_flush
        [DllImport("cdb_w50_x64.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void sof_cdb_kenq_ex(
            int index,
            ref int kwh_,
            ref int kwl_,
            int request_);

        public static unsafe void Main(string[] args)
        {
            int index = 0;
            int status = 0;
            int datalen;

            string directory = @"C:\\ Program Files \\ SOFiSTiK \\2018\\ SOFiSTiK2018\\ interfaces \\64 bit";
            string cdbPath = Path.GetDirectoryName("C: \\Users\\simon\\Downloads\\so\\sof.dat") + "\\" + Path.GetFileNameWithoutExtension("C: \\Users\\simon\\Downloads\\so\\sof.dat") + ".cdb";


            // Get the Path

            string path = Environment.GetEnvironmentVariable("path");

            // Set the new path with the directory
            path = directory + ";" + path;

            // Set the path variable (to read the data from the CDB)
            System.Environment.SetEnvironmentVariable("path", path);

            // Connect to cdb
            index = sof_cdb_init(@cdbPath, 99);

            // Check if sof_cdb_flush is working
            status = sof_cdb_flush(index);

            // Print index and status

            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = " /k" + "echo Index: " + index + " & echo Status: " + status;
            process.StartInfo = startInfo;

            if (sof_cdb_status(index) == 0)
            {
                startInfo.Arguments = ("CDB status = 0, CDB closed succesfylly");
            }
            else
            {
                startInfo.Arguments = ("CDB Status <> 0, the CDB doesn't close succesfully.");
            }

            process.Start();
        }
    }
}
