using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tdf
{
    public interface ITdfDecoder
    {
        T Decode<T>(byte[] data) where T : notnull;
        T Decode<T>(Stream stream) where T : notnull;
        object Decode(Type type, byte[] data);
        object Decode(Type type, Stream stream);
    }
}
