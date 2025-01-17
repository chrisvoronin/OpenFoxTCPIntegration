using System;
using System.Runtime.CompilerServices;

namespace OpenFox.Parsing.Utility
{
    public class PacketReader
    {
        protected byte[] _data;
        protected int _position;
        protected int _dataSize;
        private int _offset;
        private bool _isBigEndian;

        public byte[] RawData
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _data;
        }
        public int RawDataSize
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _dataSize;
        }
        public int UserDataOffset
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _offset;
        }
        public int UserDataSize
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _dataSize - _offset;
        }
        public bool IsNull
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _data == null;
        }
        public int Position
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _position;
        }
        public bool EndOfData
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _position == _dataSize;
        }
        public int AvailableBytes
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _dataSize - _position;
        }

        public void SkipBytes(int count)
        {
            _position += count;
        }

        public void SetPosition(int position)
        {
            _position = position;
        }

        public void SetSource(PacketWriter dataWriter)
        {
            _data = dataWriter.Data;
            _position = 0;
            _offset = 0;
            _dataSize = dataWriter.Length;
        }

        public void SetSource(byte[] source)
        {
            _data = source;
            _position = 0;
            _offset = 0;
            _dataSize = source.Length;
        }

        public void SetSource(byte[] source, int offset, int maxSize)
        {
            _data = source;
            _position = offset;
            _offset = offset;
            _dataSize = maxSize;
        }

        public PacketReader(bool isBigEndian)
        {
            _isBigEndian = isBigEndian;
        }

        public PacketReader(byte[] source, int offset, int maxSize)
        {
            SetSource(source, offset, maxSize);
        }

        #region GetMethods

        public byte PeekByte(int pos)
        {
            return _data[pos];
        }

        public byte GetByte()
        {
            byte res = _data[_position];
            _position++;
            return res;
        }

        public sbyte GetSByte()
        {
            return (sbyte)GetByte();
        }

        public bool GetBool()
        {
            return GetByte() == 1;
        }

        public char GetChar()
        {
            return (char)GetUShort();
        }

        public ushort GetUShort()
        {
            ushort result;
            if (_isBigEndian)
            {
                result = (ushort)(_data[_position] << 8 | _data[_position + 1]);
            }
            else
            {
                result = (ushort)(_data[_position + 1] << 8 | _data[_position]);
            }

            _position += 2;
            return result;
        }

        public int GetInt()
        {
            int result;

            if (_isBigEndian)
            {
                result = _data[_position] << 24 | _data[_position + 1] << 16 | _data[_position + 2] << 8 | _data[_position + 3];
            }
            else
            {
                result = _data[_position + 3] << 24 | _data[_position + 2] << 16 | _data[_position + 1] << 8 | _data[_position];
            }

            _position += 4;
            return result;
        }

        public uint GetUInt()
        {
            uint result = (uint)GetInt();
            return result;
        }

        public byte[] PeekBytes(int size)
        {
            if (size == 0 || size >= PacketWriter.StringBufferMaxLength)
            {
                return new byte[0];
            }

            byte[] outgoingData = new byte[size];
            Buffer.BlockCopy(_data, _position, outgoingData, 0, size);
            return outgoingData;
        }

        public string GetString(int size)
        {
            if (size == 0 || size >= PacketWriter.StringBufferMaxLength)
            {
                return string.Empty;
            }

            // this increments position
            ArraySegment<byte> data = new ArraySegment<byte>(_data, _position, size);
            _position += size;

            return PacketWriter.encoding.Value.GetString(data.Array, data.Offset, data.Count);
        }

        public byte[] GetRemainingBytes()
        {
            byte[] outgoingData = new byte[AvailableBytes];
            Buffer.BlockCopy(_data, _position, outgoingData, 0, AvailableBytes);
            _position = _data.Length;
            return outgoingData;
        }

        public void GetBytes(byte[] destination, int count)
        {
            Buffer.BlockCopy(_data, _position, destination, 0, count);
            _position += count;
        }

        #endregion

        public void Clear()
        {
            _position = 0;
            _dataSize = 0;
            _data = null;
        }
    }
}