namespace BotLinkedn.Models
{
    public class LoginModel
    {
        public string Userame { get; private set; }
        public string Password { get; private set; }
        public string FolderName { get; private set; }
        public string KeyWordAddConnections { get; private set; }
        public string KeyWordSearchJobs { get; private set; }
        public string Message { get; private set; }

        public LoginModel(
        string userame,
        string password,
        string folderName,
        string keyWordAddConnections,
        string keyWordSearchJobs,
        string message)
        {
            Userame = userame;
            Password = password;
            FolderName = folderName;
            KeyWordAddConnections = keyWordAddConnections;
            KeyWordSearchJobs = keyWordSearchJobs;
            Message = message;
        }
    }
}