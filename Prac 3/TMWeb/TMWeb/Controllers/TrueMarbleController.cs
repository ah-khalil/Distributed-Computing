using System;
using System.IO;
using System.Web.Http;
using System.Net.Http;
using System.Collections.Generic;
//using TrueMarbleLibrary;

namespace TMWeb.Controllers
{
    public class TrueMarbleController : ApiController
    {
        /* Fuction: getImausing TrueMarbleLibrary;
ge
        * Type: Post
        * Parameters: zoom level, x coordinate, y coordinate
        * Return: byte[]
        * Assertion: function returns a dummy image, regardless of input.
        * Note: Post version of the previous method.
        */
//        [HttpPost]
//        [Route("View")]
//        public byte[] Post()
//        {
//            byte[] ret_arr;
//            int x_val, y_val, zoom_level;
//            Dictionary<string, object> req;
//
//            try {
//                req = (Dictionary<string, object>)Request.Content.ReadAsAsync<Dictionary<string, object>>().Result;
//                x_val = Int32.Parse(req["x_val"].ToString());
//                y_val = Int32.Parse(req["y_val"].ToString());
//                zoom_level = Int32.Parse(req["zoom_level"].ToString());
//
//                ret_arr = load_tile(zoom_level, x_val, y_val, out int code);
//
//                if (code != 1)
//                    throw new Exception();
//            }
//            catch (Exception e) {
//                throw new HttpResponseException(this.Request.CreateResponse<object>(System.Net.HttpStatusCode.InternalServerError, "Error occurred : " + e.Message));
//            }
//
//            return ret_arr;
//        }

        /* Fuction: zoom
         * Type: Get
         * Parameters: zoom level
         * Return: zoom: the
         *         across:
         *         down:
         * function: will validate the zoom, and return the
         *           number of tiles across and down
         */
//        [HttpGet]
//        [Route("TMWeb/ZoomMax/{zoom}")]
//        public Dictionary<string, int> Get(int zoom)
//        {
//            Dictionary<string, int> ret;
//
//            try {
//                ret = new Dictionary<string, int>{
//                    { "across", get_num_tiles_across(zoom) },
//                    { "down", get_num_tiles_down(zoom) }
//                };
//            }
//            catch (Exception e) {
//                throw new HttpResponseException(this.Request.CreateResponse<object>(System.Net.HttpStatusCode.InternalServerError, "Error occurred : " + e.Message));
//            }
//
//            return ret;
//        }

        /* Function: getMaxZoomLevel
         * Type: Get
         * Parameter: nil
         * Return: int
         * function: will return the maximum level of zoom,
         *           hard-coded as 6
         * 
         */
         [HttpGet]
         [Route("TMWeb/Zoom")]
         public int Get() {
            return 6;
         }

//        private int get_num_tiles_across(int zoom)
//        {
//            TrueMarble.GetNumTiles(zoom, out int num_tile_x, out int num_tile_y);
//
//            return num_tile_x;
//        }
//
//        private int get_num_tiles_down(int zoom)
//        {
//            TrueMarble.GetNumTiles(zoom, out int num_tile_x, out int num_tile_y);
//
//            return num_tile_y;
//        }
//
//        private byte[] load_tile(int zoom, int x, int y, out int code)
//        {
//            int buff_size;
//            byte[] byte_buff;
//
//            buff_size = x * y * 3;
//            byte_buff = new byte[buff_size];
//            code = TrueMarble.GetTileImageAsRawJPG(zoom, x, y, out byte_buff, buff_size, out int jpg_size);
//
//            return byte_buff;
//        }
    }
}
