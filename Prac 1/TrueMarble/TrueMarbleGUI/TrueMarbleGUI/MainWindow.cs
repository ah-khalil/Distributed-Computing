using System;
using System.IO;
using System.Net.Sockets;
using System.ServiceModel;
using Gtk;
using TrueMarbleBiz;

public partial class MainWindow : Gtk.Window{
    int m_zoom, m_x, m_y;
    ITMBizController m_tm_biz;

    public MainWindow() : base(Gtk.WindowType.Toplevel){
        string s_url;

        NetTcpBinding tcp_binding;
        ChannelFactory<ITMBizController> tm_factory;

        tcp_binding = new NetTcpBinding();
        tcp_binding.MaxReceivedMessageSize = System.Int32.MaxValue;
        tcp_binding.ReaderQuotas.MaxArrayLength = System.Int32.MaxValue;

        s_url = "net.tcp://localhost:50002/TMBiz";
        tm_factory = new ChannelFactory<ITMBizController>(tcp_binding, s_url);

        try { 
            m_tm_biz = tm_factory.CreateChannel();
        }
        catch (SocketException) {
            show_dialog(this, "Server Error", "Could not connect to server");
            Console.WriteLine("Exiting....");
        }

        m_x = 0;
        m_y = 0;

        Build();

        m_zoom = (int)zoom_scale.Value;
        draw_map();
    }

    public void quit_application(object obj, EventArgs args) {
        Environment.Exit(1);
    }

    private void show_dialog(Window parent, String title, String message) {
        Button error_button;
        Dialog dialog = null;

        try {
            error_button = new Button("OK");
            error_button.Clicked += quit_application;

            dialog = new Dialog(title, parent, DialogFlags.DestroyWithParent | DialogFlags.Modal,  ResponseType.Ok);
            dialog.VBox.Add(new Label(message));
            dialog.VBox.Add(error_button);
            dialog.ShowAll();
            dialog.Run();
        } finally {
            if (dialog != null)
                dialog.Destroy();
        }
    }

    protected void OnDeleteEvent(object sender, DeleteEventArgs a){
        Application.Quit();
        a.RetVal = true;
    }

    protected void draw_map() {
        byte[] img_buff;

        try {
            img_buff = m_tm_biz.load_tile(m_zoom, m_x, m_y, out int code);
            img_view.Pixbuf = new Gdk.Pixbuf(img_buff);
        } catch (NullReferenceException) {
            show_dialog(this, "Loading Error", "The image failed to load");
        } catch (TimeoutException) {
            show_dialog(this, "Timeout Error", "The server timed-out");
        } catch (IOException) {
            show_dialog(this, "Connection Error", "Unable to communicate with server");
        } catch (Exception) {
            show_dialog(this, "Error", "An Error Occurred");
        }
    }

    protected int get_num_tiles_dir(String dir) {
        int ret_tile_num = -1;

        try { 
            switch (dir) {
                case "down":
                    ret_tile_num = m_tm_biz.get_num_tiles_down(m_zoom);
                    break;
                case "across":
                    ret_tile_num = m_tm_biz.get_num_tiles_across(m_zoom);
                    break;
                default:
                    break;
            }
        } catch (NullReferenceException) {
            show_dialog(this, "Loading Error", "The image failed to load");
        } catch (TimeoutException) {
            show_dialog(this, "Timeout Error", "The server timed-out");
        } catch (IOException) {
            show_dialog(this, "Connection Error", "Unable to communicate with server");
        } catch (Exception) {
            show_dialog(this, "Error", "An Error Occurred");
        }

        return ret_tile_num;
    }

    protected void on_y_inc_pressed(object sender, EventArgs e){
        int num_tiles_down;

        num_tiles_down = get_num_tiles_dir("down");

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

        num_tiles_across = get_num_tiles_dir("across");

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
        num_tiles_down = get_num_tiles_dir("down");
        num_tiles_across = get_num_tiles_dir("across");

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

    protected void verify(object sender, EventArgs e){
        try {
            if ((m_tm_biz != null) && !m_tm_biz.verify_tiles()){
                show_dialog(this, "Verification Error", "Tile verification failed");
            }
        }
        catch (TimeoutException) {
            show_dialog(this, "Timeout Error", "The server timed-out");
        }
    }
}
