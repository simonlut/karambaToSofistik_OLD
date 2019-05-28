﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;       //importing DLLS


namespace karambaToSofistik.SofistikCdb
{
    public class BeamForces
    {
        public unsafe struct beamForces                  //        102/LC:0  Maximum of Total Beam forces and deformations
        {
            public int m_id;                           //        identifier 0
            public float m_x;                          // [1001] max. beam length
            public float m_n;                          // [1101] normal force
            public float m_vy;                         // [1102] y-shear force
            public float m_vz;                         // [1102] z-shear force
            public float m_mt;                         // [1103] torsional moment
            public float m_my;                         // [1104] bending moment My
            public float m_mz;                         // [1104] bending moment Mz
            public float m_mb;                         // [1105] warping moment Mb
            public float m_mt2;                        // [1103] 2nd torsionalmom.
            public float m_ux;                         // [1003] diplacem. local x
            public float m_uy;                         // [1003] diplacem. local y
            public float m_uz;                         // [1003] diplacem. local z
            public float m_phix;                       // [1004] rotation local x
            public float m_phiy;                       // [1004] rotation local y
            public float m_phiz;                       // [1004] rotation local z
            public float m_phiw;                       // [1005] twisting
            public float m_mt3;                        // [1103] 3rd torsionalmom
            public float m_pa;                         // [1095] axial bedding
            public float m_pt;                         // [1095] transverse bedding
            public float m_pty;                        // [1095] local y component of transverse bedding
            public float m_ptz;                        // [1095] local z component of transverse bedding
        } // cs_beam_foc

        // sof_cdb_get
        [DllImport("cdb_w50_x64.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int sof_cdb_get(
            int index_,
            int kwh_,
            int kwl_,
            ref beamForces data_,
            ref int recLen_,
            int pos);
    }
}
