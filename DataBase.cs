using PaymentV.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;

namespace PaymentV
{
    public static class DataBase
    {
        public static readonly long ouwnerId = 860491148;
        static string xmlfile = "usersdata.xml";

        private static XmlDocument LoadOrCreateXmlDocument()
        {
            XmlDocument xmlDoc = new XmlDocument();
            if (File.Exists(xmlfile))
            {
                xmlDoc.Load(xmlfile);
            }
            else
            {
                XmlElement root = xmlDoc.CreateElement("Users");
                xmlDoc.AppendChild(root);

                xmlDoc.Save(xmlfile);
            }
            return xmlDoc;
        }


        public static bool IsVerified(long chatId)
        {
            if (chatId == ouwnerId) return true;


            return IsUserVerified(chatId);
        }


        public static bool IsUserVerified(long chatid)
        {
            XmlDocument xmlDoc = LoadOrCreateXmlDocument();

            XmlNode userNode = xmlDoc.SelectSingleNode($"/Users/User[@chatid='{chatid}']");

            if (userNode == null || userNode.SelectSingleNode("isverified") == null)
            {
                return false;
            }

            bool isVerified;
            if (!bool.TryParse(userNode.SelectSingleNode("isverified").InnerText, out isVerified))
            {
                return false;
            }

            return isVerified;
        }


        public static void UpdateUserDataInXml(long chatid, bool isverified)
        {
            XmlDocument xmlDoc = LoadOrCreateXmlDocument();

            XmlNode userNode = xmlDoc.SelectSingleNode($"/Users/User[@chatid='{chatid}']");

            if (userNode == null)
            {
                userNode = xmlDoc.CreateElement("User");
                XmlAttribute chatidAttribute = xmlDoc.CreateAttribute("chatid");
                chatidAttribute.Value = chatid.ToString();
                userNode.Attributes.Append(chatidAttribute);
                xmlDoc.SelectSingleNode("/Users").AppendChild(userNode);
            }

            XmlNode isVerifiedNode = userNode.SelectSingleNode("isverified");
            if (isVerifiedNode == null)
            {
                isVerifiedNode = xmlDoc.CreateElement("isverified");
                userNode.AppendChild(isVerifiedNode);
            }

            isVerifiedNode.InnerText = isverified.ToString();

            xmlDoc.Save(xmlfile);


            if (isverified)
            {
                Data.userStates[chatid].botState = State.BotState.Default;
            }
            else
            {
                Data.userStates[chatid].botState = State.BotState.VerifiedFailed;
            }

            Data.userStates[chatid].isVerified = isverified;
        }

    }
}
