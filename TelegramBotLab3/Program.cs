using ChatGPT.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBotLab3.Constants;
using TelegramBotLab3.Extensions;

namespace TelegramBotLab3
{
    public class Program
    {
        private static TelegramBotClient BotClient;
        private static string OpenAI_ApiKey = string.Empty;
        private static string TGBot_AccessKey = string.Empty;

        static async Task Main(string[] args)
        {
            InitializeConfigurationKeys();

            BotClient = await StartTelegramBot();

            Console.ReadLine();
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

        private static async Task<TelegramBotClient> StartTelegramBot()
        {
            var bot = new TelegramBotClient(TGBot_AccessKey);

            await bot.SetMyCommandsAsync(new List<BotCommand>
            {
                new() {
                    Command = TextOptions.StartOption,
                    Description = TextOptions.WelcomeMessage
                }
            });

            bot.OnError += HandleErrorAsync;
            bot.OnUpdate += HandleUpdateAsync;
            bot.OnMessage += HandleMessageAsync;

            Console.WriteLine("Bot is up and running.");

            return bot;
        }

        private static async Task HandleMessageAsync(Message message, UpdateType type)
        {
            if (type == UpdateType.Message && message.Text is not null)
            {
                var chatId = message.Chat.Id;

                if (message.Text == TextOptions.StartOption)
                {
                    await BotClient.SendTextMessageToAllOptionsAsync(chatId, TextOptions.WelcomeMessage);
                }
                else
                {
                    var chatGpt = new ChatGpt(OpenAI_ApiKey);

                    var response = await chatGpt.Ask(message.Text);

                    await BotClient.SendTextMessageToBackOptionsAsync(chatId, response);
                }
            }
        }

        private static async Task HandleUpdateAsync(Update update)
        {
            var callbackQuery = update.CallbackQuery;
            var chatId = callbackQuery!.Message!.Chat.Id;

            await(callbackQuery.Data switch
            {
                TextOptions.StudentsOption => BotClient.SendTextMessageToBackOptionsAsync(chatId, TextOptions.StudentsOptionAnswer),
                TextOptions.ITOption => BotClient.SendTextMessageToBackOptionsAsync(chatId, TextOptions.ITOptionAnswer),
                TextOptions.ContactsOption => BotClient.SendTextMessageToBackOptionsAsync(chatId, TextOptions.ContactsOptionAnswer),
                TextOptions.ChatGPTOption => BotClient.SendTextMessageAsync(chatId, TextOptions.MessageToChatGpt),
                TextOptions.BackOption => BotClient.SendTextMessageToAllOptionsAsync(chatId, TextOptions.WelcomeMessage),
                _ => throw new NotImplementedException()
            });

            await BotClient.AnswerCallbackQueryAsync(callbackQuery.Id);
        }

        private static Task HandleErrorAsync(Exception exception, HandleErrorSource source)
        {
            Console.WriteLine($"Something went wrong: {exception.Message}");

            return Task.CompletedTask;
        }
    }
}
