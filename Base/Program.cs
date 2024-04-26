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
using PaymentV.Base;

namespace PaymentV
{
    internal class Program
    {
        static void Main(string[] args)
        {
            DotNetEnv.Env.Load();
            var BotToken = DotNetEnv.Env.GetString("BOT_TOKEN"); ;
            var client = new TelegramBotClient(BotToken); // создание бота с нашим токеном

            PaymentApiService paymentApiService = PaymentApiService.Instance;

            client.StartReceiving(Update, Error); // запуск бота
            Console.WriteLine("Бот запущен. Нажмите любую клавишу, чтобы остановить.");

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
            var callbackQuery = update.CallbackQuery;

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
            else if (callbackQuery != null)
            {
                await HandleCallbackQuery(client, callbackQuery);
            }
        }



        async public static Task HandleCallbackQuery(ITelegramBotClient client, CallbackQuery callbackQuery)
        {
            if (callbackQuery.Data.StartsWith("update:"))
            {
                await Console.Out.WriteLineAsync("ds");
                await GetOrder.HandleUpdateOrder(client, callbackQuery);
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
