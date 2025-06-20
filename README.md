# TcpSharp
Original Project: https://github.com/burakoner/TcpSharp
## What's new in 1.3.9
### Added support for packet.
This means you can now define your own packet formats and parsing rules, and transmit data in a packet-based mode. The component will handle issues such as packet sticking on its own.


First, define your packet structure:
```csharp
public class PacketStruct
{
    public required int DataLength { get; init; }
    public required string Version { get; init; }
    public required int Command { get; init; }
    public required string Data { get; init; }
}
```


Then implement the packet controller:
```csharp
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
```


Now,you can use them like this:
```csharp
var packetController = new PacketController();
var server = new TcpSharpSocketServer<PacketStruct>(3001,packetController);
var client = new TcpSharpSocketClient<PacketStruct>("127.0.0.1", 3001,packetController);
//other init......
var str = "Hello World";
var packet = new PacketStruct()
{
    DataLength = Encoding.UTF8.GetByteCount(str),
    Command = 1,
    Data = str,
    Version = Guid.NewGuid().ToString()
};
client.SendPacket(packet);
```

## How to Download
- This package is available on NuGet as "Xyli.TcpSharp", not the original "TcpSharp".
