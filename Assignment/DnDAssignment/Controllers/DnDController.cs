using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web.Http;
using System.Net.Http;
using System.Text;
using System.Web.Routing;

namespace DnDAssignment.Controllers
{
    [RoutePrefix("DnD/DnDAssignment")]
    public class DnDController : ApiController
    {
        [HttpGet]
        [Route("Races")]
        public string get_races()
        {
            string json_string;
            HttpWebRequest web_req;
            HttpWebResponse web_resp;
            
            web_req = (HttpWebRequest)WebRequest.Create(string.Format("http://www.dnd5eapi.co/api/races"));
            web_req.Method = "GET";
            web_resp = (HttpWebResponse) web_req.GetResponse();
            
            Console.WriteLine(web_resp.StatusCode);

            using (Stream str = web_resp.GetResponseStream())
            {
                StreamReader reader = new StreamReader(str, System.Text.Encoding.UTF8);
                json_string = reader.ReadToEnd();
            }
            
            Console.WriteLine(json_string);

            return json_string;
        }

        [HttpGet]
        [Route("Races/{id}")]
        public string get_race(int id)
        {
            string json_string;
            HttpWebRequest web_req;
            HttpWebResponse web_resp;

            web_req = (HttpWebRequest) WebRequest.Create(string.Format("http://www.dnd5eapi.co/api/races/" + id.ToString()));
            web_req.Method = "GET";
            web_resp = (HttpWebResponse)web_req.GetResponse();
            
            Console.WriteLine(web_resp.StatusCode);

            using (Stream str = web_resp.GetResponseStream())
            {
                StreamReader reader = new StreamReader(str, Encoding.UTF8);
                json_string = reader.ReadToEnd();
            }
            
            Console.WriteLine(json_string);

            return json_string;
        }

        [HttpGet]
        [Route("Classes")]
        public string get_classes()
        {
            string json_string;
            HttpWebRequest web_req;
            HttpWebResponse web_resp;

            web_req = (HttpWebRequest) WebRequest.Create(string.Format("http://www.dnd5eapi.co/api/classes"));
            web_req.Method = "GET";
            web_resp = (HttpWebResponse) web_req.GetResponse();
            
            Console.WriteLine(web_resp.StatusCode);

            using (Stream str = web_resp.GetResponseStream())
            {
                StreamReader reader = new StreamReader(str, System.Text.Encoding.UTF8);
                json_string = reader.ReadToEnd();
            }
            
            Console.WriteLine(json_string);

            return json_string;
        }

        [HttpGet]
        [Route("Classes/{id}")]
        public string get_class(int id)
        {
            string json_string;
            HttpWebRequest web_req;
            HttpWebResponse web_resp;

            web_req = (HttpWebRequest) WebRequest.Create(
                string.Format("http://www.dnd5eapi.co/api/classes/" + id.ToString()));
            web_req.Method = "GET";
            web_resp = (HttpWebResponse) web_req.GetResponse();

            Console.WriteLine(web_resp.StatusCode);

            using (Stream str = web_resp.GetResponseStream())
            {
                StreamReader reader = new StreamReader(str, Encoding.UTF8);
                json_string = reader.ReadToEnd();
            }
            
            Console.WriteLine(json_string);

            return json_string;
        }
    }
}