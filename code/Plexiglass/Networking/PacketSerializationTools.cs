using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Plexiglass.Networking
{
    public class PacketSerializationTools
    {
        public static string GetString(byte[] data, int offset)
        {
            if (offset > data.Length) return "";

            string str = "";

            for(int i =offset;i < data.Length;i++)
            {
                if (data[i] == '\0') break;
                str += (char)data[i];
            }

            return str;
        }

        public static Vector3 GetVector3(byte[] data, int offset)
        {
            if (offset >= data.Length || (offset + 12) >= data.Length) return Vector3.Zero;

            Vector3 vec = new Vector3();

            vec.X = BitConverter.ToSingle(data, offset);
            vec.Y = BitConverter.ToSingle(data, offset + 4);
            vec.Z = BitConverter.ToSingle(data, offset + 8);

            return vec;
        }

        public static byte[] FromString(string str)
        {
            return Encoding.UTF8.GetBytes(str + '\0');
        }

        public static byte[] FromVector3(Vector3 vec)
        {
            byte[] data = new byte[12];
            BitConverter.GetBytes(vec.X).CopyTo(data, 0);
            BitConverter.GetBytes(vec.Y).CopyTo(data, 4);
            BitConverter.GetBytes(vec.Z).CopyTo(data, 8);

            return data;
        }
    }
}
