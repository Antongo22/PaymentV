using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot;
using System.Threading;

namespace PaymentV
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var client = new TelegramBotClient("7035713707:AAFIOCBRFt6xvjg0P7t6A_GMP5QshJ0QQ-Q"); // создание бота с нашим токеном
            client.StartReceiving(Update, Error); // запуск бота
            Console.WriteLine("Бот запущен. Нажмите любую клавишу, чтобы остановить.");
            Console.ReadKey(); // бот работает, пока не будет нажата любая кнопка в консоле 
        }

        /// <summary>
        /// Основной метод отлова и обработки сообщений
        /// </summary>
        /// <param name="client"></param>
        /// <param name="update"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        async static Task Update(ITelegramBotClient client, Update update, CancellationToken token)
        {
            var message = update.Message;

            Console.WriteLine($"{message?.Chat.FirstName ?? "-no name-"}\t\t|\t{message?.Text ?? "-no text-"}");

            if (message?.Text != null)
            {
                switch (State.GetBotState(message.Chat.Id))
                {
                    case State.BotState.Default:
                        await State.HandleDefaultState(client, message);
                        break;                   

                }
            }
        }

        /// <summary>
        /// Метод отлова ошибок
        /// </summary>
        /// <param name="client"></param>
        /// <param name="exception"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private static Task Error(ITelegramBotClient client, Exception exception, CancellationToken token)
        {
            throw new NotImplementedException();
        }       
    }
}
