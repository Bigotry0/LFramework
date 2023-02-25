namespace LFramework
{
    public interface ICanSendCommand : IBelongToArchitecture
    {
        
    }

    public static class CanSendCommandExtension
    {
        public static void SendCommand<T>(this ICanSendCommand self) where T : class, ICommand, new()
        {
            self.GetArchitecture().SendCommand<T>();
        }

        public static void SendCommand<T>(this ICanSendCommand self, T command) where T : class, ICommand
        {
            self.GetArchitecture().SendCommand(command);
        }
    }
}