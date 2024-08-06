using KeePassLib.Utility;

namespace KeePassCsvImporter
{
    /// <summary>
    /// Klasa narzędziowa do tworzenia różnych typów wiadomości.
    /// </summary>
    internal abstract class MessageCreator
    {
        public static void CreateInfoMessage(string title, params string[] content)
        {
            MessageService.ShowInfoEx(title, string.Join("\n", content));
        }

        public static void CreateWarningMessage(string content)
        {
            MessageService.ShowWarning(content);
        }

        public static void CreateErrorMessage(params string[] content)
        {
            MessageService.ShowFatal(string.Join("\n", content));
        }

        public static bool CreateQuestionMessage(string title, string content)
        {
            return MessageService.AskYesNo(content, title);
        }
    }
}