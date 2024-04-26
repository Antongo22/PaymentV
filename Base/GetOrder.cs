using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot;
using PaymentV.SBP_API;

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
                    await client.SendTextMessageAsync(message.Chat.Id, $"Заказ не оплачен!");
                    break;
                case "paid":
                    await client.SendTextMessageAsync(message.Chat.Id, $"Заказ оплачен!");
                    break;
            }


        }
    }
}
