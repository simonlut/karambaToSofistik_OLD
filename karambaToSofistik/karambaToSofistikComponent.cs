// Karamba To Sofistik component for GrassHopper
// Convert a karamba model to a .dat file readable by Sofistik
// Git: https://github.com/AlbericTrancart/karambaToSofistik
// Contact: alberic.trancart@eleves.enpc.fr

using System;
using System.Collections.Generic;
using System.IO;

using Grasshopper.Kernel;
using Rhino.Geometry;

using Karamba.Models;
using Karamba.Elements;

using karambaToSofistik.Classes;

namespace karambaToSofistik {
    public class karambaToSofistikComponent : GH_Component {

        // Component configuration
        public karambaToSofistikComponent() : base("karambaToSofistik", "ktS", "Converts a Karamba model to a .dat file readable by Sofistik", "Karamba3D", "Extra") { }

        // Registers all the input parameters for this component.
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager) {
            pManager.AddParameter(new Param_Model(), "Model", "Model", "Model to convert", GH_ParamAccess.item);
            pManager.AddTextParameter("Path", "Path", "Save the .dat file to this path", GH_ParamAccess.item, @"");
            pManager.AddBooleanParameter("Sofistikate", "Calc", "Calculates current DAT file", GH_ParamAccess.item, false);
        }

        // Registers all the output parameters for this component.
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager) {
            pManager.Register_StringParam("Output", "Output", "Converted model");
            pManager.Register_StringParam("Status", "Status", "Errors or success messages");
        }

        // We need to register all groups defined in Grasshopper
        static public List<string> beam_groups = new List<string>();

        // This is the method that actually does the work.
        protected override void SolveInstance(IGH_DataAccess DA) {
            // Some variables
            string output = "";                        // The file output
            string status = "Starting component...\n"; // The debug output

            // Several arrays where the data is stored
            List<Material> materials = new List<Material>();
            List<CrossSection> crossSections = new List<CrossSection>();
            List<Node> nodes = new List<Node>();
            List<Beam> beams = new List<Beam>();
            List<Load> loads = new List<Load>();


            // We need to reset some variables because the objects are not freed until Grasshopper is unloaded
            Parser.id_count = 1;

            bool iSofistik = false;

            DA.GetData(2, ref iSofistik); 

            try {
                // Load the data from Karamba

                // Retrieve and clone the input model
                GH_Model in_gh_model = null;
                if (!DA.GetData<GH_Model>(0, ref in_gh_model)) return;
                Model model = in_gh_model.Value;
                model = (Karamba.Models.Model) model.Clone(); // If the model is not cloned a modification to this variable will imply modification of the input model, thus modifying behavior in other components.
                if (model == null) {
                    status += "ERROR: The input model is null.";
                    output = "Nothing to convert";
                }
                else {
                    string path = null;
                    if (!DA.GetData<string>(1, ref path)) { path = ""; }
                    if (path == "") {
                        status += "No file path specified. Will not save data to a .dat file.\n";
                    }



                    // Retrieve and store the data
                    // Materials
                    for(int i = 0; i< model.materials.Count; i++)
                    {
                        materials.Add(new Material(model.materials[i], i+1));
                    }

                    status += materials.Count + " materials loaded...\n";

                    // Cross sections
                    for (int i = 0; i<model.crosecs.Count; i++)
                    {
                       crossSections.Add(new CrossSection(model.crosecs[i], i+1));

                    }

                        //Add beam ids to Cross sections
                    foreach (Karamba.Elements.ModelBeam beam in model.elems)
                    {
                        foreach (CrossSection crosec in crossSections)
                        {
                            if (beam.crosec.name == crosec.name)
                            {
                                crosec.ids.Add(beam.ind.ToString());
                            }

                        }

                        foreach (Material material in materials)
                        {
                            if (beam.crosec.material.name == material.name)
                            {
                                material.ids.Add(beam.ind.ToString());
                            }

                        }
                    }


                    // Nodes
                    foreach (Karamba.Nodes.Node node in model.nodes) {
                    nodes.Add(new Node(node));
                    }
                    status += nodes.Count + " nodes loaded...\n";

                    // Supports
                    foreach (Karamba.Supports.Support support in model.supports) {
                        nodes[support.node_ind].addConstraint(support);
                    }
                    status += "Support constraints added to " + model.supports.Count + " nodes.\n";

                    // Beams
                    foreach (Karamba.Elements.ModelElement beam in model.elems) {
                        Beam curBeam = new Beam(beam);
                        // Adding the start and end nodes
                        curBeam.start = nodes[curBeam.ids[0]];
                        curBeam.end = nodes[curBeam.ids[1]];
                        beams.Add(curBeam);
                      }

                    status += beams.Count + " beams loaded...\n";

                    // Loads
                    foreach (KeyValuePair<int, Karamba.Loads.GravityLoad> load in model.gravities) {
                        loads.Add(new Load(load));
                        }
                    status += model.gravities.Count + " gravity loads added.\n";
                    foreach (Karamba.Loads.PointLoad load in model.ploads) {
                        Load current = new Load(load);
                        current.node = nodes[load.node_ind];                            
                        loads.Add(current);
                    }
                    status += model.ploads.Count + " point loads added.\n";
                    
                    foreach (Karamba.Loads.ElementLoad load in model.eloads) {
                        // Create a load variable base on the load type
                        Load current = new Load();
                        Karamba.Loads.UniformlyDistLoad line = load as Karamba.Loads.UniformlyDistLoad;
                        //Karamba.Loads.PreTensionLoad pret = load as Karamba.Loads.PreTensionLoad;
                        Karamba.Loads.TemperatureLoad temp = load as Karamba.Loads.TemperatureLoad;

                        if (line != null) {
                            current = new Load(line);
                        }
                        // Very important to check Temperature BEFORE Pretension becaus Temperature derivates from Pretension
                        else if (temp != null) {
                            current = new Load(temp);
                        }
                        //else if (pret != null) {
                        //    current = new Load(pret);
                        //}
                        

                        // If there is not target element, apply the load to the whole structure
                        //if (load.beamIds[0] == "") {
                        //    current.beam_id = "";
                        //    loads.Add(current);
                        //}
                        else {
                            // We search the element
                            current.beam = beams.Find(delegate(Beam beam) {
                                return beam.user_id == load.beamIds[0];
                            });
                            loads.Add(current);
                        }
                    }

                    // ID matching
                    // Karamba and Sofistik use different ID systems
                    // Karamba's materials and cross sections are pointing to an element ID
                    // Sofistik's elements need a cross section ID which needs a material ID
                    
                    foreach (Material material in materials){
                        // If the IDs list is empty, it means that we want to apply the material to the whole structure (whichi is the default behavior: the default material is set by the constructors of all elements)
                        bool test = false;
                        foreach (string id in material.ids) {
                            if(id != "")
                                test = true;
                        }
                        if (test) {
                            foreach (CrossSection crosec in crossSections) {
                                if(material.ids.Contains((crosec.id).ToString()))
                                    crosec.material = material;
                            }
                        }
                    }
                    status += "Matching with material IDs...\n";
                    
                    foreach (CrossSection crosec in crossSections) {
                        // If the IDs list is empty, it means that we want to apply the cross section to the whole structure (which is the default behavior: the default cross section is set by the constructors of all elements)
                        bool test = false;
                        foreach (string id in crosec.ids) {
                            if (id != "")
                                test = true;
                        }
                        if (test) {
                            foreach (Beam beam in beams) {
                                if (crosec.ids.Contains((beam.id - 1).ToString()))
                                    beam.sec = crosec;
                            }
                        }
                    }
                    status += "Matching with cross section IDs...\n";

                    // Write the data into a .dat file format
                    Parser parser = new Parser(materials, crossSections, nodes, beams, loads);
                    output = parser.file;

                    if (path != "")
                    {
                        status += "Saving file to " + path + "\n";

                        System.IO.File.WriteAllText(@path, output);
                        status += "File saved!\n";
                    }



                    if (iSofistik == true && Directory.Exists(@"C:/Program Files/SOFiSTiK/2018/SOFiSTiK 2018") && path != "")
                    {
                        string targetPath = System.IO.Path.GetFullPath(path);
                        System.Diagnostics.Process process = new System.Diagnostics.Process();
                        System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                        startInfo.FileName = "cmd.exe";
                        startInfo.Arguments = "/k cd C:/Program Files/SOFiSTiK/2018/SOFiSTiK 2018/ & sps -B " + targetPath + targetPath;
                        process.StartInfo = startInfo;
                        process.Start();
                    }
                    else if (Directory.Exists(@"C:/Program Files/SOFiSTiK/2018/SOFiSTiK 2018") == false)
                    {
                        System.Diagnostics.Process process = new System.Diagnostics.Process();
                        System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                        startInfo.FileName = "cmd.exe";
                        startInfo.Arguments = "/k echo Directory for Sofistik 2018 not found. Set the directory path manually to your Sofistik main directory using a string. ";
                        process.StartInfo = startInfo;
                        process.Start();
                    }

                }
            }
            catch (Exception e) {
                status += "\nERROR!\n" + e.ToString() + "\n" + e.Data;
            }

            // Return data
            DA.SetData(0, output);
            DA.SetData(1, status);
        }

        // Icon
        protected override System.Drawing.Bitmap Icon {
            get { return Resource.Icon; }
        }

        // Each component must have a unique Guid to identify it. 
        public override Guid ComponentGuid {
            get { return new Guid("{1954a147-f7a2-4d9c-b150-b788821ccae7}"); }
        }
    }
}
