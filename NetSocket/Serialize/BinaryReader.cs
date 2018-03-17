using System;
using System.Text;

namespace JLM.NetSocket
{
    public class TBinaryReader 
    {
        private byte[] dataArray;
        private int position;
        public TBinaryReader(byte[] byteArray)
        {
            dataArray = byteArray;
            position = 0;

        }

        public int Position
        {
            get { return position; }
            set
            {
                if (value > dataArray.Length || value < 0)
                {
                    return;
                }
                position = value;
            }
        }
        public void Reset(byte[] byteArray)
        {
            dataArray = byteArray;
            position = 0;
        }

        public bool ReadBoolean()
        {
            return (dataArray[position++] != 0);
        }

        public byte ReadByte()
        {
            return dataArray[position++];
        }

        public short ReadInt16()
        {
            return (short)(dataArray[position++] | dataArray[position++] << 8);
        }
        public ushort ReadUInt16()
        {
            return (ushort)(dataArray[position++] | dataArray[position++] << 8);
        }

        public int ReadInt32()
        {
            return (int)(dataArray[position++] | dataArray[position++] << 8
                | dataArray[position++] << 16 | dataArray[position++] << 24);
        }

        public uint ReadUInt32()
        {
            return (uint)(dataArray[position++] | dataArray[position++] << 8
                | dataArray[position++] << 16 | dataArray[position++] << 24);
        }

        public long ReadInt64()
        {
            uint lo = (uint)(dataArray[position++] | dataArray[position++] << 8 |
                             dataArray[position++] << 16 | dataArray[position++] << 24);
            uint hi = (uint)(dataArray[position++] | dataArray[position++] << 8 |
                             dataArray[position++] << 16 | dataArray[position++] << 24);
            return (long)((ulong)hi) << 32 | lo;
        }

        public ulong ReadUInt64()
        {
            uint lo = (uint)(dataArray[position++] | dataArray[position++] << 8 |
                             dataArray[position++] << 16 | dataArray[position++] << 24);
            uint hi = (uint)(dataArray[position++] | dataArray[position++] << 8 |
                             dataArray[position++] << 16 | dataArray[position++] << 24);
            return ((ulong)hi) << 32 | lo;
        }

        public unsafe float ReadSingle()
        {
            uint tmpBuffer = (uint)(dataArray[position++] | dataArray[position++] << 8 |
                dataArray[position++] << 16 | dataArray[position++] << 24);
            return *((float*)&tmpBuffer);
        }

        public unsafe double ReadDouble()
        {
            uint lo = (uint)(dataArray[position++] | dataArray[position++] << 8 |
                dataArray[position++] << 16 | dataArray[position++] << 24);
            uint hi = (uint)(dataArray[position++] | dataArray[position++] << 8 |
                dataArray[position++] << 16 | dataArray[position++] << 24);
            ulong tmpBuffer = ((ulong)hi) << 32 | lo;
            return *((double*)&tmpBuffer);
        }

        public string ReadString()
        {
            ushort len = ReadUInt16();
            if (len > 0)
            {
                string s = Encoding.UTF8.GetString(dataArray, position, len);
                position += len;
                return s;
            }
            return "";
        }

        public byte[] ReadByteArray()
        {
            int size = ReadInt32();
            Byte[] tpd = new Byte[size];
            if (size > 0)
            {
                for (int i = 0; i < size; i++)
                {
                    tpd[i] = ReadByte();
                }
            }
            return tpd;
        }
    }
}
