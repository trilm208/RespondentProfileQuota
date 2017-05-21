using System;
using System.Data;

using Xamarin.Forms;
using System.IO;
using System.IO.Compression;
using System.Net;
using DataAccess;

using Shell.Core;
using RespondentProfileQuota.Droid;
using DataAcessService;

[assembly: Dependency(typeof(HttpPost_Android))]
namespace RespondentProfileQuota.Droid
{
    public class HttpPost_Android: IHttpPost
    {
        public string HttpURL { get;  set; }

        public CookieContainer HttpCookies { get;  set; }

        public string HttpUserAgent { get;  set; }

        public void HttpDataServices()
        {
            //this.HttpURL = @"http://10.0.2.2:8080/DataAccess.ashx";
            //this.HttpCookies = new CookieContainer();
            //this.HttpUserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.1; Trident/6.0)";
        }
        public string HttpPost(string data,HttpDataServices DataServices)
        {
            //HttpDataServices();
            var request = (HttpWebRequest)HttpWebRequest.Create(DataServices.HttpURL);
            request.CookieContainer = DataServices.HttpCookies;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.UserAgent = DataServices.HttpUserAgent;
            request.AllowAutoRedirect = true;
            //request.Headers.Add("Accept-Encoding", "gzip,deflate");

            using (var writer = new StreamWriter(request.GetRequestStream()))
            {
                if (string.IsNullOrEmpty(data) == false)
                {
                    writer.Write(data);
                }
            }
            var response = (HttpWebResponse)request.GetResponse();

            using (var stream = GetStreamForResponse(response))
            {
                using (var reader = new StreamReader(stream))
                {
                    var result = reader.ReadToEnd();
                    return result;
                }
            }
        }

        public  Stream GetStreamForResponse(HttpWebResponse webResponse)
        {
            Stream stream;
            switch (webResponse.ContentEncoding.ToUpperInvariant())
            {
                case "GZIP":
                    stream = new GZipStream(webResponse.GetResponseStream(), CompressionMode.Decompress);
                    break;

                case "DEFLATE":
                    stream = new DeflateStream(webResponse.GetResponseStream(), CompressionMode.Decompress);
                    break;

                default:
                    stream = webResponse.GetResponseStream();
                    break;
            }
            return stream;
        }

    }
}