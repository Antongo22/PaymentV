using PaymentV.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot;

namespace PaymentV
{
    public static class State
    {
        public enum BotState
        {
            Default, // стандартное значение
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
            switch (message.Text)
            {
                case "/start":
                    await client.SendTextMessageAsync(message.Chat.Id, "Привет! Добро пожаловать в PaymentV! Тут вы можете проверить статус оплаты.");
                    break;
                case "/help":
                    await client.SendTextMessageAsync(message.Chat.Id, "Вам никто не поможет!)");
                    SetBotState(message.Chat.Id, BotState.Default);
                    break;
                case "/cancel":
                    await client.SendTextMessageAsync(message.Chat.Id, $"Отмена.");
                    break;
                default:
                    await client.SendTextMessageAsync(message.Chat.Id, "Я не понимаю вашей команды. Если возникнут проблемы, " +
                        "можете прописать /help.");
                    break;
            }
        }
    }
}
