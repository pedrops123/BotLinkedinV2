namespace BotLinkedn.Models
{
    public class LoginModel
    {
        public string Userame { get; private set; }
        public string Password { get; private set; }
        public string FolderName { get; private set; }
        public string KeyWord { get; private set; }

        public LoginModel(
            string userame,
            string password,
            string folderName,
            string keyWord)
        {
            Userame = userame;
            Password = password;
            FolderName = folderName;
            KeyWord = keyWord;
        }
    }
}