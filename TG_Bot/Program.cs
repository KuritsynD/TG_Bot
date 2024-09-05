using System;
using System.Diagnostics;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TG_Bot
{
    class Program
    {
        // Клисент для работы с ботом
        private static ITelegramBotClient BotClient;

        // Настройки бота
        private static ReceiverOptions ReceiverOptions;
        
        static async Task Main()
        {
            BotClient = new TelegramBotClient("7331243700:AAFb-L8yFOzOLyBb1rY_ob_TkxEtmQkG-A8");

            ReceiverOptions = new ReceiverOptions
            {
                AllowedUpdates = new[] // Тут указываем типы получаемых Update`ов
                {
                    UpdateType.Message,         
                    UpdateType.CallbackQuery    
                },
                ThrowPendingUpdates = true 
            };

            using var cts = new CancellationTokenSource();

            BotClient.StartReceiving(UpdateHandler, ErrorHandler, ReceiverOptions, cts.Token); // Запускаем бота
        
            var me = await BotClient.GetMeAsync();
            Console.WriteLine($"{me.FirstName} запущен!");
        
            await Task.Delay(-1);

        }
        
        // UpdateHander - обработчик приходящих Update`ов
        private static async Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            try
            {
                switch (update.Type)
                {
                    case UpdateType.Message:
                    {
                        var message = update.Message;

                        var user = message.From;
                        
                        // Выводим на консоль то, что пишут нашему боту, а также небольшую информацию об отправителе
                        Console.WriteLine($"{user.FirstName} ({user.Id}) написал сообщение: {message.Text}");

                        var chat = message.Chat;

                        // Добавляем проверку на тип Message
                        switch (message.Type)
                        {
                            case MessageType.Text:
                            {
                                // Тут создаем нашу клавиатуру
                                var inlineKeyboard = new InlineKeyboardMarkup(
                                    new List<InlineKeyboardButton[]>()
                                    {
                                        new InlineKeyboardButton[] 
                                        {
                                            InlineKeyboardButton.WithUrl("Metanit", "https://metanit.com/"),
                                            InlineKeyboardButton.WithCallbackData("Notepad++", "notepad"), 
                                        },
                                        new InlineKeyboardButton[]
                                        { 
                                            InlineKeyboardButton.WithCallbackData("Rider", "Rider"), 
                                            InlineKeyboardButton.WithCallbackData("Выключить комп", "shutdown"), 
                                        },
                                    });
                                    
                                await botClient.SendTextMessageAsync(
                                    chat.Id,
                                    "Салам Алейкум!",
                                    replyMarkup: inlineKeyboard); 
                                    
                                return;
                            }
                        }
                        return;
                    }

                    case UpdateType.CallbackQuery:
                    {
                        var callbackQuery = update.CallbackQuery;
                        
                        var user = callbackQuery.From;

                        // Выводим на консоль нажатие кнопки
                        Console.WriteLine($"{user.FirstName} ({user.Id}) нажал на кнопку: {callbackQuery.Data}");
                      
                        var chat = callbackQuery.Message.Chat; 
                        
                        // Добавляем блок switch для проверки кнопок
                        switch (callbackQuery.Data)
                        {
                            case "notepad":
                            {
                                await botClient.AnswerCallbackQueryAsync(callbackQuery.Id, "Ща открою)", showAlert: true);
                                
                                Process.Start(@"C:\Program Files\Notepad++\notepad++.exe");
                                return;
                            }
                            
                            case "Rider":
                            {
                                await botClient.AnswerCallbackQueryAsync(callbackQuery.Id,"Ща открою)", showAlert: true);
                                
                                Process.Start(@"C:\Program Files\JetBrains\JetBrains Rider 2023.1.3\bin\rider64.exe");
                                return;
                            }
                            
                            case "shutdown":
                            {
                                await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
                                Process.Start("shutdown", "/s /t 0");
                                return;
                            }
                        }
                        
                        return;
                    }
                }
            
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            } 
        }

        // ErrorHandler - обработчик ошибок, связанных с Bot API
        private static Task ErrorHandler(ITelegramBotClient botClient, Exception error, CancellationToken cancellationToken)
        {
            // Тут создадим переменную, в которую поместим код ошибки и её сообщение 
            var ErrorMessage = error switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => error.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }
    }
}