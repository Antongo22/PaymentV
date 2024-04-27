using PaymentV.SBP_API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Telegram.Bot;

namespace PaymentV.Base
{
    public static class Verification
    {
        async public static Task HandleUnVerified(ITelegramBotClient client, Message message)
        {
            InlineKeyboardMarkup inlineKeyboard = new(new[]
                    {
                        InlineKeyboardButton.WithCallbackData(
                            text: "Отправить запрос", callbackData: $"request:{message.Chat.Id}:{message.MessageId + 1}:{message?.Chat?.Username}"),
                    });

            await client.SendTextMessageAsync(message.Chat.Id, $"Вы не верефицированны! Для верефикации отправьте запрос!",
                                                            replyMarkup: inlineKeyboard);

        }

        async public static Task HandleSendRequest(ITelegramBotClient client, CallbackQuery callbackQuery)
        {

            long chatid = long.Parse(callbackQuery.Data.Split(':')[1]);
            int messageId = int.Parse(callbackQuery.Data.Split(':')[2]);
            string username = callbackQuery.Data.Split(':')[3];


            try
            {
                await client.DeleteMessageAsync(callbackQuery.Message.Chat.Id, messageId);
            }
            catch (Exception ex)
            {
                return;
            }

            await client.SendTextMessageAsync(callbackQuery.Message.Chat.Id, $"Запрос на добавление отправлен!");
            Data.userStates[chatid].botState = State.BotState.SendVerifiedRequest;

            await SendRequest(client, chatid, username);
        }


        static async Task SendRequest(ITelegramBotClient client, long chatid, string username)
        {

            var mes = await client.SendTextMessageAsync(DataBase.ouwnerId, $"Новый запрос на верефикацию от @{username}");

            InlineKeyboardMarkup inlineKeyboard = new(new[]
                   {
                        InlineKeyboardButton.WithCallbackData(
                            text: "Принять", callbackData: $"requesttoowner:{chatid}:{mes.MessageId + 1}:confirm"),
                         InlineKeyboardButton.WithCallbackData(
                            text: "Отклонить", callbackData: $"requesttoowner:{chatid}:{mes.MessageId + 1}:notconfirm"),
                    });

            await client.SendTextMessageAsync(DataBase.ouwnerId, $"Подтвердите запрос от @{username}", replyMarkup: inlineKeyboard);
        }


        async public static Task HandleGetRequest(ITelegramBotClient client, CallbackQuery callbackQuery)
        {

            long chatid = long.Parse(callbackQuery.Data.Split(':')[1]);
            int messageId = int.Parse(callbackQuery.Data.Split(':')[2]);
            string status = callbackQuery.Data.Split(':')[3];


            try
            {
                await client.DeleteMessageAsync(DataBase.ouwnerId, messageId);
            }
            catch (Exception ex)
            {
                return;
            }

            switch (status)
            {
                case "confirm":
                    DataBase.UpdateUserDataInXml(chatid, true);
                    await client.SendTextMessageAsync(DataBase.ouwnerId, $"Запрос на добавление подтверждён!");
                    await client.SendTextMessageAsync(chatid, $"Вы верефецированны!!");
                    break;
                case "notconfirm":
                    DataBase.UpdateUserDataInXml(chatid, false);
                    await client.SendTextMessageAsync(callbackQuery.Message.Chat.Id, $"Запрос на добавление отклонён!");
                    await client.SendTextMessageAsync(chatid, $"Вам отказано в доступе!!");
                    break;
            }

            

        }


    }
}
