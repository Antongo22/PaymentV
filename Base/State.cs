using PaymentV.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace PaymentV
{
    public static class State
    {
        public enum BotState
        {
            Default, // стандартное значение
            GetOrder, // получение заказа
            OrderNotPaid, // статус того, то заказ не оплачен
            UnVerified, // состояние, когда невкрефицирован
            SendVerifiedRequest, // состояние отправки запроса на добавление в команду
            VerifiedFailed, // состояние, когда отклонили верефикацию

            StartOwner, 
        }

        /// <summary>
        /// Передача текущего состояния для каждого пользователя
        /// </summary>
        /// <param name="chatID"></param>
        /// <returns></returns>
        public static BotState GetBotState(long chatID)
        {
            if (!Data.userStates.ContainsKey(chatID))
            {
                Data.userStates.Add(chatID, new Data.User(BotState.Default));
            }
            return Data.userStates[chatID].botState;
        }


        /// <summary>
        /// Сохранение текущего состояния для каждого пользователя
        /// </summary>
        /// <param name="chatID"></param>
        /// <param name="botState"></param>
        public static void SetBotState(long chatID, BotState botState) => Data.userStates[chatID].botState = botState;


        /// <summary>
        /// Обработчик состояния Default
        /// </summary>
        /// <param name="client"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        async public static Task HandleDefaultState(ITelegramBotClient client, Message message)
        {


            switch (message.Text.ToLower())
            {
                case "/start":
                    if(message.Chat.Id == DataBase.ouwnerId)
                    {
                        ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
                        {
                            new KeyboardButton[] { "Получить ссылку", "Новая ссылка" },
                        })
                        {
                            ResizeKeyboard = true
                        };

                        await client.SendTextMessageAsync(message.Chat.Id, "Добро пожаловать в PaymentV! " +
                            "Сейчас мы вышлем вам письмо для приглашения сотрудников", replyMarkup: replyKeyboardMarkup);

                        string link = Owner.GetKey();

                        if (String.IsNullOrEmpty(link))
                        {
                            link = Owner.GenerateLink();
                        }
                        else
                        {
                            link = "https://t.me/NI_PaymentV_Bot?start=key-" + link;
                        }

                        Console.WriteLine(link);
                       
                        string messageText = $"Вас приглашают в PaymentV!";

                        string[] words = messageText.Split(' ');
                        words[words.Length - 1] = $"<a href='{link}'>{words[words.Length - 1]}</a>";
                        string hyperlinkedMessage = string.Join(" ", words);

                        await client.SendTextMessageAsync(message.Chat.Id, hyperlinkedMessage, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
                    }                   
                    else
                    {
                        await client.SendTextMessageAsync(message.Chat.Id, "Привет! Добро пожаловать в PaymentV! " +
                                                                            "Тут вы можете проверить статус оплаты вбив его id.");
                    }
                    break;
                case "/newlink":
                case "новая ссылка":
                    if (message.Chat.Id == DataBase.ouwnerId)
                    {
                        await client.SendTextMessageAsync(message.Chat.Id, "Сейчас мы вышлем вам письмо для приглашения сотрудников с новой ссылкой");

                        string link = Owner.GenerateLink();
                        string messageText = $"Вас приглашают в PaymentV!";

                        string[] words = messageText.Split(' ');
                        words[words.Length - 1] = $"<a href='{link}'>{words[words.Length - 1]}</a>";
                        string hyperlinkedMessage = string.Join(" ", words);

                        await client.SendTextMessageAsync(message.Chat.Id, hyperlinkedMessage, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
                    }
                    else
                    {
                        await client.SendTextMessageAsync(message.Chat.Id, "У вас нет доступа к этой команде");
                    }
                    break;
                case "получить ссылку":
                case "/getlink":
                    if (message.Chat.Id == DataBase.ouwnerId)
                    {
                        await client.SendTextMessageAsync(message.Chat.Id, "Сейчас мы вышлем вам письмо для приглашения сотрудников");

                        string link = Owner.GetKey();

                        if (String.IsNullOrEmpty(link))
                        {
                            link = Owner.GenerateLink();
                        }
                        else
                        {
                            link = "https://t.me/NI_PaymentV_Bot?start=key-" + link;
                        }

                        string messageText = $"Вас приглашают в PaymentV!";

                        string[] words = messageText.Split(' ');
                        words[words.Length - 1] = $"<a href='{link}'>{words[words.Length - 1]}</a>";
                        string hyperlinkedMessage = string.Join(" ", words);

                        await client.SendTextMessageAsync(message.Chat.Id, hyperlinkedMessage, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
                    }
                    else
                    {
                        await client.SendTextMessageAsync(message.Chat.Id, "У вас нет доступа к этой команде");
                    }

                    break;
                case "/help":
                    await client.SendTextMessageAsync(message.Chat.Id, "Вам никто не поможет!)");
                    SetBotState(message.Chat.Id, BotState.Default);
                    break;
                case "/cancel":
                    await client.SendTextMessageAsync(message.Chat.Id, $"Отмена.");
                    break;
                case "/myid":
                    await client.SendTextMessageAsync(message.Chat.Id, $"Ваш id - {message.Chat.Id}");
                    break;
                case "1488":
                    await client.SendTextMessageAsync(message.Chat.Id, $"Заказ опалачивать не просим");
                    break;
                default:
                    await GetOrder.HandleGetOrder(client, message);
                    break;
            }
        }
    }
}
