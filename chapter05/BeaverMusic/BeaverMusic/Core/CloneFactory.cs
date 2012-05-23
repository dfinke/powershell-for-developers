using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace BeaverMusic.Core
{
    public static class CloneFactory
    {
        public static T DeepClone<T>(T obj)
        {
            using (var stream = new MemoryStream(1024))
            {
                var serializer = new BinaryFormatter();
                serializer.Serialize(stream, obj);

                stream.Seek(0, SeekOrigin.Begin);

                return (T)serializer.Deserialize(stream);
            }
        }
    }
}
