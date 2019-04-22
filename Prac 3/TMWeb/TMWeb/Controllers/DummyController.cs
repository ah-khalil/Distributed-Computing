using System;
using System.Web.Http;
using TrueMarbleLibrary;
//using System.Web.Http.Cors;
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
            return load_tile(zoom, x, y, out int code);
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
            byte[] imageBuf = System.IO.File.ReadAllBytes("Resources/LoremIpsum.jpeg");
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
                { "across", get_num_tiles_across(zoom) },
                { "down", get_num_tiles_down(zoom) }
            };

            return ret;
        }

        public int get_num_tiles_across(int zoom)
        {
            TrueMarble.GetNumTiles(zoom, out int num_tile_x, out int num_tile_y);

            return num_tile_x;
        }

        public int get_num_tiles_down(int zoom)
        {
            TrueMarble.GetNumTiles(zoom, out int num_tile_x, out int num_tile_y);

            return num_tile_y;
        }

        public byte[] load_tile(int zoom, int x, int y, out int code)
        {
            int buff_size;
            byte[] byte_buff;

            buff_size = x * y * 3;
            byte_buff = new byte[buff_size];
            code = TrueMarble.GetTileImageAsRawJPG(zoom, x, y, out byte_buff, buff_size, out int jpg_size);

            return byte_buff;
        }
    }
}
