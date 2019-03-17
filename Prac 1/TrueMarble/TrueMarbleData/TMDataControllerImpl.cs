using System;
using TrueMarbleLibrary;
using System.ServiceModel;

namespace TrueMarbleData
{
    [ServiceBehavior(ConcurrencyMode=ConcurrencyMode.Multiple, UseSynchronizationContext=false, InstanceContextMode=InstanceContextMode.Single)]
    internal class TMDataControllerImpl : ITMDataController
    {
        public TMDataControllerImpl()
        {
            Console.WriteLine("A connection was made!");
        }

        ~TMDataControllerImpl() {
            Console.WriteLine("Disconnected...");
        }

        public int get_tile_width() {
            TrueMarble.GetTileSize(out int width, out int height);

            return width;
        }

        public int get_tile_height() {
            TrueMarble.GetTileSize(out int width, out int height);

            return height;
        }

        public int get_num_tiles_across(int zoom) {
            TrueMarble.GetNumTiles(zoom, out int num_tile_x, out int num_tile_y);

            return num_tile_x;
        }

        public int get_num_tiles_down(int zoom) {
            TrueMarble.GetNumTiles(zoom, out int num_tile_x, out int num_tile_y);

            return num_tile_y;
        }

        public byte[] load_tile(int zoom, int x, int y, out int code) {
            int buff_size;
            byte[] byte_buff;

            buff_size = x * y * 3;
            byte_buff = new byte[buff_size];
            code = TrueMarble.GetTileImageAsRawJPG(zoom, x, y, out byte_buff, buff_size, out int jpg_size);

            return byte_buff;
        }
    }
}
