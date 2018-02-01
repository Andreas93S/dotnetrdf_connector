using System;
using System.Collections.Generic;
using System.Text;

namespace dotnetrdf_connector
{
    public class TripleStructure
    {
        public string subject;
        public string predicate;
        public string objectL;
        public bool isUri;

        public TripleStructure(string subject, string predicate, string objectL, bool isUri)
        {
            this.subject = subject;
            this.predicate = predicate;
            this.objectL = objectL;
            this.isUri = isUri;
        }
    }
}
