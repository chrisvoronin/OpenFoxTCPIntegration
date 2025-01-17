using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;
using OpenFox.DataAccess;
using OpenFox.Parsing.Packets;

namespace OpenFox.Parsing.Utility
{
    public class DataMessageManager
    {
        private Dictionary<ushort, List<DataMessagePacket>> _mDict = new Dictionary<ushort, List<DataMessagePacket>>();

        public bool Process(DataMessagePacket pt, out OFMLResponse response)
        {
            var exchangeId = pt.ExchangeId;
            bool isEnd = pt.endIndicator == MessageConstants.ExchangeConstantY;

            if (_mDict.ContainsKey(exchangeId))
            {
                _mDict[exchangeId].Add(pt);
            }
            else
            {
                _mDict[exchangeId] = new List<DataMessagePacket> { pt };
            }

            if (!isEnd)
            {
                response = null;
                return false;
            } 
            else
            {
                response = BuildOFML(exchangeId);
                _mDict.Remove(exchangeId);
                return true;
            }
        }

        //Backup method

        private OFMLResponse BuildOFML(ushort exchangeId)
        {
            List<DataMessagePacket> packets = _mDict[exchangeId];

            // combine all
            StringBuilder builder = new StringBuilder();
            foreach (var packet in packets)
            {
                builder.Append(packet.OFML);
            }

            // convert to xml document
            string fullText = builder.ToString();

            //just in case it is invalid xml we still want to return all
            //only fallback, shouldn't happen
            if (!MakeDocument(fullText, out XDocument xmlDoc))
            {
                OFMLResponse ret = new OFMLResponse();
                ret.message = fullText;
                return ret;
            }

            OFMLResponse ofml = new OFMLResponse();
            ofml.userId = ParseUserId(xmlDoc);
            ofml.message = ParseMessage(xmlDoc);
            ofml.source = ParseSource(xmlDoc);

            return ofml;
        }

        private string ParseMessage(XDocument xmlDoc)
        {
            return xmlDoc.ToString();
        }

        private string ParseUserId(XDocument xmlDoc)
        {
            XElement sourceElement = xmlDoc.XPathSelectElement("/OFML/HDR/PQR");
            if (sourceElement != null)
            {
                return sourceElement.Value;
            }
            return null;
        }

        private string ParseSource(XDocument xmlDoc)
        {
            XElement sourceElement = xmlDoc.XPathSelectElement("/OFML/HDR/SRC");
            if (sourceElement != null)
            {
                return sourceElement.Value;
            }
            return null;
        }

        private bool MakeDocument(string xmlString, out XDocument doc)
        {
            try
            {
                XDocument xmlDoc = XDocument.Parse(xmlString);
                doc = xmlDoc;
                return true;
            }
            catch
            {
                doc = null;
                return false;
            }
        }

    }
}
