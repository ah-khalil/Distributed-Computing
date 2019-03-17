using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.ServiceModel;
using TrueMarbleData;

namespace TrueMarbleBiz
{
    [ServiceBehavior(ConcurrencyMode=ConcurrencyMode.Multiple, UseSynchronizationContext=false, InstanceContextMode=InstanceContextMode.Single)]
    internal class TMBizControllerImpl : ITMBizController
    {
        const int ZOOM_LEVEL_MAX = 6;
        ITMDataController m_tm_data;

        private struct Coords {
            private int x, y, zoom_level;

            public Coords(int x, int y, int zoom_l) {
                this.x = x;
                this.y = y;
                this.zoom_level = zoom_l;
            }

            public String to_string() => "At Zoom Level: " + zoom_level + "\n\tX: " + x + "\n\tY: " + y;
        }

        public TMBizControllerImpl()
        {
            string s_url;

            NetTcpBinding tcp_binding;
            ChannelFactory<ITMDataController> tm_factory;

            tcp_binding = new NetTcpBinding();
            tcp_binding.MaxReceivedMessageSize = System.Int32.MaxValue;
            tcp_binding.ReaderQuotas.MaxArrayLength = System.Int32.MaxValue;

            s_url = "net.tcp://localhost:50001/TMData";
            tm_factory = new ChannelFactory<ITMDataController>(tcp_binding, s_url);

            try
            {
                m_tm_data = tm_factory.CreateChannel();
            }
            catch (SocketException)
            {
                //show_dialog(this, "Server Error", "Could not connect to server");
                Console.WriteLine("Exiting....");
            }
        }

        public int get_tile_width()
        {
            int tile_width;

            tile_width = m_tm_data.get_tile_width();

            return tile_width;
        }

        public int get_tile_height()
        {
            int tile_height;

            tile_height = m_tm_data.get_tile_height();

            return tile_height;
        }

        public int get_num_tiles_across(int zoom)
        {
            int num_tiles_across;

            num_tiles_across = m_tm_data.get_num_tiles_across(zoom);

            return num_tiles_across;
        }

        public int get_num_tiles_down(int zoom)
        {
            int num_tiles_down;

            num_tiles_down = m_tm_data.get_num_tiles_down(zoom);

            return num_tiles_down;
        }

        public byte[] load_tile(int zoom, int x, int y, out int code)
        {
            byte[] img_buff = null;
            code = 0;

            try {
                img_buff = m_tm_data.load_tile(zoom, x, y, out code);
            } catch (Exception) {
                //Do Something
            }

            return img_buff;
        }

        public bool verify_tiles() {
            List<Coords> invalid_coord_list;
            int num_tiles_down, num_tiles_across;

            invalid_coord_list = new List<Coords>();

            Console.WriteLine("Nigger");

            for (int i = 0; i < ZOOM_LEVEL_MAX; i++) {
                num_tiles_down = get_num_tiles_down(i);
                num_tiles_across = get_num_tiles_across(i);

                for (int x = 0; i < num_tiles_across; x++) {
                    for (int y = 0; y < num_tiles_down; y++) {
                        try {
                            load_tile(i, x, y, out int code);
                        
                            if (code == 0) {
                                invalid_coord_list.Add(new Coords(x, y, i));
                            } else
                                Console.WriteLine("Verified -> X: " + x + "Y: " + y);
                        }
                        catch (Exception) { 
                            //Do Something
                        }
                    }
                }
            }

            Console.WriteLine("Invalid Coordinates: ");

            foreach (Coords coord in invalid_coord_list) {
                Console.WriteLine(coord.to_string());
            }

            return !(invalid_coord_list.Count > 0);
        }
    }
}
