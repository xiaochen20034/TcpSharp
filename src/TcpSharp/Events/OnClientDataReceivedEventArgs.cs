namespace TcpSharp;

public class OnClientDataReceivedEventArgs : EventArgs
{
    public byte[] Data { get; internal set; }
}

public class OnClientDataReceivedEventArgs<TPacketStruct> : EventArgs
{
    public TPacketStruct Packet { get; internal set; }
}
