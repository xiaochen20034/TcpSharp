namespace TcpSharp;

public class OnServerDataReceivedEventArgs : EventArgs
{
    public TcpClient Client { get; internal set; }
    public string ConnectionId { get; internal set; }
    public byte[] Data { get; internal set; }
}

public class OnServerDataReceivedEventArgs<TPackageStruct> : EventArgs
{
    public TcpClient Client { get; internal set; }
    public string ConnectionId { get; internal set; }
    public TPackageStruct Packet { get; internal set; }
}
