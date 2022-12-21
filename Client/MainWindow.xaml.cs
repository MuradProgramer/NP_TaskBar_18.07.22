using Client.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using System.Windows.Data;
using System.Windows.Documents;

using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace Client;

public partial class MainWindow : Window
{
    private Dispatcher _dispatcher = Dispatcher.CurrentDispatcher;

    //public List<string>? ProcessesList { get; set; }

    public TcpClient client { get; set; } = new();

    public BinaryReader br { get; set; }
    public BinaryWriter bw { get; set; }

    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;
    }

    private void BorderClose_MouseDown(object sender, MouseButtonEventArgs e)
    {

        if (client.Connected)
        {
            var bw = new BinaryWriter(client.GetStream());
            bw.Write("connection closed");
        }
        
        try { Application.Current.Shutdown(); }
        catch (System.Exception) { }
    }

    private void BorderMenu_MouseDown(object sender, MouseButtonEventArgs e)
    {
        try { DragMove(); }
        catch (System.Exception) { }
    }

    private void ProcessesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ProcessTxtb.Text = (ProcessesListView.SelectedItem != null) ? ProcessesListView.SelectedItem.ToString() : null ;
    }

    private void ConnectBtn_Click(object sender, RoutedEventArgs e)
    {
        IPAddress ip = IPAddress.Parse(IpTxtb.Text.ToString());
        try
        {
            client.Connect(new IPEndPoint(ip, int.Parse(PortTxtb.Text)));
            MessageBox.Show("Connected Succesfuly");
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
        
    }

    private void RunBtn_Click(object sender, RoutedEventArgs e)
    {
        if (client.Connected)
        {
            //var text = Console.ReadLine();
            var stream = client.GetStream();
            var bw = new BinaryWriter(stream);
            var br = new BinaryReader(stream);

            //Console.WriteLine("~ message of getted response: {0}", br.ReadString());
            if (CommandCmbb.Text.ToLower() == "list")
            {
                bw.Write("list:list");
                var jsonString = br.ReadString();
                ProcessesListView.ItemsSource = JsonSerializer.Deserialize<List<string>>(jsonString);
            }
            else if (CommandCmbb.Text.ToLower() == "create")
            {
                bw.Write($"create:{ProcessTxtb.Text}");
                var jsonString = br.ReadString();
                ProcessesListView.ItemsSource = JsonSerializer.Deserialize<List<string>>(jsonString);
            }
            else if (CommandCmbb.Text.ToLower() == "kill")
            {
                bw.Write($"kill:{ProcessTxtb.Text}");
                var jsonString = br.ReadString();
                ProcessesListView.ItemsSource = JsonSerializer.Deserialize<List<string>>(jsonString);
            }
        }
    }

    private void CommandCmbb_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ProcessTxtb.IsEnabled = true;
        ProcessTxtb.Text = "";

        if (CommandCmbb.SelectedIndex == 2 || CommandCmbb.SelectedIndex == 0)
        {
            ProcessTxtb.IsEnabled = false;
        }
    }
}
