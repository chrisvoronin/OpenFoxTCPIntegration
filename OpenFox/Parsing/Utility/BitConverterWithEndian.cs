using System;
using System.Runtime.CompilerServices;

namespace OpenFox.Parsing.Utility
{
    public static class BitConverterWithEndian
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void WriteWithEndian(bool isBigEndian, byte[] buffer, int offset, ulong data)
        {
            if (isBigEndian)
            {
                buffer[offset + 7] = (byte)(data >> 0);
                buffer[offset + 6] = (byte)(data >> 8);
                buffer[offset + 5] = (byte)(data >> 16);
                buffer[offset + 4] = (byte)(data >> 24);
                buffer[offset + 3] = (byte)(data >> 32);
                buffer[offset + 2] = (byte)(data >> 40);
                buffer[offset + 1] = (byte)(data >> 48);
                buffer[offset] = (byte)(data >> 56);
            }
            else
            {
                buffer[offset] = (byte)(data >> 0);
                buffer[offset + 1] = (byte)(data >> 8);
                buffer[offset + 2] = (byte)(data >> 16);
                buffer[offset + 3] = (byte)(data >> 24);
                buffer[offset + 4] = (byte)(data >> 32);
                buffer[offset + 5] = (byte)(data >> 40);
                buffer[offset + 6] = (byte)(data >> 48);
                buffer[offset + 7] = (byte)(data >> 56);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void WriteWithEndian(bool isBigEndian, byte[] buffer, int offset, int data)
        {

            if (isBigEndian)
            {
                buffer[offset + 3] = (byte)(data >> 0);
                buffer[offset + 2] = (byte)(data >> 8);
                buffer[offset + 1] = (byte)(data >> 16);
                buffer[offset] = (byte)(data >> 24);
            }
            else
            {
                buffer[offset] = (byte)(data >> 0);
                buffer[offset + 1] = (byte)(data >> 8);
                buffer[offset + 2] = (byte)(data >> 16);
                buffer[offset + 3] = (byte)(data >> 24);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteWithEndian(bool isBigEndian, byte[] buffer, int offset, short data)
        {
            if (isBigEndian)
            {
                buffer[offset + 1] = (byte)(data >> 0);
                buffer[offset] = (byte)(data >> 8);
            }
            else
            {
                buffer[offset] = (byte)(data >> 0);
                buffer[offset + 1] = (byte)(data >> 8);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetBytes(bool isBigEndian, byte[] bytes, int startIndex, short value)
        {
            WriteWithEndian(isBigEndian, bytes, startIndex, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetBytes(bool isBigEndian, byte[] bytes, int startIndex, ushort value)
        {
            WriteWithEndian(isBigEndian, bytes, startIndex, (short)value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetBytes(bool isBigEndian, byte[] bytes, int startIndex, int value)
        {
            WriteWithEndian(isBigEndian, bytes, startIndex, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetBytes(bool isBigEndian, byte[] bytes, int startIndex, uint value)
        {
            WriteWithEndian(isBigEndian, bytes, startIndex, (int)value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetBytes(bool isBigEndian, byte[] bytes, int startIndex, long value)
        {
            WriteWithEndian(isBigEndian, bytes, startIndex, (ulong)value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetBytes(bool isBigEndian, byte[] bytes, int startIndex, ulong value)
        {
            WriteWithEndian(isBigEndian, bytes, startIndex, value);
        }
    }
}