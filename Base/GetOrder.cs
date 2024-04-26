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
                    await client.SendTextMessageAsync(message.Chat.Id, $"Заказ не оплачен!", replyMarkup: inlineKeyboard);
                    break;
                case "paid":
                    await client.SendTextMessageAsync(message.Chat.Id, $"Заказ оплачен!");
                    break;
            }
        }

        static InlineKeyboardMarkup inlineKeyboard = new(new[]
        {
            InlineKeyboardButton.WithCallbackData(
                text: "Обновить", callbackData: "update"),
        });

        async public static Task HandleUpdateOrder(ITelegramBotClient client, CallbackQuery callbackQuery)
        {
            PaymentApiService paymentApiService = PaymentApiService.Instance;

            string orderId = callbackQuery.Data.Split(':')[1];
            string response = await paymentApiService.GetOrderDataAsync(orderId);

            switch (response?.ToLower())
            {
                case null:
                    await client.SendTextMessageAsync(callbackQuery.Message.Chat.Id, $"Ошибка при получении статуса заказа!");
                    break;
                case "new":
                    await client.SendTextMessageAsync(callbackQuery.Message.Chat.Id, $"Заказ не оплачен!");
                    break;
                case "paid":
                    await client.SendTextMessageAsync(callbackQuery.Message.Chat.Id, $"Заказ оплачен!");
                    break;
            }
        }

    }
}
