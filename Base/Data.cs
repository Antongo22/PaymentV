using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentV.Base
{
    internal class Data
    {
        /// <summary>
        /// Данные, которые хранятся у юзера
        /// </summary>
        public class User
        {
            public User(State.BotState botState)
            {
                this.botState = botState;
            }
            public State.BotState botState; // состояние 
        }

        // Хранение состояния для каждого пользователя
        public static Dictionary<long, User> userStates = new Dictionary<long, User>();



    }
}
