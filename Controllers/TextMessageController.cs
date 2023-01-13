using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using UtilityTestBot.Services;

namespace UtilityTestBot.Controllers
{
    internal class TextMessageController
    {
        private readonly ITelegramBotClient _telegramClient;
        private readonly IStorage _memoryStorage;

        public TextMessageController(ITelegramBotClient telegramBotClient, IStorage memoryStorage)
        {
            _telegramClient = telegramBotClient;
            _memoryStorage = memoryStorage;
        }

        public async Task Handle(Message message, CancellationToken ct)
        {
            if (_memoryStorage.GetSession(message.Chat.Id).OptionCode == "Подсчет" && message.Text != "/start")
            {
                await _telegramClient.SendTextMessageAsync(message.Chat.Id, $"В Вашем сообщении {message?.Text?.Length} символов)", cancellationToken: ct);
                await _telegramClient.SendTextMessageAsync(message.Chat.Id, $" Введите символы для подсчета", cancellationToken: ct);
            }
            else if (_memoryStorage.GetSession(message.Chat.Id).OptionCode == "Сложение" && message.Text != "/start")
            {
                string[] splittedNumbers = message.Text.Split(' ');
                int sum = 0;
                foreach (string digit in splittedNumbers)
                {
                    if (int.TryParse(digit, out int number) == true)
                    {
                        sum += number;
                    }
                }
                await _telegramClient.SendTextMessageAsync(message.Chat.Id, $"Сумма чисел: {sum}", cancellationToken: ct);
                await _telegramClient.SendTextMessageAsync(message.Chat.Id, $"Введите цифры через пробел для подсчета суммы", cancellationToken: ct);
            }
            else if (message.Text == "/start")
            {
                // Объект, представляющий кноки
                var buttons = new List<InlineKeyboardButton[]>();
                buttons.Add(new[]
                {
                        InlineKeyboardButton.WithCallbackData($" Подсчет символов" , $"Подсчет"),
                        InlineKeyboardButton.WithCallbackData($" Сложение чисел" , $"Сложение")
                    });

                // передаем кнопки вместе с сообщением (параметр ReplyMarkup)
                await _telegramClient.SendTextMessageAsync(message.Chat.Id, $"<b>  Наш бот умеет считать введенные символы или складывать числа, введенные через пробел.</b> {Environment.NewLine}",
                    cancellationToken: ct, parseMode: ParseMode.Html, replyMarkup: new InlineKeyboardMarkup(buttons));
            }
            else
            {
                await _telegramClient.SendTextMessageAsync(message.Chat.Id, "Введите текст или числа через пробел.", cancellationToken: ct);
            }
        }
    }
}
