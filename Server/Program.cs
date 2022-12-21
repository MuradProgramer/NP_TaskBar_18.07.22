using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Timers;
using System.Diagnostics;
using Server.Services;
using static System.Net.Mime.MediaTypeNames;
using System.IO;
using System.Text.Json;

TcpListener listener;

BinaryReader br;
BinaryWriter bw;

var ip = IPAddress.Parse("192.168.0.113");
var ep = new IPEndPoint(ip, 27001);
listener = new TcpListener(ep);
listener.Start();
Console.WriteLine($"---------------------------------------");
Console.WriteLine($"Listening on {listener.LocalEndpoint}");
Console.WriteLine($"---------------------------------------\n");
while (true)
{
    var client = listener.AcceptTcpClient();

    Task.Run(() =>
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"{client.Client.RemoteEndPoint} connected...\n");
        NetworkStream stream = client.GetStream();
        br = new(stream);
        bw = new(stream);
        while (true)
        {
            var msg = br.ReadString();

            if (msg.Contains("connection closed"))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"~~~ user {client.Client.RemoteEndPoint} disconnected ~~~\n");
            }

            if (msg.Split(":")[0].ToLower() == "create")
            {
                Process.Start(msg.Split(":")[1]);
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine($"--- {msg.Split(":")[1]} Proccess Started ---\n");
            }

            else if (msg.Split(":")[0].ToLower() == "kill")
            {
                Task.Run(() => Process.GetProcessesByName(msg.Split(":")[1])[0].Kill());
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine($"--- {msg.Split(":")[1]} Proccess Killed ---\n");
            }

            List<string> processesList = Process.GetProcesses().ToList().Select(p => p.ProcessName).ToList();
            string jsonString = JsonSerializer.Serialize(processesList);

            bw.Write(jsonString);

            //else if (msg.Split(":")[0].ToLower() == "list")
            //{
            //    List<string> processesList = Process.GetProcesses().ToList().Select(p => p.ProcessName).ToList();
            //    string jsonString = JsonSerializer.Serialize(processesList);

            //    bw.Write(jsonString);
            //}

            //Console.WriteLine($"Command Name: {msg.Split(':')[0]}  |  Proccess Name: {msg.Split(':')[1]}");
            //bw.Write("madafaka");
        }
    });
}

    //Task.Run(() =>
    //{
    //    client1 = listener.AcceptTcpClient();
    //    Task.Run(() =>
    //    {
    //        var st = client1.GetStream();
    //        var bytes = new byte[1024];
    //        while (true)
    //        {
    //            st.Read(bytes, 0, bytes.Length);
    //            var message = Encoding.Default.GetString(bytes);
    //            if (message.Contains("connection closed"))
    //            {
    //                counter--;
    //                break;
    //            }
    //        }

    //    });
    //    counter++;

    //});

    //Task.Run(() =>
    //{
    //    while (true)
    //    {
    //        if (counter == 0) continue;

    //        var client = new UdpClient();
    //        var address = IPAddress.Parse(client1.Client.RemoteEndPoint.ToString().Split(':')[0]);
    //        var connectEndPoint = new IPEndPoint(address, 27002);
    //        try
    //        {

    //            var bytes = client.Receive(ref connectEndPoint);
    //            var msg = Encoding.UTF8.GetString(bytes);

    //            if (msg.Split(":")[0].ToUpper() == "CREATE")
    //                Process.Start(msg.Split(":")[1]);

    //            else if (msg.Split(":")[0].ToUpper() == "KILL")
    //                Process.GetProcessesByName(msg.Split(":")[1])[0].Kill();

    //            else if (msg.Split(":")[0].ToUpper() == "LIST")
    //            {
    //                List<string> processesList = Process.GetProcesses().ToList().Select(p => p.ProcessName).ToList();
    //                Service.JsonSerialize(processesList, "ProcessesList");

    //                var text = Service.GetJSONString("ProcessesList");
    //                var bytes1 = Encoding.UTF8.GetBytes(text);

    //                client.Send(bytes1, bytes1.Length, connectEndPoint);
    //            }
    //        }
    //        catch (Exception ex) { Console.WriteLine(ex.Message); }
    //        //client.Send(bytes, bytes.Length, connectEndPoint);
    //    }
    //});