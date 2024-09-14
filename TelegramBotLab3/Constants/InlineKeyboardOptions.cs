using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBotLab3.Constants
{
    public static class InlineKeyboardOptions
    {
        public static InlineKeyboardMarkup AllOptions => new(new InlineKeyboardButton[][]
        {
            [InlineKeyboardButton.WithCallbackData(TextOptions.StudentsOption)],
            [InlineKeyboardButton.WithCallbackData(TextOptions.ITOption)],
            [InlineKeyboardButton.WithCallbackData(TextOptions.ContactsOption)],
            [InlineKeyboardButton.WithCallbackData(TextOptions.ChatGPTOption)]
        });

        public static InlineKeyboardMarkup BackOptions => new(new InlineKeyboardButton[][]
        {
            [InlineKeyboardButton.WithCallbackData(TextOptions.BackOption)],
        });
    }
}
