using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Grasshopper.Kernel;
using Grasshopper;

namespace karambaToSofistik.Classes {
    class Parser {
        static public int id_count = 1; // Karamba does not provide IDs for loads so we must create one
        public string file { get; protected set; }
        public List<int> loadcase_index = new List<int> {};

        public Parser(List<Material> materials, List<CrossSection> crossSections, List<Node> nodes, List<Beam> beams, List<Load> loads) {
            file = "";
            
            // AQUA definitions
            file += "+PROG AQUA urs:1\nHEAD Material and cross section definitions\nUNIT 5\nNORM DIN EN1992-2004\n\n";

            foreach (Material material in materials) {
                if(material.sofistring() != "")
                    file += material.sofistring() + "\n";
            }
            file += "\n";

            foreach (CrossSection crossSection in crossSections) {
                    if (crossSection.sofistring() != "")
                        file += crossSection.sofistring() + "\n";
            }

            // SOFIMSHA definitions
            file += "\nEND\n\n\n+PROG SOFIMSHA urs:2\nHEAD Elements\nUNIT 5\nSYST TYPE 3D GDIR NEGZ GDIV 1000\n\n";

            foreach (Node node in nodes) {
                file += node.sofistring() + "\n";
            }

            // Special addition of beam: we must define groups
            int cluster_start, node_start, node_end, crosec; 
            cluster_start = node_start = node_end = crosec = 1;
            int iterator = 0;
            Beam last_beam = new Beam();

            foreach (string group in karambaToSofistikComponent.beam_groups) {
                if (karambaToSofistikComponent.beam_groups.Count > 0) {
                    file += "\nGRP " + (karambaToSofistikComponent.beam_groups.IndexOf(group)+1)+";\n";
                    iterator = 0;

                    foreach (Beam beam in beams) {
                        //Initiate first beam
                        if (last_beam == new Beam())
                            last_beam = beam;

                        // Output one group after the other
                        if (beam.user_id == group) {
                            // Beams are automatically ordered by their ID, therefore it is simple to clear the syntax by defining them in cluster
                            if (iterator == 0) {
                                // Start a new cluster
                                cluster_start = beam.id;
                                node_start = beam.start.id;
                                node_end = beam.end.id;
                                crosec = beam.sec.id;
                                iterator = 1;
                                last_beam = beam;
                                continue;
                            }

                            // Check if we are moving into another cluster
                            if (beam.id != cluster_start + iterator
                                || beam.start.id != node_start + iterator
                                || beam.end.id != node_end + iterator
                                || beam.sec.id != crosec) {

                                // End the cluster and print it
                                if (iterator == 1) {
                                    // Normal beam
                                    file += last_beam.sofistring() + "\n";
                                }
                                else {
                                    // Clusterized definition
                                    file += "BEAM NO (" + cluster_start + " " + (cluster_start + iterator - 1) + " 1)"
                                          + " NA (" + node_start + " 1)"
                                          + " NE (" + node_end + " 1)"
                                          + " NCS " + crosec + " DIV " + karambaToSofistikComponent.beamSplit + "\n";
                                }

                                // Start a new cluster
                                cluster_start = beam.id;
                                node_start = beam.start.id;
                                node_end = beam.end.id;
                                crosec = beam.sec.id;
                                iterator = 1;
                                last_beam = beam;
                                continue;
                            }
                            else {
                                iterator++;
                                last_beam = beam;
                            }
                        }
                    }

                    // Print the last cluster
                    if (iterator == 1) {
                        // Normal beam
                        file += last_beam.sofistring() + "\n";
                    }
                    else {
                        // Clusterized definition
                        file += "BEAM NO (" + cluster_start + " " + (cluster_start + iterator - 1) + " 1)"
                              + " NA (" + node_start + " 1)"
                              + " NE (" + node_end + " 1)"
                              + " NCS " + crosec + " DIV " + karambaToSofistikComponent.beamSplit + "\n";
                    }
                }
            }

            // SOFILOAD definitions
            file += "END\n\n+PROG SOFILOAD urs:4\nHEAD Loads\nUNIT 5\n\n";


            foreach( Load load in loads)
            {
                if (loadcase_index.Contains(load.loadcase) == false)
                {
                    loadcase_index.Add(load.loadcase);
                }
            }
            loadcase_index.Sort();

            for (int i=0; i<loadcase_index.Count; i++)
            {   foreach (Load load in loads)
                {
                    if (load.loadcase == loadcase_index[i])
                    {
                        file += load.sofistring(loadcase_index[i]) + "\n";
                    }
                }
                id_count = 1;
            }
            
            // Analysis
            file += "\nEND\n\n\n+PROG ASE urs:13\nHEAD Solving\n\nSYST PROB LINE\nLC ALL\n\nEND\n";
        }
    }
}
