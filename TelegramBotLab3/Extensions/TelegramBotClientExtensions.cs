using Telegram.Bot.Types;
using Telegram.Bot;
using TelegramBotLab3.Constants;

namespace TelegramBotLab3.Extensions
{
    public static class TelegramBotClientExtensions
    {
        public static async Task SendTextMessageToAllOptionsAsync(this ITelegramBotClient client, 
            long chatId, 
            string text)
        {
            await client.SendTextMessageAsync(
                chatId,
                text,
                replyMarkup: InlineKeyboardOptions.AllOptions
            );
        }

        public static async Task SendTextMessageToBackOptionsAsync(this ITelegramBotClient client,
            long chatId,
            string text)
        {
            await client.SendTextMessageAsync(
                chatId,
                text,
                replyMarkup: InlineKeyboardOptions.BackOptions
            );
        }
    }
}
