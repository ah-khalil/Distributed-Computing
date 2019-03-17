using System;
using System.ServiceModel;
using Gtk;
using TrueMarbleData;

public partial class MainWindow : Gtk.Window{
    int m_zoom, m_x, m_y;
    ITMDataController m_tm_data;

    public MainWindow() : base(Gtk.WindowType.Toplevel){
        string s_url;
        NetTcpBinding tcp_binding;
        ChannelFactory<ITMDataController> tm_factory;

        tcp_binding = new NetTcpBinding();
        tcp_binding.MaxReceivedMessageSize = System.Int32.MaxValue;
        tcp_binding.ReaderQuotas.MaxArrayLength = System.Int32.MaxValue;

        s_url = "net.tcp://localhost:50001/TMData";
        tm_factory = new ChannelFactory<ITMDataController>(tcp_binding, s_url);
        m_tm_data = tm_factory.CreateChannel();

        m_x = 0;
        m_y = 0;

        Build();

        m_zoom = (int)zoom_scale.Value;
        draw_map();
    }

    protected void OnDeleteEvent(object sender, DeleteEventArgs a){
        Application.Quit();
        a.RetVal = true;
    }

    protected void draw_map() {
        byte[] img_buff;

        img_buff = m_tm_data.load_tile(m_zoom, m_x, m_y);
        img_view.Pixbuf = new Gdk.Pixbuf(img_buff);
    }

    protected void on_y_inc_pressed(object sender, EventArgs e){
        int num_tiles_down;

        num_tiles_down = m_tm_data.get_num_tiles_down(m_zoom);

        if (m_y < num_tiles_down - 1) {
            Console.WriteLine("Y increased:\n\t" + m_y + "->" + (m_y + 1) + "\n\t" + num_tiles_down);

            m_y++;
            draw_map();
        }
    }

    protected void on_y_dec_clicked(object sender, EventArgs e){
        if(m_y >= 0) {
            Console.WriteLine("Y decreased");

            m_y = (m_y == 0) ? m_y : m_y - 1;
            draw_map();
        }
    }

    protected void on_x_inc_clicked(object sender, EventArgs e){
        int num_tiles_across;

        num_tiles_across = m_tm_data.get_num_tiles_across(m_zoom);

        if (m_x < num_tiles_across - 1) {
            Console.WriteLine("Y increased:\n\t" + m_x + "->" + (m_x + 1) + "\n\t" + num_tiles_across);

            m_x++;
            draw_map();
        }
        Console.WriteLine("X increased");

    }

    protected void on_x_dec_clicked(object sender, EventArgs e){
        if(m_x >= 0) {
            Console.WriteLine("X decreased");

            m_x = (m_x == 0) ? m_x : m_x - 1;
            draw_map();
        }
    }

    protected void on_zoom_changed(object o, EventArgs args){
        int num_tiles_across, num_tiles_down;

        m_zoom = (int)zoom_scale.Value;
        num_tiles_down = m_tm_data.get_num_tiles_down(m_zoom);
        num_tiles_across = m_tm_data.get_num_tiles_across(m_zoom);

        Console.WriteLine("Zoom Changed: \n\t" + m_zoom);
        Console.WriteLine("Tiles Down: " + num_tiles_down);
        Console.WriteLine("Tiles Across: " + num_tiles_across);

        if (m_x > num_tiles_across - 1)
            m_x = (num_tiles_across - 1) / 2;
        else if (m_x < 0)
            m_x = 0;

        if (m_y > num_tiles_down - 1)
            m_y = (num_tiles_down - 1) / 2;
        else if (m_y < 0)
            m_y = 0;

        draw_map();
    }
}
