using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace HSVStudio.Tutorial
{
    public static class HSVExtensions
    {
        public static T DeepClone<T>(this T obj)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;

                return (T)formatter.Deserialize(ms);
            }
        }
    }
}
