using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OpenCPULib
{
    public class OpenCPUResult
    {
        private Dictionary<string, string> LinksByName { get; set; }
        private List<string> Links { get; set; }

        private WebClient wc;

        internal OpenCPUResult(WebClient wc, Uri modelUrl, string respStr)
        {
            this.wc = wc;
            // the response consists of links, which are seperated by /n 
            // which we split into its components
            //
            // example response:
            // /ocpu/tmp/x049176415a/R/.val
            // /ocpu/tmp/x049176415a/messages
            // /ocpu/tmp/x049176415a/source 
            // /ocpu/tmp/x049176415a/console
            // /ocpu/tmp/x049176415a/info
            // /ocpu/tmp/x049176415a/files/DESCRIPTION

            Links = respStr.Split('\n').ToList();

            LinksByName = new Dictionary<string, string>();

            foreach (var link in Links)
            {
                var seg = link.Split('/');
                // we store the absolute link
                LinksByName[seg.Last()] = string.Format("{0}://{1}{2}", modelUrl.Scheme, modelUrl.Host, link);
            }
        }

        public List<double> GetDoubles()
        {
            try
            {
                // we read the values from the provided link
                StreamReader sr = new StreamReader(wc.OpenRead(LinksByName[".val"]));
                string rawPrediction = sr.ReadToEnd();
                // the parsing will depend on the called R function
                // in this case, we just extract all doubles and put them in a list
                var predictions = ParsDoubles(rawPrediction);
                return predictions;
            }
            catch (WebException)
            {
                return null;
            }
        }

        private static List<double> ParsDoubles(string rawPrediction)
        {
            var predictions = new List<double>();
            var lines = rawPrediction.Split('\n');
            foreach (var line in lines)
            {
                var values = line.Split(' ');
                foreach (var value in values)
                {
                    double dv;
                    if (double.TryParse(value, out dv))
                    {
                        predictions.Add(dv);
                    }
                }
            }
            return predictions;
        }
    }
}
