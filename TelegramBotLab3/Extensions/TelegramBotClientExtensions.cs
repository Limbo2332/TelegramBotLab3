using Telegram.Bot.Types;
using Telegram.Bot;
using TelegramBotLab3.Constants;

namespace TelegramBotLab3.Extensions
{
    public static class TelegramBotClientExtensions
    {
        public static async Task SendTextMessageToAllOptionsAsync(this ITelegramBotClient client, 
            long chatId, 
            string text, 
            CancellationToken token)
        {
            await client.SendTextMessageAsync(
                chatId,
                text,
                replyMarkup: InlineKeyboardOptions.AllOptions,
                cancellationToken: token
            );
        }

        public static async Task SendTextMessageToBackOptionsAsync(this ITelegramBotClient client,
            long chatId,
            string text,
            CancellationToken token)
        {
            await client.SendTextMessageAsync(
                chatId,
                text,
                replyMarkup: InlineKeyboardOptions.BackOptions,
                cancellationToken: token
            );
        }
    }
}
