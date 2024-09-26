using System;
using System.Collections.Generic;

namespace CSGenio.reporting.serialization
{
    [Serializable()]
    public class DataSet
    {
        [System.Xml.Serialization.XmlAttribute]
        public string Name;
        public Query Query = new Query();
        public List<Field> Fields = new List<Field>();

        /// <summary>
        /// copy the query parameters values (probably from the QueryString) into this report's parameters (in this object)
        /// </summary>
        /// <param name="webParameters"></param>
        public void AssignParameters(Dictionary<string,string> webParameters)
        {
            foreach (QueryParameter param in this.Query.QueryParameters)
            {
                string paramName = param.Name.Replace("@", "");
                //if this report param was passed as an arg to the report, then populate it
                if (webParameters[paramName] != null)
                    param.Value = webParameters[paramName].ToString();
            }
        }

    }
}