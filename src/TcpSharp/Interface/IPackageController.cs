namespace TcpSharp.Interface;

public interface IPacketController<TPacketStruct>
{
   public int HeaderLength { get; }
   public int ReturnBodyLength(byte[] header);
   public TPacketStruct Deserialize(byte[] data);
   public byte[] Serialize(TPacketStruct packet);
}