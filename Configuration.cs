namespace BlogAPI
{
    public static class Configuration
    {
        public static string JwtKey = "ZmVkYWY3ZDg4NjNiNDhlMTk3YjkyODdkNDkyYjcwOGU=";
        public static SmtpConfiguration Smtp = new(); //cria uma estancia da classe 


        //classe para configuração de envio de e-mail
        public class SmtpConfiguration
        {
            public string Host { get; set; }
            public int Port { get; set; } = 25;
            public string UserName { get; set; }
            public string Password { get; set; }
        }
    }
}
