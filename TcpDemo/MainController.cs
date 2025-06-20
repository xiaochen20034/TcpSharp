using System.Text;
using TcpSharp.Interface;

namespace TcpDemo;
using TcpSharp;
public class MainController
{
    private readonly TcpSharpSocketServer<PackageStruct> _server;
    private readonly TcpSharpSocketClient<PackageStruct> _client;
    private string _id = string.Empty;
    public MainController()
    {
        var packageController = new PackageController();
        _server = new TcpSharpSocketServer<PackageStruct>(3001,packageController);
        _client = new TcpSharpSocketClient<PackageStruct>("127.0.0.1", 3001,packageController);
        _server.OnConnected += (s, e) =>
        {
            Console.WriteLine("Server Connected!");
            _id = e.ConnectionId;
        };
        _server.OnDataReceived += (s, e) =>
        {
            Console.WriteLine($"DataLength:{e.Package.DataLength},Command:{e.Package.Command},Data:{e.Package.Data},Version:{e.Package.Version}");
        };
        _client.OnConnected += (s, e) =>
        {
            Console.WriteLine("Client Connected!");
        };
        _client.OnDataReceived += (s, e) =>
        {
            Console.WriteLine($"DataLength:{e.Package.DataLength},Command:{e.Package.Command},Data:{e.Package.Data},Version:{e.Package.Version}");
        };
        _client.OnError += (s, e) =>
        {
            Console.WriteLine(e.Exception);
        };
        _server.StartListening();
    }

    public void Invoke()
    {
        _client.Connect();
        while (true)
        {
            var sendStr = Console.ReadLine();
            
            if ("Exit".Equals(sendStr))
            {
                break;
            }
            var cmd = 5;

            var package = new PackageStruct()
            {
                DataLength = Encoding.ASCII.GetByteCount(sendStr),
                Command = cmd,
                Data = sendStr,
                Version = Guid.NewGuid().ToString()
            };
            _client.SendPackage(package);
        }
    }
}

public class PackageStruct
{
    public required int DataLength { get; init; }
    public required string Version { get; init; }
    public required int Command { get; init; }
    public required string Data { get; init; }
}

public class PackageController : IPackageController<PackageStruct>
{
    public int HeaderLength => 44;

    public PackageStruct Deserialize(byte[] data)
    {
        var dataLength = BitConverter.ToInt32(data.Take(4).ToArray(), 0);
        var cmd = BitConverter.ToInt32(data.Skip(4).Take(4).ToArray(), 0);
        var version = Encoding.UTF8.GetString(data.Skip(8).Take(36).ToArray());
        
        var str = Encoding.UTF8.GetString(data.Skip(HeaderLength).Take(dataLength).ToArray());
        var package = new PackageStruct()
        {
            DataLength = dataLength,
            Command = cmd,
            Data = str,
            Version = version
        };
        return package;
    }

    public int ReturnBodyLength(byte[] header)
    {
        var length = BitConverter.ToInt32(header.Take(4).ToArray(), 0);
        return length;
    }

    public byte[] Serialize(PackageStruct package)
    {
        List<byte> data = [];
        data.AddRange(BitConverter.GetBytes(package.DataLength));
        data.AddRange(BitConverter.GetBytes(package.Command));
        data.AddRange(Encoding.UTF8.GetBytes(package.Version));
        data.AddRange(Encoding.UTF8.GetBytes(package.Data));
        return data.ToArray();
    }
}