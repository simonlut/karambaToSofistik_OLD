using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;       //importing DLLS

namespace karambaToSofistik.AccessSofistik
{

    public static class AccessSofData
    {

        public static List<string> SofBeamForces = new List<string>();
        // In this example 64bit dlls are used (Visual Studio Platform 64bit)

        // sof_cdb_init
        [DllImport("cdb_w_edu50_x64.dll", CallingConvention = CallingConvention.Cdecl)]
        // [DllImport("cdb_w33_x64.lib")]
        public static extern int sof_cdb_init(
            string name_,
            int initType_
        );

        // sof_cdb_close
        [DllImport("cdb_w_edu50_x64.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void sof_cdb_close(
            int index_);

        // sof_cdb_status
        [DllImport("cdb_w_edu50_x64.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int sof_cdb_status(
            int index_);

        // sof_cdb_flush
        [DllImport("cdb_w_edu50_x64.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int sof_cdb_flush(
            int index_);

        // sof_cdb_flush
        [DllImport("cdb_w_edu50_x64.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int sof_cdb_free(
            int kwh_,
            int kwl_);

        // sof_cdb_flush
        [DllImport("cdb_w_edu50_x64.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void sof_cdb_kenq_ex(
            int index,
            ref int kwh_,
            ref int kwl_,
            int request_);

        // sof_cdb_get
        [DllImport("cdb_w_edu50_x64.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int sof_cdb_get(
            int index_,
            int kwh_,
            int kwl_,
            ref beamForces data_,
            ref int recLen_,
            int pos);

        public static unsafe void Main()
        {
            int index = 0;
            int status = 0;
            int datalen;


            string directory1 = @"C:\Program Files\SOFiSTiK\2018\SOFiSTiK 2018\interfaces\64bit";
            string directory2 = @"C:\Program Files\SOFiSTiK\2018\SOFiSTiK 2018";
            string cdbPath = @"C:\Users\simon\Downloads\so\sof.cdb";

            // Get the path
            string path = Environment.GetEnvironmentVariable("path");

            // Set the new path environment variable + SOFiSTiK dlls path
            path = directory1 + ";" + directory2 + ";" + path;

            // Set the path variable (to read the data from CDB)
            System.Environment.SetEnvironmentVariable("path", path);

            // connect to CDB
            index = sof_cdb_init(cdbPath, 99);
            // check if sof_cdb_flush is working
            status = AccessSofData.sof_cdb_status(index);


            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/k echo Index:" + index;
            process.StartInfo = startInfo;
            process.Start();

            beamForces getBeamForces = new karambaToSofistik.AccessSofistik.beamForces();
            datalen = Marshal.SizeOf(typeof(karambaToSofistik.AccessSofistik.beamForces));

            int pos = 1;

            while (sof_cdb_get(index, 102, 1, ref getBeamForces, ref datalen, pos) < 2)
            {
                if(sof_cdb_get(index, 102, 1, ref getBeamForces, ref datalen, pos) == 0)
                {
                    SofBeamForces.Add("Beam: " + getBeamForces.m_id.ToString() + " Normal Force:" + getBeamForces.m_n.ToString());


                }

                datalen = Marshal.SizeOf(typeof(karambaToSofistik.AccessSofistik.beamForces));

            }




            // use sof_cdb_flush() and sof_cdb_close()
            sof_cdb_flush(index);
            sof_cdb_close(0);   // close the CDB

            // Output the status after closing the CDB
            ////Console.WriteLine();
            ////if (sof_cdb_status(index) == 0)
            ////{
            ////    Console.WriteLine("CDB Status = 0, CDB closed succesfully");
            ////}
            ////else
            ////{
            ////    Console.WriteLine("CDB Status <> 0, the CDB doesn't closed successfully");
            ////}

            //Console.Write("Press any key to close the application...");
            //Console.ReadKey();
    }
}
}