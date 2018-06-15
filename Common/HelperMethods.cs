using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Common
{
    public static class HelperMethods
    {
        public static byte[] SerialiseIntoBinary(object customer)
        {
            MemoryStream memoryStream = new MemoryStream();
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(memoryStream, customer);
            memoryStream.Flush();
            memoryStream.Seek(0, SeekOrigin.Begin);
            return memoryStream.GetBuffer();
        }
    }
}
