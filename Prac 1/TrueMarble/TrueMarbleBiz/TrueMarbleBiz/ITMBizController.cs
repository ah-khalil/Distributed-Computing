using System.ServiceModel;

namespace TrueMarbleBiz
{
    [ServiceContract]
    public interface ITMBizController
    {
        [OperationContract]
        int get_tile_width();

        [OperationContract]
        int get_tile_height();

        [OperationContract]
        int get_num_tiles_across(int zoom);

        [OperationContract]
        int get_num_tiles_down(int zoom);

        [OperationContract]
        byte[] load_tile(int zoom, int x, int y, out int code);

        [OperationContract]
        bool verify_tiles();
    }
}
