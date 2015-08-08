using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OneCog.Io.Onkyo
{
    public static class Extensions
    {
        public static Stream AsStream(this byte[] buffer)
        {
            return new MemoryStream(buffer);
        }

        public static Stream AsStream(this IEnumerable<byte> buffer)
        {
            return new MemoryStream(buffer.ToArray());
        }
    }
}
