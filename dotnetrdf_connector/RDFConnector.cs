﻿using System;
using System.Collections.Generic;
using System.Linq;
using VDS.RDF;
using VDS.RDF.Query;
using VDS.RDF.Storage;

namespace dotnetrdf_connector
{
    public class RDFConnector
    {
        private string serverID;
        private string repositoryID;
        private BaseSesameHttpProtocolConnector connection;
        private string ns;
        public RDFConnector(string serverID, string repositoryID, string ns)
        {
            this.serverID = serverID;
            this.repositoryID = repositoryID;
            this.ns = ns;
            connection = new SesameHttpProtocolConnector(serverID, repositoryID);
        }

        private IEnumerable<Uri> listGraphs_modified(BaseSesameHttpProtocolConnector store)
        {
            try
            {
                Object results = store.Query("SELECT DISTINCT ?g WHERE { GRAPH ?g { ?s ?p ?o } }");
                if (results is SparqlResultSet)
                {
                    List<Uri> graphs = new List<Uri>();
                    foreach (SparqlResult r in ((SparqlResultSet)results))
                    {
                        if (r.HasValue("g"))
                        {
                            INode temp = r["g"];
                            if (temp.NodeType == NodeType.Literal)
                            {
                                graphs.Add(UriFactory.Create(((ILiteralNode)temp).ToString()));
                            }
                        }
                    }
                    return graphs;
                }
                else
                {
                    return Enumerable.Empty<Uri>();
                }
            }
            catch (Exception ex)
            {
                throw StorageHelper.HandleError(ex, "listing Graphs from");
            }
        }
        public void listAllNamedGraphs()
        {
            // Get all graphs in storeID
            IEnumerable<Uri> graph_list = listGraphs_modified(connection);
            if (graph_list.Count<Uri>() == 0)
            {
                Console.WriteLine("No named graphs in " + repositoryID + " at " + serverID);
            }
            else
            {
                Console.WriteLine(repositoryID + " contains the following named graphs");
                foreach (Uri g in graph_list)
                {
                    Console.WriteLine("\t" + g.ToString());
                }
            }
        }

        public void deleteGraph(string graphName)
        {
            // Removes the specified graph
            connection.DeleteGraph(graphName);
        }
        public void clearRepository()
        {
            // Removes all statements from a store in RDF-server

            // Delete all graphs 
            if (connection.DeleteSupported)
            {
                IEnumerable<Uri> graph_list = listGraphs_modified(connection);
                if (graph_list.Count() != 0)
                {
                    Console.WriteLine("Deleting all graphs in " + repositoryID + "\n");
                    foreach (Uri g in graph_list)
                    {
                        Console.WriteLine("\tRemoving the graph: " + g.ToString() + "...");
                        connection.DeleteGraph(g);
                    }
                }
                Console.WriteLine("\n\tClearing statements from the default graph...");
                connection.DeleteGraph(""); // Clear default graph
                Console.WriteLine("Done");
            }
            else
            {
                Console.WriteLine("Delete graph is not supported in " + repositoryID);
            }
        }

        private Graph loadGraph(string graphName)
        {
            Graph g = new Graph();
            connection.LoadGraph(g, graphName);
            return g;
        }

        public void AddStatements(List<TripleStructure> triples, string graphName)
        {
            Console.WriteLine("Adding statements to " + repositoryID + "...");
            Graph g;
            try
            {
                g = loadGraph(ns + graphName);
            }
            catch (System.UriFormatException ex)
            {
                // No graph was found named graphName, create new graph
                g = new Graph();
                g.BaseUri = UriFactory.Create(ns + graphName);
            }
            foreach (TripleStructure t in triples)
            {
                IUriNode subject = g.CreateUriNode(UriFactory.Create(ns + t.subject));
                IUriNode predicate = g.CreateUriNode(UriFactory.Create(ns + t.predicate));
                ILiteralNode objectL = g.CreateLiteralNode(ns + t.objectL);
                g.Assert(new Triple(subject, predicate, objectL));
            }

            connection.SaveGraph(g);
            Console.WriteLine("Done");

        }
    }
}


