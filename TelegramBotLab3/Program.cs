using ChatGPT.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBotLab3.Constants;
using TelegramBotLab3.Extensions;

namespace TelegramBotLab3
{
    public class Program
    {
        private static string OpenAI_ApiKey = string.Empty;
        private static string TGBot_AccessKey = string.Empty;

        static async Task Main(string[] args)
        {
            InitializeConfigurationKeys();

            await StartTelegramBot();
        }

        private static void InitializeConfigurationKeys()
        {
            var builder = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddUserSecrets<Program>()
                .Build();

            TGBot_AccessKey = Environment.GetEnvironmentVariable("TelegramBotAccessKey") 
                ?? builder.GetSection("TelegramBotAccessKey").Value!;
            OpenAI_ApiKey = Environment.GetEnvironmentVariable("OpenAI_ApiKey") 
                ?? builder.GetSection("OpenAI_ApiKey").Value!;
        }

        private static async Task StartTelegramBot()
        {
            var bot = new TelegramBotClient(TGBot_AccessKey);

            await bot.SetMyCommandsAsync(new List<BotCommand>
            {
                new() {
                    Command = TextOptions.StartOption,
                    Description = TextOptions.WelcomeMessage
                }
            });

            bot.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync
            );

            Console.WriteLine("Bot is up and running.");
            Console.ReadLine();
        }

        private static async Task HandleUpdateAsync(ITelegramBotClient client, Update update, CancellationToken token)
        {
            if (update.Type == UpdateType.Message && update.Message?.Text is not null)
            {
                var chatId = update.Message.Chat.Id;

                if (update.Message.Text == TextOptions.StartOption)
                {
                    await client.SendTextMessageToAllOptionsAsync(chatId, TextOptions.WelcomeMessage, token);
                }
                else
                {
                    var chatGpt = new ChatGpt(OpenAI_ApiKey);

                    var response = await chatGpt.Ask(update.Message.Text);

                    await client.SendTextMessageToBackOptionsAsync(chatId, response, token);
                }
            }

            if (update.Type == UpdateType.CallbackQuery && update.CallbackQuery is not null)
            {
                var callbackQuery = update.CallbackQuery;
                var chatId = callbackQuery.Message!.Chat.Id;

                await (callbackQuery.Data switch
                {
                    TextOptions.StudentsOption => client.SendTextMessageToBackOptionsAsync(chatId, TextOptions.StudentsOptionAnswer, token),
                    TextOptions.ITOption => client.SendTextMessageToBackOptionsAsync(chatId, TextOptions.ITOptionAnswer, token),
                    TextOptions.ContactsOption => client.SendTextMessageToBackOptionsAsync(chatId, TextOptions.ContactsOptionAnswer, token),
                    TextOptions.ChatGPTOption => client.SendTextMessageAsync(chatId, TextOptions.MessageToChatGpt, cancellationToken: token),
                    TextOptions.BackOption => client.SendTextMessageToAllOptionsAsync(chatId, TextOptions.WelcomeMessage, token),
                    _ => throw new NotImplementedException()
                });

                await client.AnswerCallbackQueryAsync(callbackQuery.Id, cancellationToken: token);
            }
        }

        private static async Task HandleErrorAsync(ITelegramBotClient client, Exception exception, CancellationToken token)
        {
            Console.WriteLine($"Something went wrong: {exception.Message}");

            await Task.Delay(100);
        }
    }
}
