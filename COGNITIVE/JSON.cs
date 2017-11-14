using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace COGNITIVE
{
    static class JSON
    {
        public static JObject GenerateJSONDocument(List<Documents> arrayContent)
        {
            JObject documents =
                new JObject(
                    new JObject(
                        new JProperty("documents",
                            new JArray(
                                from p in arrayContent
                                select new JObject(
                                    new JProperty("id", p.id),
                                    new JProperty("text", p.text))))));

            return documents;
        }
    }
}
