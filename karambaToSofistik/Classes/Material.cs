﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace karambaToSofistik.Classes {
    class Material {
        public int id;
        public List<string> ids; // Elements to apply to
        public double E, G, gamma, alphaT, fy;
        string name;

        public Material(Karamba.Materials.FemMaterial material = null, int mat_id = 0) {
            ids  = new List<string>();
            id   = 0;
            E    = G 
                 = gamma 
                 = alphaT 
                 = fy 
                 = 0;
            name = "";

            if (material != null)
                hydrate(material, mat_id);
        }

        public void hydrate(Karamba.Materials.FemMaterial material, int mat_id) {
            id     = mat_id;
            ids    = material.elemIds;
            E      = Math.Round(material.E() / 1000, 3);
            G      = Math.Round(material.G12() / 1000, 3);
            gamma  = Math.Round(material.gamma(), 3);
            alphaT = Math.Round(material.alphaT(), 3);
            fy     = Math.Round(material.fy() / 1000, 3);
            name   = material.name;
        }

        public string sofistring() {
            // We need not to forget ton convert into units used by Sofistik
            return "STEE NO " + id + " ES "   + E
                                   + " GAM "  + gamma
                                   + " ALFA " + alphaT
                                   + " GMOD " + G
                                   + " FY "   + fy;
        }

        //Check if "test" is a duplicate of this material - necessary because karamba adds preset materials
        public bool duplicate(Material test) {
            if(E == test.E
                    && G == test.G
                    && gamma == test.gamma
                    && alphaT == test.alphaT
                    && fy == test.fy
                ) {
                    ids.AddRange(test.ids);
                    return true;
            }
            return false;
        }
    }
}
