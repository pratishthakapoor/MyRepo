namespace Microsoft.Bot.Sample.ProactiveBot
{
    internal static class Responses
    {
        public const string Features =
            "* Create an Virtual Machine for the user\n\n"
            + "* Raise a Ticket\n\n"
            + "* Allows the user to check the status of the previous ticket\n\n";

        public const string WelcomeMessage =
            "Hi there\n\n"
            + "Currently I have following features  \n"
            + Features
            + "You can type 'Help' to get this information again";

        public const string HelpMessage =
            "I can do the following   \n"
            + Features
            + "What would you like me to do?";
    }
}