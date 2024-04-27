using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Telegram.Bot;

namespace PaymentV.Base
{
    public static class Owner
    {
        static string keyfile = "keyfile.tmp";

        public static string GenerateLink()
        {
            string key = GenerateKey(30);

            System.IO.File.WriteAllText(keyfile, key);

            return $"https://t.me/NI_PaymentV_Bot?start=key-{key}";
        }

        private static readonly Random random = new Random();
        private const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";


        public static bool IsValidKey(string key)
        {
            if(key.Contains(System.IO.File.ReadAllText(keyfile))) return true;
            return false;
        }

        public static string GenerateKey(int length)
        {
            var stringBuilder = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                stringBuilder.Append(chars[random.Next(chars.Length)]);
            }
            return stringBuilder.ToString();
        }

        public static string GetKey()
        {
            if(!System.IO.File.Exists(keyfile)) return null;

            return System.IO.File.ReadAllText(keyfile);
        }


        async public static Task HandleShowTeam(ITelegramBotClient client, Message message)
        {
            var usersTuple = DataBase.GetAllUsers();

            List<DataUser> verifiedUsers = usersTuple.Item1;
            List<DataUser> unverifiedUsers = usersTuple.Item2;

            var mes = await client.SendTextMessageAsync(DataBase.ouwnerId, $"Вот список сотрудников, у которых есть доступ:");

            int messageID = mes.MessageId;

            for(int i = 0; i < verifiedUsers.Count; i++)
            {
                InlineKeyboardMarkup inlineKeyboard = new(new[]
                   {
                        InlineKeyboardButton.WithCallbackData(
                            text: "Убрать доступ", callbackData: $"deleteuser:{verifiedUsers[i].ChatID}:{verifiedUsers[i].Name}:{++messageID}"), 
                    });

                await client.SendTextMessageAsync(DataBase.ouwnerId, 
                    $"Сотрудник @{verifiedUsers[i].Name} c id {verifiedUsers[i].ChatID}", replyMarkup: inlineKeyboard);
            }

            mes = await client.SendTextMessageAsync(DataBase.ouwnerId, $"Вот список сотрудников, у которых нет доступа:");

            messageID = mes.MessageId;

            for (int i = 0; i < unverifiedUsers.Count; i++)
            {
                InlineKeyboardMarkup inlineKeyboard = new(new[]
                   {
                        InlineKeyboardButton.WithCallbackData(
                            text: "Восстановить доступ", callbackData: $"recoveruser:{unverifiedUsers[i].ChatID}:{unverifiedUsers[i].Name}:{++messageID}"),
                    });

                await client.SendTextMessageAsync(DataBase.ouwnerId,
                    $"Сотрудник @{unverifiedUsers[i].Name} c id {unverifiedUsers[i].ChatID}", replyMarkup: inlineKeyboard);
            }
        }

        async public static Task HandleDelete(ITelegramBotClient client, CallbackQuery callbackQuery)
        {

            long chatid = long.Parse(callbackQuery.Data.Split(':')[1]);
            string name = callbackQuery.Data.Split(':')[2];
            int messageId = int.Parse(callbackQuery.Data.Split(':')[3]);

            InlineKeyboardMarkup inlineKeyboard = new(new[]
                  {
                        InlineKeyboardButton.WithCallbackData(
                            text: "Да", callbackData: $"confdelete:{chatid}:{name}:{messageId}:confirm"),
                        InlineKeyboardButton.WithCallbackData(
                            text: "Отмена", callbackData: $"confdelete:{chatid}:{name}:{messageId}:confirm"),
                    });

            try
            {
                await client.EditMessageTextAsync(DataBase.ouwnerId, messageId, $"Точно удалить права сотрудника @{name}?", replyMarkup : inlineKeyboard);
            }
            catch (Exception ex)
            {
                return;
            }

        }

        async public static Task HandleConfDelete(ITelegramBotClient client, CallbackQuery callbackQuery)
        {

            long chatid = long.Parse(callbackQuery.Data.Split(':')[1]);
            string name = callbackQuery.Data.Split(':')[2];
            int messageId = int.Parse(callbackQuery.Data.Split(':')[3]);
        

            try
            {
                await client.EditMessageTextAsync(DataBase.ouwnerId, messageId, $"Сотрудник @{name} исключён из доступа!");
                DataBase.UpdateUserDataInXml(chatid, false, name);

                await client.SendTextMessageAsync(chatid, $"Вы лишены прав!");

            }
            catch (Exception ex)
            {
                return;
            }

        }


        async public static Task HandleRecover(ITelegramBotClient client, CallbackQuery callbackQuery)
        {

            long chatid = long.Parse(callbackQuery.Data.Split(':')[1]);
            string name = callbackQuery.Data.Split(':')[2];
            int messageId = int.Parse(callbackQuery.Data.Split(':')[3]);
           

            try
            {
                await client.EditMessageTextAsync(DataBase.ouwnerId, messageId, $"Сотрудник @{name} восстановлен в правах!");
                DataBase.UpdateUserDataInXml(chatid, true, name);
                await client.SendTextMessageAsync(chatid, $"Вы востановлены в правах!");
            }
            catch (Exception ex)
            {
                return;
            }

        }
    }
}
