using System.Text;
using TcpSharp.Interface;

namespace TcpDemo;
using TcpSharp;
public class MainController
{
    private readonly TcpSharpSocketServer<PacketStruct> _server;
    private readonly TcpSharpSocketClient<PacketStruct> _client;
    private string _id = string.Empty;
    public MainController()
    {
        var packetController = new PacketController();
        _server = new TcpSharpSocketServer<PacketStruct>(3001,packetController);
        _client = new TcpSharpSocketClient<PacketStruct>("127.0.0.1", 3001,packetController);
        _server.OnConnected += (s, e) =>
        {
            Console.WriteLine("Server Connected!");
            _id = e.ConnectionId;
        };
        _server.OnDataReceived += (s, e) =>
        {
            Console.WriteLine($"DataLength:{e.Packet.DataLength},Command:{e.Packet.Command},Data:{e.Packet.Data},Version:{e.Packet.Version}");
        };
        _client.OnConnected += (s, e) =>
        {
            Console.WriteLine("Client Connected!");
        };
        _client.OnDataReceived += (s, e) =>
        {
            Console.WriteLine($"DataLength:{e.Packet.DataLength},Command:{e.Packet.Command},Data:{e.Packet.Data},Version:{e.Packet.Version}");
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

            var packet = new PacketStruct()
            {
                DataLength = Encoding.UTF8.GetByteCount(sendStr),
                Command = cmd,
                Data = sendStr,
                Version = Guid.NewGuid().ToString()
            };
            _client.SendPacket(packet);
        }
    }
}

public class PacketStruct
{
    public required int DataLength { get; init; }
    public required string Version { get; init; }
    public required int Command { get; init; }
    public required string Data { get; init; }
}

public class PacketController : IPacketController<PacketStruct>
{
    public int HeaderLength => 44;

    public PacketStruct Deserialize(byte[] data)
    {
        var dataLength = BitConverter.ToInt32(data.Take(4).ToArray(), 0);
        var cmd = BitConverter.ToInt32(data.Skip(4).Take(4).ToArray(), 0);
        var version = Encoding.UTF8.GetString(data.Skip(8).Take(36).ToArray());
        
        var str = Encoding.UTF8.GetString(data.Skip(HeaderLength).Take(dataLength).ToArray());
        var packet = new PacketStruct()
        {
            DataLength = dataLength,
            Command = cmd,
            Data = str,
            Version = version
        };
        return packet;
    }

    public int ReturnBodyLength(byte[] header)
    {
        var length = BitConverter.ToInt32(header.Take(4).ToArray(), 0);
        return length;
    }

    public byte[] Serialize(PacketStruct packet)
    {
        List<byte> data = [];
        data.AddRange(BitConverter.GetBytes(packet.DataLength));
        data.AddRange(BitConverter.GetBytes(packet.Command));
        data.AddRange(Encoding.UTF8.GetBytes(packet.Version));
        data.AddRange(Encoding.UTF8.GetBytes(packet.Data));
        return data.ToArray();
    }
}