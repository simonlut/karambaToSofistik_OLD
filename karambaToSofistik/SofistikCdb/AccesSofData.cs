using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;       //importing DLLS


namespace karambaToSofistik.SofistikCdb
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

            if ( karambaToSofistikComponent.filePath != ""){
                index = sof_cdb_init(karambaToSofistikComponent.filePath, 99);
            }

            string directory = "C:\\ Program Files \\ SOFiSTiK \\2018\\ SOFiSTiK2018\\ interfaces \\64 bit";

            // Get the Path

            string path = Environment.GetEnvironmentVariables("path");

            // Set the new path with the directory
            path = directory + ";" + path;

            // Set the path variable (to read the data from the CDB)
            System.Environment.SetEnvironmentVariable("path", path);


        }
    }
}
