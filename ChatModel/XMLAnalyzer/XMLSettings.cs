using System;
using System.Net;
using System.Xml;

namespace ChatModel.XMLAnalyzer {
    public class XmlSettings {
        public readonly IPAddress IpAddress ;
        public readonly int Port;

        public XmlSettings(string filePath) {
            var doc = new XmlDocument();
            {
                doc.Load(filePath);
                XmlNodeList mainNode = doc.SelectNodes("settings");
                if (mainNode == null) {
                    throw new XmlException("Кривая структура XML");
                }
                if (mainNode.Count != 1) {
                    throw new XmlException("Кривая структура XML");
                }
                foreach (XmlNode node in mainNode) {
                    XmlNodeList settingsNodeList = node.ChildNodes;
                    if (settingsNodeList.Count != 2) {
                        throw new XmlException("Кривая структура XML");
                    }
                    foreach (XmlNode child in settingsNodeList) {
                            if (child.Name.Equals("IpAddress")) {
                                try {
                                    IpAddress = IPAddress.Parse(child.InnerText); {
                                        
                                    }
                                }
                                catch (Exception) {
                                    
                                    throw new XmlException("Невозможно прочитать айпи");
                                }
                                
                            }
                            else if (child.Name.Equals("Port")) {
                                try {
                                    Port = Int32.Parse(child.InnerText);
                                    {

                                    }
                                }
                                catch (Exception) {

                                    throw new XmlException("Невозможно прочитать порт");
                                }
                            }
                            else {
                            throw new XmlException("Кривая структура XML");
                            }
                        }
                }
            }
        } 
    }
}