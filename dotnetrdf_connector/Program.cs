using System;

namespace dotnetrdf_connector
{
    class Program
    {
        static void Main(string[] args)
        {
            string serverID = "http://vm25.cs.lth.se/rdf4j-server";
            string repositoryID = "test_workspace";
            RDFConnector rdfConn = new RDFConnector(serverID, repositoryID);

            rdfConn.listAllNamedGraphs();

            Console.ReadLine();
        }
    }
}