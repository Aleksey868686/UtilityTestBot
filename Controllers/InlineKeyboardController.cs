using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using UtilityTestBot.Services;

namespace UtilityTestBot.Controllers
{
    internal class InlineKeyboardController
    {
        private readonly IStorage _memoryStorage;
        private readonly ITelegramBotClient _telegramClient;
        private readonly TextMessageController _textMessageController;

        public InlineKeyboardController(ITelegramBotClient telegramBotClient, IStorage memoryStorage, TextMessageController textMessageController)
        {
            _telegramClient = telegramBotClient;
            _memoryStorage = memoryStorage;
            _textMessageController = textMessageController;
        }

        public async Task Handle(CallbackQuery? callbackQuery, CancellationToken ct)
        {
            if (callbackQuery?.Data == null)
                return;

            // Обновление пользовательской сессии новыми данными
            _memoryStorage.GetSession(callbackQuery.From.Id).OptionCode = callbackQuery.Data;

            // Генерим информационное сообщение
            string options = callbackQuery.Data switch
            {
                "Подсчет" => " Подсчет введенных символов",
                "Сложение" => " Сложение чисел, введенных через пробел",
                _ => String.Empty
            };

            // Отправляем в ответ уведомление о выборе
            await _telegramClient.SendTextMessageAsync(callbackQuery.From.Id,
                $"<b>Выбрана опция - {options}.{Environment.NewLine}</b>" +
                $"{Environment.NewLine}Можно поменять в главном меню.", cancellationToken: ct, parseMode: ParseMode.Html);
            if (options == " Подсчет введенных символов")
            {
                await _telegramClient.SendTextMessageAsync(callbackQuery.From.Id,
                $" Введите символы для подсчета", cancellationToken: ct);
            }
            else if (options == " Сложение чисел, введенных через пробел")
            {
                await _telegramClient.SendTextMessageAsync(callbackQuery.From.Id,
                $" Введите числа через пробел для подсчета суммы", cancellationToken: ct);
            }
        }
    }
}
