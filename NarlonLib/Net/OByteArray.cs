using System.Collections.Generic;

namespace NarlonLib.Net
{
    public class OByteArray
    {
        private List<byte> datas = new List<byte>();

        public byte[] Datas
        {
            get { return datas.ToArray(); }
        }

        public void WriteByte(byte value)
        {
            datas.Add(value);
        }

        public void WriteShort(short value)
        {
            datas.AddRange(System.BitConverter.GetBytes(value));
        }

        public void WriteInt32(int value)
        {
            datas.AddRange(System.BitConverter.GetBytes(value));
        }

        public void WriteString(string value)
        {
            byte[] dat = System.Text.Encoding.UTF8.GetBytes(value);
            WriteShort((short)dat.Length);
            datas.AddRange(dat);
        }

        public void WriteBytes(byte[] data)
        {
            datas.AddRange(data);
        }
    }
}
