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

        async public static Task HandleUpdateOrder(ITelegramBotClient client, CallbackQuery callbackQuery)
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

            Data.userStates[chatid].botState = State.BotState.Default;
            Data.userStates[chatid].isVerified = true;
            await client.SendTextMessageAsync(chatid, $"Вы верефецированны!!");
        }

    }
}
