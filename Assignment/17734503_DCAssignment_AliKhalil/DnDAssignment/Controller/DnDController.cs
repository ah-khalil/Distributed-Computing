using System;
using System.IO;
using System.Net;
using System.Web.Http;
using System.Net.Http;
using System.Text;
using System.Web;
using Newtonsoft.Json.Linq;

namespace DnDAssignment.Controllers
{
    [RoutePrefix("DnD/DnDAssignment")]
    public class DnDController : ApiController
    {

        /*
         * Purpose: Gets list of races from DnD5e Api
         * Params: None
         * Return: JSON string representing all races retrieved
         * Method: GET
         */
        [HttpGet]
        [Route("Races")]
        public string get_races()
        {
            string json_string = "";
            HttpWebRequest web_req;
            HttpWebResponse web_resp;

            try
            {
                /*Open request to dnd5eapi to get the races. Uses a GET method.*/
                web_req = (HttpWebRequest)WebRequest.Create(string.Format("http://www.dnd5eapi.co/api/races"));
                web_req.Method = "GET";
                web_resp = (HttpWebResponse)web_req.GetResponse();

                /*Read the response stream into the json_string variable using UTF8 encoding*/
                using (Stream str = web_resp.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(str, System.Text.Encoding.UTF8);
                    json_string = reader.ReadToEnd();
                }

                /*Encode the string so that insecure HTML/javascript is not run when client receives the JSON string*/
                json_string = HttpUtility.HtmlEncode(json_string);
            }
            catch (WebException)
            {
                /*Throw a code 500 error*/
                throw new HttpResponseException(this.Request.CreateResponse<object>(
                    System.Net.HttpStatusCode.InternalServerError, "An error occurred communicating with the DnD5e Api"));
            }
            catch (Exception)
            {
                /*Throw a code 500 error*/
                throw new HttpResponseException(this.Request.CreateResponse<object>(
                    System.Net.HttpStatusCode.InternalServerError, "An error occurred on the server"));
            }

            return json_string;
        }

        /*
         * Purpose: Gets specific race using its ID
         * Params: id - int
         * Return: JSON string representing the retrieved race
         * Method: GET
         */
        [HttpGet]
        [Route("Races/{id}")]
        public string get_race(int id)
        {
            string json_string = "";
            HttpWebRequest web_req;
            HttpWebResponse web_resp;

            try
            {
                /*Open request to dnd5eapi to get the race. Uses a GET method.*/
                web_req = (HttpWebRequest)WebRequest.Create(
                    string.Format("http://www.dnd5eapi.co/api/races/" + id.ToString()));
                web_req.Method = "GET";
                web_resp = (HttpWebResponse)web_req.GetResponse();

                /*Read the response stream into the json_string variable using UTF8 encoding*/
                using (Stream str = web_resp.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(str, Encoding.UTF8);
                    json_string = reader.ReadToEnd();
                }

                /*Encode the string so that insecure HTML/javascript is not run when client receives the JSON string*/
                json_string = HttpUtility.HtmlEncode(json_string);
            }
            catch (WebException)
            {
                /*Throw a code 500 error*/
                throw new HttpResponseException(this.Request.CreateResponse<object>(
                    System.Net.HttpStatusCode.InternalServerError, "An error occurred communicating with the DnD5e Api"));
            }
            catch (Exception)
            {
                /*Throw a code 500 error*/
                throw new HttpResponseException(this.Request.CreateResponse<object>(System.Net.HttpStatusCode.InternalServerError, "An error occurred on the server"));
            }

            return json_string;
        }

        /*
         * Purpose: Gets list of classes from DnD5e Api
         * Params: None
         * Return: JSON string representing all classes retrieved
         * Method: GET
         */
        [HttpGet]
        [Route("Classes")]
        public string get_classes()
        {
            string json_string = "";
            HttpWebRequest web_req;
            HttpWebResponse web_resp;

            try
            {
                /*Open request to dnd5eapi to get the classes. Uses a GET method.*/
                web_req = (HttpWebRequest)WebRequest.Create(string.Format("http://www.dnd5eapi.co/api/classes"));
                web_req.Method = "GET";
                web_resp = (HttpWebResponse)web_req.GetResponse();

                /*Read the response stream into the json_string variable using UTF8 encoding*/
                using (Stream str = web_resp.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(str, System.Text.Encoding.UTF8);
                    json_string = reader.ReadToEnd();
                }

                /*Encode the string so that insecure HTML/javascript is not run when client receives the JSON string*/
                json_string = HttpUtility.HtmlEncode(json_string);
            }
            catch (WebException)
            {
                /*Throw a code 500 error*/
                throw new HttpResponseException(this.Request.CreateResponse<object>(
                    System.Net.HttpStatusCode.InternalServerError, "An error occurred communicating with the DnD5e Api"));
            }
            catch (Exception)
            {
                /*Throw a code 500 error*/
                throw new HttpResponseException(this.Request.CreateResponse<object>(System.Net.HttpStatusCode.InternalServerError, "An error occurred on the server"));
            }

            return json_string;
        }

        /*
         * Purpose: Gets specific class using its ID
         * Params: id - int
         * Return: JSON string representing the retrieved class
         * Method: GET
         */
        [HttpGet]
        [Route("Classes/{id}")]
        public string get_class(int id)
        {
            HttpWebRequest web_req;
            HttpWebResponse web_resp;
            string json_string = "";

            try
            {
                /*Open request to dnd5eapi to get the class. Uses a GET method.*/
                web_req = (HttpWebRequest)WebRequest.Create(string.Format("http://www.dnd5eapi.co/api/classes/" + id.ToString()));
                web_req.Method = "GET";
                web_resp = (HttpWebResponse)web_req.GetResponse();
                
                /*Read the response stream into the json_string variable using UTF8 encoding*/
                using (Stream str = web_resp.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(str, Encoding.UTF8);
                    json_string = reader.ReadToEnd();
                }

                /*Encode the string so that insecure HTML/javascript is not run when client receives the JSON string*/
                json_string = HttpUtility.HtmlEncode(json_string);
            }
            catch (WebException)
            {
                /*Throw a code 500 error*/
                throw new HttpResponseException(this.Request.CreateResponse<object>(
                    System.Net.HttpStatusCode.InternalServerError, "An error occurred communicating with the DnD5e Api"));
            }
            catch (Exception)
            {
                /*Throw a code 500 error*/
                throw new HttpResponseException(this.Request.CreateResponse<object>(System.Net.HttpStatusCode.InternalServerError, "An error occurred on the server"));
            }

            return json_string;
        }

        /*
         * Purpose: Gets list of ability types from DnD5e Api
         * Params: None
         * Return: JSON string representing all ability types retrieved
         * Method: GET
         */
        [HttpGet]
        [Route("AbilityTypes")]
        public string get_ability_types()
        {
            HttpWebRequest web_req;
            HttpWebResponse web_resp;
            string json_string = "";

            try
            {
                /*Open request to dnd5eapi to get the ability types. Uses a GET method.*/
                web_req = (HttpWebRequest)WebRequest.Create(string.Format("http://www.dnd5eapi.co/api/ability-scores"));
                web_req.Method = "GET";
                web_resp = (HttpWebResponse)web_req.GetResponse();

                /*Read the response stream into the json_string variable using UTF8 encoding*/
                using (Stream str = web_resp.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(str, Encoding.UTF8);
                    json_string = reader.ReadToEnd();
                }

                /*Encode the string so that insecure HTML/javascript is not run when client receives the JSON string*/
                json_string = HttpUtility.HtmlEncode(json_string);
            }
            catch (WebException)
            {
                /*Throw a code 500 error*/
                throw new HttpResponseException(this.Request.CreateResponse<object>(
                    System.Net.HttpStatusCode.InternalServerError, "An error occurred communicating with the DnD5e Api"));
            }
            catch (Exception)
            {
                /*Throw a code 500 error*/
                throw new HttpResponseException(this.Request.CreateResponse<object>(System.Net.HttpStatusCode.InternalServerError, "An error occurred on the server"));
            }

            return json_string;
        }
    }
}