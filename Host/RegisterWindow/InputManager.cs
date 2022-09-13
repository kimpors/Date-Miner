using System.Text.RegularExpressions;

namespace Host
{
    public static class InputManager
    {
        public static bool isID { get; private set; }
        public static bool isHASH { get; private set; }
        public static bool isName { get; private set; }
        public static bool isPhone { get; private set; }

        public static bool isCorrectPhone(string phone)
        {
            if (phone.Length > 16 || phone.Length < 10 || phone is null)
                return isPhone = false;

            return isPhone = new Regex(@"\+\d{1,3}\d{9,13}").IsMatch(phone);
        }

        public static bool isCorrectName(string name)
        {
            if (name is null || name.Length == 0)
                return isName = false;

            return isName = new Regex(@"\w{1,50}").IsMatch(name);
        }

        public static bool isCorrectID(string id)
            => isID = id.Length == 8;

        public static bool isCorrectHASH(string hash)
            => isHASH = hash.Length == 32;

        public static bool isAllCorrect()
            => true ? isID && isHASH && isName && isPhone : false;
    }
}
