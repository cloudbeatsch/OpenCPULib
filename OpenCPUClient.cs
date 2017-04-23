using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OpenCPULib
{
    public class OpenCPUClient
    {
        private WebClient wc;
        private Uri modelUrl;


        public OpenCPUClient(string modelUrlStr)
        {
            modelUrl = new Uri(modelUrlStr);
            wc = new WebClient();
            wc.Headers["Content-Type"] = "application/json";
        }
        public OpenCPUResult ExecutePrediction(object requestObj)
        {
            try
            {
                var respObj = wc.UploadData(modelUrl.ToString(), "POST",
                                System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(
                                    requestObj
                                 )));
                return new OpenCPUResult(wc, modelUrl, System.Text.Encoding.ASCII.GetString(respObj));
            }
            catch (WebException)
            {
                return null;
            }
        }
    }
}
