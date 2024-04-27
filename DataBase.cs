using PaymentV.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using Telegram.Bot.Types;

namespace PaymentV
{
    public static class DataBase
    {
        public static long ouwnerId;
        public static long authgoupid;


        static string xmlfile = "usersdata.xml";

        private static XmlDocument LoadOrCreateXmlDocument()
        {
            XmlDocument xmlDoc = new XmlDocument();
            if (System.IO.File.Exists(xmlfile))
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


        static bool IsUserVerified(long chatid)
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


        public static void UpdateUserDataInXml(long chatid, bool isverified, string username)
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


            XmlNode usernameNode = userNode.SelectSingleNode("username");
            if (usernameNode == null)
            {
                usernameNode = xmlDoc.CreateElement("username");
                userNode.AppendChild(usernameNode);
            }

            usernameNode.InnerText = username.ToString();

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

        public static void SetVerKey(string key, long chatid)
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

            XmlNode lastVerKeyNode = userNode.SelectSingleNode("lastVerKey");
            if (lastVerKeyNode == null)
            {
                lastVerKeyNode = xmlDoc.CreateElement("lastVerKey");
                userNode.AppendChild(lastVerKeyNode);
            }

            lastVerKeyNode.InnerText = key;
            xmlDoc.Save(xmlfile);
        }


        public static bool IsNewKey(string key, long chatid)
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

            XmlNode lastVerKeyNode = userNode.SelectSingleNode("lastVerKey");

            if(key == null)
            {
                return true;
            }
            else if (lastVerKeyNode == null)
            {
                return true;
            }
            else
            {
                string existingKey = lastVerKeyNode.InnerText;
                if (existingKey == key)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public static (List<DataUser>, List<DataUser>) GetAllUsers()
        {
            List<DataUser> verifiedUsers = new List<DataUser>();
            List<DataUser> unverifiedUsers = new List<DataUser>();

            XmlDocument xmlDoc = LoadOrCreateXmlDocument();
            XmlNodeList userNodes = xmlDoc.SelectNodes("/Users/User");

            foreach (XmlNode userNode in userNodes)
            {
                string chatId = userNode.Attributes["chatid"].Value;
                string name = userNode.SelectSingleNode("username").InnerText;
                bool isVerified = false;
                bool.TryParse(userNode.SelectSingleNode("isverified").InnerText, out isVerified);

                DataUser user = new DataUser(chatId, name, isVerified);

                if (isVerified)
                {
                    verifiedUsers.Add(user);
                }
                else
                {
                    unverifiedUsers.Add(user);
                }
            }

            return (verifiedUsers, unverifiedUsers);
        }

    }
}
