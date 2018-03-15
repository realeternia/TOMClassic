namespace JLM.NetSocket
{
    public class LogHandlerRegister
    {
        public delegate void LogHandler(string msg);

        public static LogHandler Log;
    }
}