namespace TcpSharp;

public class OnClientDataReceivedEventArgs : EventArgs
{
    public byte[] Data { get; internal set; }
}

public class OnClientDataReceivedEventArgs<TPackageStruct> : EventArgs
{
    public TPackageStruct Package { get; internal set; }
}
