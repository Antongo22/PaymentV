using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot;
using PaymentV.SBP_API;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace PaymentV.Base
{
    public static class GetOrder
    {
        async public static Task HandleGetOrder(ITelegramBotClient client, Message message)
        {
            PaymentApiService paymentApiService = PaymentApiService.Instance;

            string response = await paymentApiService.GetOrderDataAsync(message.Text);

            switch (response?.ToLower())
            {
                case null:
                    await client.SendTextMessageAsync(message.Chat.Id, $"Ошибка при получении статуса заказа!");
                    break;
                case "new":
                    InlineKeyboardMarkup inlineKeyboard = new(new[]
                    {
                        InlineKeyboardButton.WithCallbackData(
                            text: "Обновить", callbackData: $"update:{message.Text}:{message.MessageId + 1}"),
                    });

                    await client.SendTextMessageAsync(message.Chat.Id, $"Заказ - {message.Text}\n" +
                                                                    $"Статус - не оплачен!\n" +
                                                                    $"Последнее обновление - {DateTime.Now.ToString("HH:mm:ss")}\n",
                                                                    replyMarkup: inlineKeyboard);
                    break;
                case "paid":
                    await client.SendTextMessageAsync(message.Chat.Id, $"Заказ - {message.Text}\n" +
                                                                       $"Статус - оплачен!");
                    break;
            }
        }


        async public static Task HandleUpdateOrder(ITelegramBotClient client, CallbackQuery callbackQuery)
        {
            PaymentApiService paymentApiService = PaymentApiService.Instance;

            string orderId = callbackQuery.Data.Split(':')[1];
            int messageId = int.Parse(callbackQuery.Data.Split(':')[2]);
            string response = await paymentApiService.GetOrderDataAsync(orderId);

            try
            {
                await client.DeleteMessageAsync(callbackQuery.Message.Chat.Id, messageId);
            }
            catch (Exception ex)
            {
                return;
            }

            switch (response?.ToLower())
            {
                case null:
                    await client.SendTextMessageAsync(callbackQuery.Message.Chat.Id, $"Ошибка при получении статуса заказа!");
                    break;
                case "new":
                    InlineKeyboardMarkup inlineKeyboard = new(new[]
                    {
                        InlineKeyboardButton.WithCallbackData(
                            text: "Обновить", callbackData: $"update:{orderId}:{messageId + 1}"),
                    });

                    try
                    {
                        await client.SendTextMessageAsync(callbackQuery.Message.Chat.Id, $"Заказ - {orderId}\n" +
                                                                                        $"Статус - не оплачен!\n" +
                                                                                        $"Последнее обновление - {DateTime.Now.ToString("HH:mm:ss")}",
                                                                                        replyMarkup: inlineKeyboard);
                    } catch (Exception ex) 
                    {
                        await client.SendTextMessageAsync(callbackQuery.Message.Chat.Id, $"Произошла ошибка при обновлении данных заказа! Повторите попытку позже");
                    }
                    break;
                case "paid":
                    await client.SendTextMessageAsync(callbackQuery.Message.Chat.Id, $"Заказ - {orderId}\n" +
                                                                                     $"Статус - оплачен\n" +
                                                                                     $"Последнее обновление - {DateTime.Now.ToString("HH:mm:ss")}\",!");
                    break;
            }
        }x
    }
}
