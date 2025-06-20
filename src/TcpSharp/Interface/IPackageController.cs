namespace TcpSharp.Interface;

public interface IPackageController<TPackageStruct>
{
   public int HeaderLength { get; }
   public int ReturnBodyLength(byte[] header);
   public TPackageStruct Deserialize(byte[] data);
   public byte[] Serialize(TPackageStruct package);
}