using PaymentV.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentV
{
    public static class DataBase
    {
        public static readonly long ouwnerId = 860491148;

        public static bool IsVerified(long chatId)
        {
            //if(chatId == 860491148) return true;

            return false;
        }

        public static void SetVerification(long chatid)
        {
            Data.userStates[chatid].botState = State.BotState.Default;
            Data.userStates[chatid].isVerified = true;
        }

        public static void SetNotVerification(long chatid)
        {
            Data.userStates[chatid].botState = State.BotState.VerifiedFailed;
            Data.userStates[chatid].isVerified = false;
        }
    }
}
