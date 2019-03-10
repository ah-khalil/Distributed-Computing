using System;
using System.ServiceModel;
using Gtk;
using TrueMarbleData;

public partial class MainWindow : Gtk.Window
{
    int m_zoom, m_x, m_y;
    ITMDataController m_tm_data;

    public MainWindow() : base(Gtk.WindowType.Toplevel)
    {
        string s_url;
        NetTcpBinding tcp_binding;
        ChannelFactory<ITMDataController> tm_factory;

        tcp_binding = new NetTcpBinding();
        s_url = "net.tcp://localhost:50001/TMData";
        tm_factory = new ChannelFactory<ITMDataController>(tcp_binding, s_url);
        m_tm_data = tm_factory.CreateChannel();

        m_x = 0;
        m_y = 0;

        Build();

        m_zoom = (int)zoom_scale.Value;
        draw_map();
    }

    protected void OnDeleteEvent(object sender, DeleteEventArgs a)
    {
        Application.Quit();
        a.RetVal = true;
    }

    protected void draw_map() {
        byte[] img_buff;

        img_buff = m_tm_data.load_tile(m_zoom, m_x, m_y);
        img_view.Pixbuf = new Gdk.Pixbuf(img_buff);
    }

    protected void on_y_inc_pressed(object sender, EventArgs e){
        m_y++;

        draw_map();
    }

    protected void on_y_dec_clicked(object sender, EventArgs e)
    {
        m_y--;

        draw_map();
    }

    protected void on_x_inc_clicked(object sender, EventArgs e){
        m_x++;

        draw_map();
    }

    protected void on_x_dec_clicked(object sender, EventArgs e){
        m_x--;

        draw_map();
    }

    protected void on_zoom_changed(object o, MoveSliderArgs args){
        m_zoom = (int)zoom_scale.Value;

        draw_map();
    }
}
