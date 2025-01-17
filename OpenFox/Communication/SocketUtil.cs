using System;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace OpenFox.Communication
{
    internal class SocketUtil
    {
        public static void SetupKeepAlive(Socket socket)
        {
            // Get the size of the uint to use to back the byte array
            int size = Marshal.SizeOf((uint)0);

            // Create the byte array
            byte[] keepAlive = new byte[size * 3];

            // Pack the byte array:
            // Turn keepalive on
            Buffer.BlockCopy(BitConverter.GetBytes((uint)1), 0, keepAlive, 0, size);
            // Set amount of time without activity before sending a keepalive to 5 seconds
            Buffer.BlockCopy(BitConverter.GetBytes((uint)5000), 0, keepAlive, size, size);
            // Set keepalive interval to 5 seconds
            Buffer.BlockCopy(BitConverter.GetBytes((uint)5000), 0, keepAlive, size * 2, size);

            // Set the keep-alive settings on the underlying Socket
            socket.IOControl(IOControlCode.KeepAliveValues, keepAlive, null);
        }
    }
}
