using System;
using System.CodeDom;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using OpenFox.Parsing.Packets;

namespace OpenFox.Parsing.Utility
{
    public class PacketWriter
    {
        protected byte[] _data;
        protected int _position;
        private const int InitialSize = 16;
        private readonly bool _autoResize;
        public bool _isBigEndian;

        public int Capacity
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _data.Length;
        }
        public byte[] Data
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _data;
        }
        public int Length
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _position;
        }

        public static readonly ThreadLocal<ASCIIEncoding> encoding = new ThreadLocal<ASCIIEncoding>(() => new ASCIIEncoding());
        public const int StringBufferMaxLength = ushort.MaxValue;
        private readonly byte[] _stringBuffer = new byte[StringBufferMaxLength];

        public PacketWriter(bool autoResize, int initialSize, bool isBigEndian)
        {
            _data = new byte[initialSize];
            _autoResize = autoResize;
            _isBigEndian = isBigEndian;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ResizeIfNeed(int newSize)
        {
            if (_data.Length < newSize)
            {
                Array.Resize(ref _data, newSize);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EnsureFit(int additionalSize)
        {
            if (_data.Length < _position + additionalSize)
            {
                Array.Resize(ref _data, Math.Max(_position + additionalSize, _data.Length * 2));
            }
        }

        public void Reset(int size)
        {
            ResizeIfNeed(size);
            _position = 0;
        }

        public void Reset()
        {
            _position = 0;
        }

        public byte[] CopyData()
        {
            byte[] resultData = new byte[_position];
            Buffer.BlockCopy(_data, 0, resultData, 0, _position);
            return resultData;
        }

        /// <summary>
        /// Sets position of NetDataWriter to rewrite previous values
        /// </summary>
        /// <param name="position">new byte position</param>
        /// <returns>previous position of data writer</returns>
        public int SetPosition(int position)
        {
            int prevPosition = _position;
            _position = position;
            return prevPosition;
        }



        public void Put(long value)
        {
            if (_autoResize)
                ResizeIfNeed(_position + 8);
            BitConverterWithEndian.GetBytes(_isBigEndian, _data, _position, value);
            _position += 8;
        }

        public void Put(ulong value)
        {
            if (_autoResize)
                ResizeIfNeed(_position + 8);
            BitConverterWithEndian.GetBytes(_isBigEndian, _data, _position, value);
            _position += 8;
        }

        public void Put(int value)
        {
            if (_autoResize)
                ResizeIfNeed(_position + 4);
            BitConverterWithEndian.GetBytes(_isBigEndian, _data, _position, value);
            _position += 4;
        }

        public void Put(uint value)
        {
            if (_autoResize)
                ResizeIfNeed(_position + 4);
            BitConverterWithEndian.GetBytes(_isBigEndian, _data, _position, value);
            _position += 4;
        }

        public void Put(char value)
        {
            Put((ushort)value);
        }

        public void Put(ushort value)
        {
            if (_autoResize)
                ResizeIfNeed(_position + 2);
            BitConverterWithEndian.GetBytes(_isBigEndian, _data, _position, value);
            _position += 2;
        }

        public void Put(short value)
        {
            if (_autoResize)
                ResizeIfNeed(_position + 2);
            BitConverterWithEndian.GetBytes(_isBigEndian, _data, _position, value);
            _position += 2;
        }

        public void Put(sbyte value)
        {
            if (_autoResize)
                ResizeIfNeed(_position + 1);
            _data[_position] = (byte)value;
            _position++;
        }

        public void Put(byte value)
        {
            if (_autoResize)
                ResizeIfNeed(_position + 1);
            _data[_position] = value;
            _position++;
        }

        public void Put(byte[] data, int offset, int length)
        {
            if (_autoResize)
                ResizeIfNeed(_position + length);
            Buffer.BlockCopy(data, offset, _data, _position, length);
            _position += length;
        }

        public void Put(byte[] data)
        {
            if (_autoResize)
                ResizeIfNeed(_position + data.Length);
            Buffer.BlockCopy(data, 0, _data, _position, data.Length);
            _position += data.Length;
        }

        public void PutSBytesWithLength(sbyte[] data, int offset, ushort length)
        {
            if (_autoResize)
                ResizeIfNeed(_position + 2 + length);
            BitConverterWithEndian.GetBytes(_isBigEndian, _data, _position, length);
            Buffer.BlockCopy(data, offset, _data, _position + 2, length);
            _position += 2 + length;
        }

        public void PutSBytesWithLength(sbyte[] data)
        {
            PutArray(data, 1);
        }

        public void PutBytesWithLength(byte[] data, int offset, ushort length)
        {
            if (_autoResize)
                ResizeIfNeed(_position + 2 + length);
            BitConverterWithEndian.GetBytes(_isBigEndian, _data, _position, length);
            Buffer.BlockCopy(data, offset, _data, _position + 2, length);
            _position += 2 + length;
        }

        public void PutBytesWithLength(byte[] data)
        {
            PutArray(data, 1);
        }

        public void Put(bool value)
        {
            Put((byte)(value ? 1 : 0));
        }

        public void PutArray(Array arr, int sz)
        {
            ushort length = arr == null ? (ushort)0 : (ushort)arr.Length;
            sz *= length;
            if (_autoResize)
                ResizeIfNeed(_position + sz + 2);
            BitConverterWithEndian.GetBytes(_isBigEndian, _data, _position, length);
            if (arr != null)
                Buffer.BlockCopy(arr, 0, _data, _position + 2, sz);
            _position += sz + 2;
        }

        public void PutArray(float[] value)
        {
            PutArray(value, 4);
        }

        public void PutArray(double[] value)
        {
            PutArray(value, 8);
        }

        public void PutArray(long[] value)
        {
            PutArray(value, 8);
        }

        public void PutArray(ulong[] value)
        {
            PutArray(value, 8);
        }

        public void PutArray(int[] value)
        {
            PutArray(value, 4);
        }

        public void PutArray(uint[] value)
        {
            PutArray(value, 4);
        }

        public void PutArray(ushort[] value)
        {
            PutArray(value, 2);
        }

        public void PutArray(short[] value)
        {
            PutArray(value, 2);
        }

        public void PutArray(bool[] value)
        {
            PutArray(value, 1);
        }

        public void PutArray(string[] value)
        {
            ushort strArrayLength = value == null ? (ushort)0 : (ushort)value.Length;
            Put(strArrayLength);
            for (int i = 0; i < strArrayLength; i++)
                Put(value[i]);
        }

        public void PutArray(string[] value, int strMaxLength)
        {
            ushort strArrayLength = value == null ? (ushort)0 : (ushort)value.Length;
            Put(strArrayLength);
            for (int i = 0; i < strArrayLength; i++)
                Put(value[i], strMaxLength);
        }

        public void Put(string value)
        {
            Put(value, 0);
        }

        /// <summary>
        /// Note that "maxLength" only limits the number of characters in a string, not its size in bytes.
        /// </summary>
        public void Put(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            int length = maxLength > 0 && value.Length > maxLength ? maxLength : value.Length;
            int size = encoding.Value.GetBytes(value, 0, length, _stringBuffer, 0);

            if (size == 0 || size >= StringBufferMaxLength)
            {
                return;
            }

            Put(_stringBuffer, 0, size);
        }

        public void Put<T>(T obj) where T : INetSerializable
        {
            obj.Serialize(this);
        }
    }
}