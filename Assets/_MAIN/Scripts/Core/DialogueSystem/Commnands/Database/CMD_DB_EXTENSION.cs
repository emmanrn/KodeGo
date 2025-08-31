namespace COMMANDS
{

    public abstract class CMD_DB_EXTENSION
    {
        public static void Extend(CommandDatabase database) { }
        public static CommandParameters ConvertDataToParams(string[] data, int startingIndex = 0) => new CommandParameters(data, startingIndex);
    }
}