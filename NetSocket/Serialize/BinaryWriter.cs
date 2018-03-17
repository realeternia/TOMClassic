using System;
using System.Text;

namespace JLM.NetSocket
{
    public class TBinaryWriter
    {
        private byte[] byteArray;
        private int capacity;
        private int position;

        public TBinaryWriter(int capacity = 32)
        {
            byteArray = new byte[capacity];
            this.capacity = capacity;
            position = 0;
        }

        public int Position
        {
            get { return position; }
            set
            {
                if (value > capacity)
                {
                    CheckCapacity(value - capacity);
                }
                position = value;
            }
        }

        public int Capacity
        {
            get
            {
                return capacity;
            }
            set
            {
                if (value != capacity)
                {
                    if (capacity < position)
                    {
                        return;
                    }
                    if (value > 0)
                    {
                        byte[] newBuffer = new byte[value];
                        if (position > 0) Buffer.BlockCopy(byteArray, 0, newBuffer, 0, position);
                        byteArray = newBuffer;
                    }
                    else
                    {
                        byteArray = null;
                    }
                    capacity = value;
                }
            }
        }

        private void CheckCapacity(int addCount)
        {
            int newCapacity = position + addCount;
            if (newCapacity > capacity)
            {
                if (newCapacity < 256)
                    newCapacity = 256;
                if (newCapacity < capacity * 2)
                    newCapacity = capacity * 2;
                Capacity = newCapacity;
            }
        }
        public void Write(bool value)
        {
            CheckCapacity(1);
            byteArray[position++] = (byte)(value ? 1 : 0);
        }

        public void Write(byte value)
        {
            CheckCapacity(1);
            byteArray[position++] = value;
        }
        public void Write(byte[] buffer)
        {
            CheckCapacity(4+buffer.Length);
            Write(buffer.Length);
            Buffer.BlockCopy(buffer, 0, byteArray, position, buffer.Length);
            position += buffer.Length;
        }

        public unsafe void Write(double value)
        {
            CheckCapacity(8);
            ulong TmpValue = *(ulong*)&value;
            byteArray[position++] = (byte)TmpValue;
            byteArray[position++] = (byte)(TmpValue >> 8);
            byteArray[position++] = (byte)(TmpValue >> 16);
            byteArray[position++] = (byte)(TmpValue >> 24);
            byteArray[position++] = (byte)(TmpValue >> 32);
            byteArray[position++] = (byte)(TmpValue >> 40);
            byteArray[position++] = (byte)(TmpValue >> 48);
            byteArray[position++] = (byte)(TmpValue >> 56);
        }

        public void Write(short value)
        {
            CheckCapacity(2);
            byteArray[position++] = (byte)value;
            byteArray[position++] = (byte)(value >> 8);
        }

        public void Write(ushort value)
        {
            CheckCapacity(2);
            byteArray[position++] = (byte)value;
            byteArray[position++] = (byte)(value >> 8);
        }

        public void Write(int value)
        {
            CheckCapacity(4);
            byteArray[position++] = (byte)value;
            byteArray[position++] = (byte)(value >> 8);
            byteArray[position++] = (byte)(value >> 16);
            byteArray[position++] = (byte)(value >> 24);

        }

        public void Write(uint value)
        {
            CheckCapacity(4);
            byteArray[position++] = (byte)value;
            byteArray[position++] = (byte)(value >> 8);
            byteArray[position++] = (byte)(value >> 16);
            byteArray[position++] = (byte)(value >> 24);
        }

        public void Write(long value)
        {
            CheckCapacity(8);
            byteArray[position++] = (byte)value;
            byteArray[position++] = (byte)(value >> 8);
            byteArray[position++] = (byte)(value >> 16);
            byteArray[position++] = (byte)(value >> 24);
            byteArray[position++] = (byte)(value >> 32);
            byteArray[position++] = (byte)(value >> 40);
            byteArray[position++] = (byte)(value >> 48);
            byteArray[position++] = (byte)(value >> 56);
        }

        public void Write(ulong value)
        {
            CheckCapacity(8);
            byteArray[position++] = (byte)value;
            byteArray[position++] = (byte)(value >> 8);
            byteArray[position++] = (byte)(value >> 16);
            byteArray[position++] = (byte)(value >> 24);
            byteArray[position++] = (byte)(value >> 32);
            byteArray[position++] = (byte)(value >> 40);
            byteArray[position++] = (byte)(value >> 48);
            byteArray[position++] = (byte)(value >> 56);
        }

        public unsafe void Write(float value)
        {
            CheckCapacity(4);
            uint TmpValue = *(uint*)&value;
            byteArray[position++] = (byte)TmpValue;
            byteArray[position++] = (byte)(TmpValue >> 8);
            byteArray[position++] = (byte)(TmpValue >> 16);
            byteArray[position++] = (byte)(TmpValue >> 24);
        }

        public void Write(string value)
        {
            if (value == null || value.Length == 0)
            {
                Write((ushort)0);
                return;
            }

            int len = Encoding.UTF8.GetByteCount(value);
            if (len >= ushort.MaxValue)
            {
                return;
            }

            Write((ushort)len);
            CheckCapacity(len);
            int realLen = Encoding.UTF8.GetBytes(value, 0, value.Length, byteArray, position);
            position += realLen;
        }

        public byte[] GetBytes()
        {
            byte[] bytes = new byte[position];
            Buffer.BlockCopy(byteArray, 0, bytes, 0, position);
            return bytes;
        }
    }

}
