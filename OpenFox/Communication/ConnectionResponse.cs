namespace OpenFox.Communication
{
    internal class ConnectionResponse
    {
        public int exchangeId = 0;
        public int majorVersion = 0;
        public int minorVersion = 0;
        public long maxFrameLength = 0;
        public int maxIdleTimeOut = 0;
        public int defaultTimeOut = 0;
        public bool useEncryption = false;
        public string codingTechnique = "";
        public string newLineSequence = "";
    }
}
