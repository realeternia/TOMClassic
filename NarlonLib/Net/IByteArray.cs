using System.Collections.Generic;
using System.Text;

namespace NarlonLib.Net
{
    public class IByteArray
    {
        private List<byte> datas = new List<byte>();
        private int type;
        private int id;

        public int Type
        {
            get { return type; }
            set { type = value; }
        }

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public IByteArray(byte[] data, bool isFull)
        {
            datas.AddRange(data);

            if (isFull)
            {
                type = ReadByte();
                id = ReadInt16();
            }
        }

        public int ReadInt32()
        {
            int i = System.BitConverter.ToInt32(datas.ToArray(), 0);
            datas.RemoveRange(0, 4);
            return i;
        }

        public int ReadInt16()
        {
            int i = System.BitConverter.ToInt16(datas.ToArray(), 0);
            datas.RemoveRange(0, 2);
            return i;
        }

        public byte ReadByte()
        {
            byte b = datas[0];
            datas.RemoveAt(0);
            return b;
        }

        public string ReadString()
        {
            int i = ReadInt16();
            string s = Encoding.UTF8.GetString(datas.ToArray(), 0, i);
            datas.RemoveRange(0, i);

            return s;
        }
    }
}
