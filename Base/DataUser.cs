namespace PaymentV.Base
{
    public class DataUser
    {
        readonly public string ChatID;
        readonly public string Name;
        readonly public bool IsVerified;

        public DataUser(string ChatID, string Name, bool IsVerified)
        {
            this.ChatID = ChatID;
            this.Name = Name;
            this.IsVerified = IsVerified;
        }
    }
}
