using System;
using System.Collections.Generic;

namespace dotnetrdf_connector
{
    class Program
    {
        static void Main(string[] args)
        {
            string serverID = "http://vm25.cs.lth.se/rdf4j-server";
            string repositoryID = "test_workspace";
            string ns = "http://kif.cs.lth.se/";
            RDFConnector rdfConn = new RDFConnector(serverID, repositoryID, ns);

            rdfConn.listAllNamedGraphs();
            rdfConn.clearRepository();

            // Create some simple statements
            List<TripleStructure> tripleList = new List<TripleStructure>();
            tripleList.Add(new TripleStructure("skill1", "hasAction", "action1", true));
            tripleList.Add(new TripleStructure("skill2", "hasAction", "action1", true));
            rdfConn.AddStatements(tripleList, "skills");

            Console.ReadLine();
        }
    }
}