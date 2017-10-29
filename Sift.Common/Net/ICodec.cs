using System.IO;

namespace Sift.Common.Net
{
    public interface ICodec
    {
        void Decode(BinaryReader reader);
        void Encode(BinaryWriter writer);
    }
}
