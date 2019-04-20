using System;
using System.Web.Http;
using System.Collections.Generic;

namespace TMWeb.Controllers
{
    public class DummyController : ApiController
    {

        /* Fuction: getImage
         * Type: Get
         * Parameters: zoom level, x coordinate, y coordinate
         * Return: byte[]
         * Assertion:  function returns a dummy image, regardless of input.
         */
        [HttpGet]
        [Route("TMWeb/{zoom}/{x}/{y}")]
        public byte[] Get(int zoom, int x, int y)
        {
            byte[] imageBuf = System.IO.File.ReadAllBytes("Resources/LoremIpsum.jpg");
            return imageBuf;
        }

        /* Fuction: getImage
        * Type: Post
        * Parameters: zoom level, x coordinate, y coordinate
        * Return: byte[]
        * Assertion: function returns a dummy image, regardless of input.
        * Note: Post version of the previous method.
        */
        [HttpPost]
        [Route("TMWeb")]
        public byte[] Post([FromBody]Dictionary<string, object> req)
        {
            byte[] imageBuf = System.IO.File.ReadAllBytes("Resources/LoremIpsum.jpg");
            return imageBuf;
        }

        /* Fuction: zoom
         * Type: Get
         * Parameters: -
         * Return: the number of zoom levels
         */
        [HttpGet]
        [Route("TMWeb/Zoom")]
        public int Get()
        {
            return 6;
        }

        /* Fuction: zoom
         * Type: Get
         * Parameters: zoom level
         * Return: zoom: the
         *         across:
         *         down:
         * function: will validate the zoom, and return the
         *           number of tiles across and down
         */
        [HttpGet]
        [Route("TMWeb/zoom/{Zoom}")]
        public Dictionary<string, int> Get(int zoom)
        {
            Dictionary<string, int> ret = new Dictionary<string, int>
            {
                { "across", 1 },
                { "down", 1 }
            };

            return ret;
        }
    }
}
