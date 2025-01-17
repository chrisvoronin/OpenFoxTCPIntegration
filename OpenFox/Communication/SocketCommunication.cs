using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using OpenFox.Parsing.Packets;

namespace OpenFox.Communication
{
    internal class SocketCommunication : ISocketCommunication
    {
        private TcpClient _client;
        private const int _bufferSize = 1024 * 64;
        private byte[] _buffer = new byte[_bufferSize];

        private byte[] result = null;
        private int resultSize = 0;

        private readonly byte[] _messageStartIndicator;
        private readonly byte[] _messageEndIndicator;

        ISocketCallback _callbackHandler;
        private string _ip;
        private int _port;

        public SocketCommunication(string ip, int port, byte[] startIndicator, byte[] endIndicator)
        {
            _ip = ip;
            _port = port;
            _messageStartIndicator = startIndicator;
            _messageEndIndicator = endIndicator;
        }

        public void Connect(ISocketCallback handler)
        {
            _callbackHandler = handler;

            IPAddress parsedIP = IPAddress.Parse(_ip);
            IPEndPoint endPoint = new IPEndPoint(parsedIP, _port);

            _client = new TcpClient();

            SocketUtil.SetupKeepAlive(_client.Client);

            try
            {
                _client.BeginConnect(parsedIP, _port, new AsyncCallback(ConnectCallback), null);
            }
            catch
            {
                _client.Close();
                _callbackHandler.Socket_OnConnected(false);
            }
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                _client.EndConnect(ar);
                _client.Client.BeginReceive(_buffer, 0, _bufferSize, SocketFlags.None, ReceiveCallback, null);
                _callbackHandler.Socket_OnConnected(true);
            }
            catch (Exception ex)
            {
                _callbackHandler.Socket_OnDisconnected(ex);
            }            
            
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                int bytesRead = _client.Client.EndReceive(ar);
                byte[] receivedData = new byte[bytesRead];
                Array.Copy(_buffer, receivedData, bytesRead);

                // now build result, which may come from multiple receives
                // on first recieve bytesRead is the result for now.
                if (resultSize == 0)
                {
                    result = receivedData;
                    resultSize = bytesRead;
                }
                else
                {
                    // on any other receive we append.
                    int prevSize = resultSize;
                    byte[] combinedArray = new byte[prevSize + bytesRead];
                    Array.Copy(result, 0, combinedArray, 0, prevSize);
                    Array.Copy(receivedData, 0, combinedArray, prevSize, bytesRead);
                    result = combinedArray;
                    resultSize += bytesRead;
                }

                // if we read something, lets check if there are messages to return
                // there can be multiple messages at once.
                if (bytesRead > 0)
                {
                    SendResultsIfNeededAndRecalculateResult();
                }

                // Continue listening for more messages
                _client.Client.BeginReceive(_buffer, 0, _bufferSize, SocketFlags.None, ReceiveCallback, null);
            }
            catch (SocketException ex)
            {
                _client.Close();
                _callbackHandler.Socket_OnDisconnected(ex);
            }
        }

        private void SendResultsIfNeededAndRecalculateResult()
        {
            var resStart = FindAllOccurrences(result, _messageStartIndicator);
            var resEnd = FindAllOccurrences(result, _messageEndIndicator);
            int endLen = _messageEndIndicator.Length;

            // IF NO END INDICATOR, NOTHING TO RETURN
            if (resEnd.Count == 0) return;

            // check all results for multiple messages
            // and send them all
            for (int i = 0; i < resEnd.Count; i++)
            {
                int partLen = (resEnd[i] + endLen) - resStart[i];
                byte[] part = new byte[partLen];
                Array.Copy(result, resStart[i], part, 0, partLen);
                _callbackHandler.Socket_OnMessageReceived(part);
            }
            //now remove whatever we sent back from result
            int sizeSent = resEnd[resEnd.Count - 1] + endLen;
            resultSize -= sizeSent;
            result = RemoveFirstNCharacters(result, sizeSent);
        }

        private byte[] RemoveFirstNCharacters(byte[] inputArray, int numberOfCharacters)
        {
            if (inputArray == null || numberOfCharacters <= 0)
            {
                // Invalid input, return the original array.
                return inputArray;
            }

            int remainingLength = inputArray.Length - numberOfCharacters;

            if (remainingLength <= 0)
            {
                // If numberOfCharacters is greater than or equal to the array length, return an empty array.
                return new byte[0];
            }

            byte[] resultArray = new byte[remainingLength];

            // Copy the remaining bytes to the result array.
            Array.Copy(inputArray, numberOfCharacters, resultArray, 0, remainingLength);

            return resultArray;
        }

        private List<int> FindAllOccurrences(byte[] source, byte[] pattern)
        {
            List<int> occurrences = new List<int>();
            int patternLength = pattern.Length;
            int sourceLength = source.Length;

            for (int i = 0; i <= sourceLength - patternLength; i++)
            {
                bool found = true;
                for (int j = 0; j < patternLength; j++)
                {
                    if (source[i + j] != pattern[j])
                    {
                        found = false;
                        break;
                    }
                }

                if (found)
                {
                    occurrences.Add(i);
                }
            }

            return occurrences;
        }

        private int _tag;
        private PacketType _type;

        public void Write(byte[] data, int tag, PacketType type)
        {
            _tag = tag;
            _type = type;

            try
            {
                _client.Client.BeginSend(data, 0, data.Length, SocketFlags.None, SendCallback, null);
            }
            catch (Exception ex)
            {
                _client.Close();
                _callbackHandler.Socket_OnDisconnected(ex);
            }

        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                _client.Client.EndSend(ar);
                _callbackHandler.Socket_OnMessageSent(ar.IsCompleted, _tag, _type);
            }
            catch (SocketException ex)
            {
                _client.Close();
                _callbackHandler.Socket_OnDisconnected(ex);
            }
        }
    }
}


