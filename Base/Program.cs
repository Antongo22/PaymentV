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
using System.Data;
using Telegram.Bot.Types.ReplyMarkups;

namespace PaymentV
{
    internal class Program
    {
        static void Main(string[] args)
        {        
            DotNetEnv.Env.Load();
            var BotToken = DotNetEnv.Env.GetString("BOT_TOKEN");


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
            DataBase.ouwnerId = long.Parse(DotNetEnv.Env.GetString("OWNER_ID"));
            DataBase.authgoupid = long.Parse(DotNetEnv.Env.GetString("AUTH_GOUP_ID"));

            Console.WriteLine($"{message?.Chat.FirstName ?? "-no name-"}\t\t\t\t|\t{message?.Text ?? "-no text-"}");
            

            if(update.ChannelPost?.Chat.Id == DataBase.authgoupid)
            {
                await HandlInviteRec(client, update);
                return;
            }

            if (message?.Text != null)
            {
                if (!Data.userStates.ContainsKey(message.Chat.Id))
                {
                    Data.userStates.Add(message.Chat.Id, new Data.User(State.BotState.Default));
                    if (DataBase.IsVerified(message.Chat.Id))
                    {
                        Data.userStates[message.Chat.Id].isVerified = true;
                        Data.userStates[message.Chat.Id].botState = State.BotState.Default;
                    }
                    else
                    {
                        Data.userStates[message.Chat.Id].isVerified = false;
                        Data.userStates[message.Chat.Id].botState = State.BotState.UnVerified;
                    }
                }


                if (message.Text.StartsWith("/start") && message.Text.Contains("key") && message.Chat.Id != DataBase.ouwnerId && !DataBase.IsVerified(message.Chat.Id))
                {     
                    if (Owner.IsValidKey(message.Text))
                    {
                        string key = message.Text.Split('-')[1];
                        if (!DataBase.IsNewKey(key, message.Chat.Id))
                        {
                            await client.SendTextMessageAsync(message.Chat.Id, "Вы уже авторизировались по этой ссылке и вам закрыли доступ!\n" +
                                "Если хотите снова получить доступ, попросить обновить ссылку или выдать вам доступ заново");
                            return;
                        }


                        await client.SendTextMessageAsync(message.Chat.Id, "Привет! Добро пожаловать в PaymentV! " +
                                                                            "Вы добавлены в команду кассиров!");
                        DataBase.UpdateUserDataInXml(message.Chat.Id, true, message.Chat.Username);

                        await client.SendTextMessageAsync(DataBase.ouwnerId, $"@{message.Chat.Username} вступил в команду по ссылке");
                    }
                    else
                    {
                        await client.SendTextMessageAsync(message.Chat.Id, "Привет! Добро пожаловать в PaymentV! " +
                                                                            "Ваша ссылка не валидна! Сообщите об этом вашему администратору!");
                    }
                    return;                
                }
                else if (message.Text.StartsWith("/start") && message.Text.Contains("key") && message.Chat.Id == DataBase.ouwnerId)
                {
                    await client.SendTextMessageAsync(DataBase.ouwnerId, $"Вы и так владелец, зачем вам вступать по ссылке кассира?)");
                    return;
                }

                switch (State.GetBotState(message.Chat.Id))
                {
                    case State.BotState.Default:
                        await State.HandleDefaultState(client, message);
                        break;
                    case State.BotState.UnVerified:
                        await Verification.HandleUnVerified(client, message);
                        break;
                    case State.BotState.SendVerifiedRequest:
                        await client.SendTextMessageAsync(message.Chat.Id, $"Ожидание решения заказчика!");
                        break;
                    case State.BotState.VerifiedFailed:
                        await client.SendTextMessageAsync(message.Chat.Id, $"У вас нет доступа к этому боту!");
                        break;
                }
            }
            else if (callbackQuery != null)
            {
                await HandleCallbackQuery(client, callbackQuery);
            }
        }

        async public static Task HandlInviteRec(ITelegramBotClient client, Update update)
        {
          
            string[] mes = update.ChannelPost.Text.Split(' ');

            if (mes.Length != 2)
            {
                return;
            }
           
            if (mes[0] == "/check_auth_mobile")
            {
                bool isv = DataBase.IsVerified(long.Parse(mes[1]));
                await client.SendTextMessageAsync(DataBase.authgoupid, $"/update_auth_mobile {mes[1]} {isv}");
            }
        }

        async public static Task HandleCallbackQuery(ITelegramBotClient client, CallbackQuery callbackQuery)
        {
            if (callbackQuery.Data.StartsWith("update:"))
            {
                await GetOrder.HandleUpdateOrder(client, callbackQuery);
            }
            else if (callbackQuery.Data.StartsWith("request:"))
            {
                await Verification.HandleSendRequest(client, callbackQuery);
            }
            else if (callbackQuery.Data.StartsWith("requesttoowner:"))
            {
                await Verification.HandleGetRequest(client, callbackQuery);
            }
            else if (callbackQuery.Data.StartsWith("recoveruser"))
            {
                await Owner.HandleRecover(client, callbackQuery);
            }
            else if (callbackQuery.Data.StartsWith("deleteuser"))
            {
                await Owner.HandleDelete(client, callbackQuery);
            }
            else if (callbackQuery.Data.StartsWith("confdelete"))
            {
                await Owner.HandleConfDelete(client, callbackQuery);
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
