namespace API_Backend_App_Industrializacion.Util
{
    public class Base58
    {
        private static readonly char[] Base58Alphabet = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz".ToCharArray();

        private readonly Random rnd;

        public Base58()
        {
            rnd = new Random((int)DateTime.Now.Ticks);
        }

        public string randomCode(int length)
        {
            string res = "";

            for (int i = 0; i < length; i++)
            {
                res += Base58Alphabet[rnd.Next(Base58Alphabet.Length)];
            }

            return res;
        }
    }
}
