using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot;
using System.Threading;
using PaymentV.SBP_API;

namespace PaymentV
{
    internal class Program
    {
        static void Main(string[] args)
        {
            DotNetEnv.Env.Load();
            var BotToken = DotNetEnv.Env.GetString("BOT_TOKEN"); ;
            var client = new TelegramBotClient(BotToken); // создание бота с нашим токеном

            client.StartReceiving(Update, Error); // запуск бота
            Console.WriteLine("Бот запущен. Нажмите любую клавишу, чтобы остановить.");
            Console.ReadKey(); // бот работает, пока не будет нажата любая кнопка в консоле 

            MainAsync(args).GetAwaiter().GetResult();
        }

        static async Task MainAsync(string[] args)
        {
            PaymentApiService paymentApiService = new PaymentApiService();
            string orderId = "c5b3fd07-c66b-4f11-9999-1cc5d319f9e3"; // Замените "your_order_id_here" на реальный идентификатор заказа
            String response = await paymentApiService.GetOrderDataAsync(orderId); // Передаем идентификатор заказа методу GetOrderDataAsync()
            Console.WriteLine(response);
            Console.ReadLine();
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
