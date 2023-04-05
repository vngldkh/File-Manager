using System;
using System.Text;
using System.IO;
using System.Collections.Generic;

namespace FileManager
{
    /// <summary>
    /// Главный класс программы.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Основной метод программы.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            do
            {
                // Обрабатываем возможные ошибки доступа.
                try
                {
                    Console.Clear();
                    Console.WriteLine("Добро пожаловать в \"Файловый менеджер\".\n\n" +
                        "Принцип работы:\n" +
                        "1) Вам предлагается посмотреть список доступных дисков или выбрать путь.\n" +
                        "2) Если Вы установите путь к какой-либо директории,\n" +
                        "   Вам будут доступны операции над файлами в текущей директории.\n\n" +
                        "Для выхода из программы нажмите ESC.\n" +
                        "Для начала работы - любую другую клавишу.");
                    if (Console.ReadKey().Key == ConsoleKey.Escape)
                        break;
                    // Вызываем метод, инициализирующий процесс работы менеджера.
                    Init();
                } 
                // Если у пользователя не будет доступа к чему-либо, и будет выброшено исключение, обработаем его.
                catch 
                {
                    Console.Clear();
                    Console.WriteLine("Ошибка доступа!\n" +
                        "Для возвращения на стартовую страницу нажмите любую клавишу.");
                    Console.ReadKey();
                }
            } while (true);
        }

        /// <summary>
        /// Метод инициализирует работу файлового менеджера.
        /// </summary>
        static void Init()
        {
            // Путь к текущей директории.
            string path;
            // Переменная показывает нужно ли завершить работу менеджера досрочно.
            bool exit = false;
            // Показывает пользователю доступные опции и предлагаем выбрать одну из них.
            var option = DriveOptions(ref exit);
            if (exit)
                return;
            // Выполняем соответсвующее выбранной опции действие.
            switch (option)
            {
                case 1:
                    DisplayDrives();  
                    break;
                case 2:
                    do
                    {
                        path = GetPath(ref exit);
                        if (exit) return;
                    } while (DirectoryOptions(path, ref exit));
                    break;
                case 3:
                    exit = true;
                    break;
            }
            if (exit)
                return;
            Console.WriteLine("Для продолжения нажмите любую клавишу.");
            Console.ReadKey();
        }

        /// <summary>
        /// Метод предлагает пользователю 
        /// 1) Посмотреть список дисков.
        /// 2) Установить путь к директории.
        /// 3) Вернуться.
        /// </summary>
        /// <param name="exit"> Переменная показывает, нужно ли вернуться на стартовую страницу. </param>
        /// <returns> Возвращается номер выбранной опции. </returns>
        static int DriveOptions(ref bool exit)
        {
            Console.Clear();
            Console.WriteLine("Список доступных действий:\n1) Просмотр списка доступных дисков.\n" +
                "2) Выбор диска/директории.\n3) Вернуться на стартовую страницу\n" +
                "Для работы с файлами и директориями выберите диск/директорию.\n" +
                "Для выбора действия введите соответствующий номер:");
            bool inputIsCorrect;
            int option;
            // Цикл повторяется пока ввод не будет корректным, или пользователь не захочет вернуться.
            do
            {
                Console.Write("\b>>> ");
                inputIsCorrect = int.TryParse(Console.ReadLine(), out option) && option >= 1 && option <= 3;
                if (!inputIsCorrect)
                {
                    Console.WriteLine("Выбранной опции не существует.\n" +
                        "Для вовзращения на стартовую страницу нажмите ESC.\nДля повторного ввода - любую другую клавишу.");
                    exit = inputIsCorrect = Console.ReadKey().Key == ConsoleKey.Escape;
                }
            } while (!inputIsCorrect);
            return option;
        }

        /// <summary>
        /// Метод предлагает пользователю действия с файлами внутри директории.
        /// </summary>
        /// <param name="path"> Путь к текущей директории. </param>
        /// <param name="exit"> Показывает, хочет ли пользователь вернуться на стартовую страницу. </param>
        /// <returns> Возвращает значение типа bool и показывает, нужно ли заново устанавливать путь. </returns>
        static bool DirectoryOptions(string path, ref bool exit)
        {
            Console.Clear();
            Console.WriteLine("Список доступных действий:\n1) Переход в другую директорию.\n" +
                "2) Просмотр списка папок в текущей директории (по маске).\n" +
                "3) Просмотр списка файлов в текущей директории (по маске).\n" +
                "4) Вывод содержимого файла в консоль (в выбранной кодировке).\n" +
                "5) Конкатенация содержимого нескольких файлов и вывод результата в консоль (в выбранной кодировке).\n" +
                "6) Копирование файла.\n7) Перемещение файла.\n8) Удаление файла.\n" +
                "9) Ввод текста через консоль и сохранение файла в выбранной кодирове.\n" +
                "10) Вовзрат на стартовую страницу.\n" +
                "Для работы с файлами и директориями выберите диск/директорию.\n" +
                "Для выбора действия введите соответствующий номер:");
            var option = ChooseDirectoryOption();
            return CheckDirectoryOption(path, ref exit, option);
        }

        /// <summary>
        /// Метод позволяет пользователю выбрать опции ввода.
        /// </summary>
        /// <returns> Возвращает кортеж с опциями ввода. </returns>
        static (string, bool) ChooseInputOptions()
        {
            Console.Clear();
            Console.WriteLine("Введите маску (чтобы вывести файлы/папки с любыми именами введите \"*\"):");
            Console.Write("\b>>> ");
            // Маска для имён.
            var pattern = Console.ReadLine();
            Console.Clear();
            Console.WriteLine("Хотите ли вы также увидеть файлы/папки из поддерикторий?\n" +
                "Если да - нажмите Y, если нет - нажмите любую другую клавишу.");
            // Опция поиска (показывает, нужно ли смотреть поддирективы).
            var allDirectories = Console.ReadKey().Key == ConsoleKey.Y;
            return (pattern, allDirectories);
        }

        /// <summary>
        /// В соответствии с выбранным действием вызываем необходимый методж
        /// </summary>
        /// <param name="path"> Путь к текущей директории. </param>
        /// <param name="exit"> Показывает, нужно ли вернуться на стартовую страницу. </param>
        /// <param name="option"> Номер выбранного действия. </param>
        /// <returns> Вовзращает true, если необходимо заново установить путь. </returns>
        static bool CheckDirectoryOption(string path, ref bool exit, int option)
        {
            (string, bool) inputOptions;
            switch (option)
            {
                // Установить путь заново.
                case 1:
                    return true;
                // Вывести папки в текущей директории.
                case 2:
                    inputOptions = ChooseInputOptions();
                    DisplayDirectories(path, inputOptions.Item1, inputOptions.Item2);
                    return false;
                // Вывести файлы в текущей директории.
                case 3:
                    inputOptions = ChooseInputOptions();
                    DisplayFiles(path, inputOptions.Item1, inputOptions.Item2);
                    return false;
                // Вывести содержимое файла из текущей директории.
                case 4:
                    DisplayText(path, ref exit);
                    return false;
                // Вывести содержимое нескольких файлов.
                case 5:
                    ReadFiles(path, ref exit);
                    return false;
                // Скопировать файл.
                case 6:
                    CopyFile(path, ref exit);
                    return false;
                // Переместить файл.
                case 7:
                    MoveFile(path, ref exit);
                    return false;
                // Удалить файл.
                case 8:
                    DeleteFile(path, ref exit);
                    return false;
                // Ввести файл и сохранить в выбранной кодировке.
                case 9:
                    InputFile(path, ref exit);
                    return false;
                // Вернуться на стартовую страницу.
                default:
                    exit = true;
                    return false;
            }
        }

        /// <summary>
        /// Метод предлагает пользователю выбрать одну из операций над файлами.
        /// </summary>
        /// <returns> Возвращает номер выбранной опции. </returns>
        static int ChooseDirectoryOption()
        {
            bool inputIsCorrect;
            int option;
            do
            {
                Console.Write("\b>>> ");
                inputIsCorrect = int.TryParse(Console.ReadLine(), out option) && option >= 1 && option <= 9;
                if (!inputIsCorrect)
                {
                    Console.WriteLine("Выбранной опции не существует.\n" +
                        "Для возврата на стартовую страницу нажмите ESC.\nДля повторного ввода - любую другую клавишу");
                    inputIsCorrect = Console.ReadKey().Key == ConsoleKey.Escape;
                }
            } while (!inputIsCorrect);
            return option;
        }

        /// <summary>
        /// Выводит список доступных дисков в консоль.
        /// </summary>
        /// <returns> Вовзращает список доступных действий. </returns>
        static string[] DisplayDrives()
        {
            Console.Clear();
            Console.WriteLine("Список доступных дисков: ");
            var drives = Environment.GetLogicalDrives();
            for (int i = 0; i < drives.Length; i++)
                Console.WriteLine("{0}) {1}", i + 1, drives[i]);
            return drives;
        }

        /// <summary>
        /// Метод позволяет пользователю выбрать диск.
        /// </summary>
        /// <param name="path"> Путь к текущей директории. </param>
        /// <param name="exit"> Показывает, хочет ли пользователь вернуться на стартовую страницу. </param>
        static void ChooseDrive(ref string path, ref bool exit)
        {
            // Выводим список доступных дисков.
            var drives = DisplayDrives();
            Console.WriteLine($"{drives.Length + 1}) Вернуться на стартовую страницу.");
            Console.WriteLine("\nДля выбора диска или выхода введите соответствующий номер:");
            bool inputIsCorrect = false;
            int driveNumber;
            // Цикл повторяется, пока ввод не будет корректным, или пользователь не захочет вернуться.
            do
            {
                Console.Write("\b>>> ");
                var input = Console.ReadLine();
                inputIsCorrect = int.TryParse(input, out driveNumber) && driveNumber >= 1 && driveNumber <= drives.Length + 1;
                if (!inputIsCorrect)
                {
                    Console.WriteLine("Введённая строка не является числом из списка доступных!\n" +
                        "Для возврата на стартовую страницу нажмите ESC, для повторного ввода - любую другую клавишу.");
                    exit = inputIsCorrect = Console.ReadKey().Key == ConsoleKey.Escape;
                }                    
            } while (!inputIsCorrect);
            exit &= driveNumber == drives.Length + 1;
            if (exit) return;
            path = drives[driveNumber - 1];
        }

        /// <summary>
        /// Метод выводит список доступных директорий в текущей папке с заданными опциями.
        /// </summary>
        /// <param name="path"> Путь к текущей директории. </param>
        /// <param name="pattern"> Маска поиска. </param>
        /// <param name="allDirectories"> Опция поиска (показывает, нужно ли искать в поддиректориях). </param>
        /// <returns> Вовзращает список подходящих по параметрам директорий. </returns>
        static string[] DisplayDirectories(string path, string pattern, bool allDirectories)
        {
            Console.Clear();
            Console.WriteLine("Текущая директория: {0}", path);
            Console.WriteLine("Список доступных директорий в этой папке:");
            // Получаем  список подходящих директорий.
            var directories = ListOfDirectories(path, pattern, allDirectories);
            var posOfLastSeparator = directories[0].LastIndexOf(Path.DirectorySeparatorChar);
            for (int i = 0; i < directories.Length; i++)
            {
                var folder = directories[i];
                var fileName = folder[(posOfLastSeparator + 1)..];
                Console.WriteLine($"{i + 1}) {fileName}");
            }
            return directories;
        }

        /// <summary>
        /// Метод выводит список доступных файлов в текущей папке с заданными опциями.
        /// </summary>
        /// <param name="path"> Путь к текущей директории. </param>
        /// <param name="pattern"> Маска поиска. </param>
        /// <param name="allDirectories"> Опция поиска (показывает, нужно ли искать в поддиректориях) </param>
        /// <returns> Возвращает списко подходящих по параметрам файлов. </returns>
        static string[] DisplayFiles(string path, string pattern, bool allDirectories)
        {
            Console.Clear();
            Console.WriteLine("Текущая директория: {0}", path);
            Console.WriteLine("Список доступных файлов в этой папке:");
            // Получаем список файлов.
            var files = ListOfFiles(path, pattern, allDirectories);
            var posOfLastSeparator = files[0].LastIndexOf(Path.DirectorySeparatorChar);
            for (int i = 0; i < files.Length; i++)
            {
                var file = files[i];
                var fileName = file[(posOfLastSeparator + 1)..];
                Console.WriteLine($"{i + 1}) {fileName}");
            }
            return files;
        }

        /// <summary>
        /// Метод получает список доступных папок в текущей директории с заданными параметрами.
        /// </summary>
        /// <param name="path"> Путь к текущей директории. </param>
        /// <param name="pattern"> Маска поиска. </param>
        /// <param name="allDirectories"> Опция поиска (показывает, нужно ли искать в поддиректориях) </param>
        /// <returns> Возвращает список папок, подходящих по параметрам. </returns>
        static string[] ListOfDirectories(string path, string pattern, bool allDirectories)
            => Directory.GetDirectories(path, pattern, allDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

        /// <summary>
        /// Метод получает список доступных файлов в текущей директории с заданными параметрами.
        /// </summary>
        /// <param name="path"> Путь к текущей директории. </param>
        /// <param name="pattern"> Маска поиска. </param>
        /// <param name="allDirectories"> Опция поиска (показывает, нужно ли искать в поддиректориях) </param>
        /// <returns> Возвращает список файлов, подходящих по параметрам. </returns>
        static string[] ListOfFiles(string path, string pattern, bool allDirectories)
            => Directory.GetFiles(path, pattern, allDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

        /// <summary>
        /// Метод позволяет пользователю установить путь вручную.
        /// </summary>
        /// <param name="path"> Путь к текущей директории. </param>
        /// <param name="exit"> Паказывает, нужно ли вернуться на стартовую страницу. </param>
        static void ChoosePathDirectly(ref string path, ref bool exit)
        {
            Console.Clear();
            Console.WriteLine("Введите путь директории, к которой Вы хотите перейти: ");
            exit = false;
            bool inputIsCorrect;
            string userPath;
            // Вводим, пока путь не будет корректным, или пользователь не захочет вернуться.
            do
            {
                Console.Write("\b>>> ");
                userPath = Console.ReadLine();
                inputIsCorrect = Directory.Exists(userPath);
                if (!inputIsCorrect)
                {
                    Console.WriteLine("По введённому пути папки не существует!\n" +
                        "Чтобы вернуться на стартовую страницу нажмите ESC.\n" +
                        "Чтобы ввести путь ещё раз - любую другую клавишу.");
                    if (Console.ReadKey().Key == ConsoleKey.Escape)
                        exit = inputIsCorrect = true;
                }
            } while (!inputIsCorrect);
            if (exit)
                return;
            path = userPath;
            Console.WriteLine("Путь к директории успешно установлен. Для продолжения нажмите любую клавишу.");
            Console.ReadKey();
        }

        /// <summary>
        /// Метод позволяет выбрать файл в текущей директории.
        /// </summary>
        /// <param name="path"> Путь к текущей директории. </param>
        /// <param name="exit"> Показывает, нужно ли вернуться на стартовую страницу. </param>
        /// <returns> Возвращает полный путь к выбранному файлу. </returns>
        static string ChooseFile(string path, ref bool exit)
        {
            Console.Clear();
            Console.WriteLine("1) Выбрать файл из списка.\n2) Ввести имя вручную (рекомендуется, если файлов много).\n" +
                "3) Вернуться на стартовую страницу.\n" +
                "Для продолжения выберите один из предложенных вариантов.");
            bool inputIsCorrect;
            int option;
            // Цикл будет повторяться, пока ввод не будет корректным, или пользователь не захочет вернуться.
            do
            {
                Console.Write("\b>>> ");
                inputIsCorrect = int.TryParse(Console.ReadLine(), out option) && option >= 1 && option <= 3;
                if (!inputIsCorrect)
                {
                    Console.WriteLine("Выбранной опции не существует!\n" +
                        "Чтобы вернуться на стартовую страницу, нажмите ESC.\nЧтобы повторить ввод - любую другую клавишу.");
                    exit = inputIsCorrect = Console.ReadKey().Key == ConsoleKey.Escape;
                }
            } while (!inputIsCorrect);
            string filePath = null;
            // Выполняем действие, соответствующее введённому числу.
            switch (option)
            {
                case 1:
                    filePath = ChooseFileAutomaticly(path, ref exit);
                    break;
                case 2:
                    filePath = ChooseFileManually(path, ref exit);
                    break;
                case 3:
                    exit = true;
                    break;
            }
            return filePath;
        }

        /// <summary>
        /// Позволяет ввести имя файла вручную.
        /// </summary>
        /// <param name="path"> Путь к текущей директории. </param>
        /// <param name="exit"> Показывает, нужно ли вернуться на стартовую страницу. </param>
        /// <returns> Возвращает полный путь к выбранному файлу. </returns>
        static string ChooseFileManually(string path, ref bool exit)
        {
            Console.Clear();
            Console.WriteLine("Текущая директория: {0}", path);
            Console.WriteLine("Введите имя файла из текущей директории: ");
            exit = false;
            bool inputIsCorrect;
            string fileName, newPath;
            // Цикл будет повторяться, пока ввод не будет корректным, или пользователь не захочет вернуться.
            do
            {
                Console.Write("\b>>> ");
                fileName = Console.ReadLine();
                newPath = path + Path.DirectorySeparatorChar + fileName;
                inputIsCorrect = File.Exists(newPath);
                if (!inputIsCorrect)
                {
                    Console.WriteLine("В текущей директории не существует файла с введённым именем!\n" +
                        "Чтобы вернуться на стартовую страницу нажмите ESC.\n" +
                        "Чтобы ввести имя файла ещё раз - любую другую клавишу.");
                    if (Console.ReadKey().Key == ConsoleKey.Escape)
                        exit = inputIsCorrect = true;
                }
            } while (!inputIsCorrect);
            if (exit)
                return null;
            return newPath;
        }

        /// <summary>
        /// Метод позволяет пользователю выбрать файл из списка доступных.
        /// </summary>
        /// <param name="path"> Путь к текущей директории. </param>
        /// <param name="exit"> Показывает, нужно ли вернуться на стартовую страницу. </param>
        /// <returns></returns>
        static string ChooseFileAutomaticly(string path, ref bool exit)
        {
            Console.Clear();
            Console.WriteLine("Текущая директория: {0}", path);
            Console.WriteLine("Список доступных файлов:");
            var files = DisplayFiles(path, "*", false);
            Console.WriteLine($"{files.Length + 1}) Вернуться на стартовую страницу");
            Console.WriteLine("Чтобы выбрать файл или вернуться на стартовую страницу, введите соответствующее число: ");
            bool inputIsCorrect;
            // Выбранный номер файла.
            int noOfFile;
            // Цикл повторяется, пока ввод не будет корректным, или пользователь не захочет вернуться.
            do
            {
                Console.Write("\b>>> ");
                inputIsCorrect = int.TryParse(Console.ReadLine(), out noOfFile) && noOfFile >= 1 && noOfFile <= files.Length + 1;
                if (!inputIsCorrect)
                {
                    Console.WriteLine("Выбранной опции не существует!\n" +
                        "Чтобы вернуться на стартовую страницу, нажмите ESC.\nЧтобы повторить ввод - любую другую клавишу.");
                    exit = inputIsCorrect = Console.ReadKey().Key == ConsoleKey.Escape;
                    continue;
                }
                exit = noOfFile == files.Length + 1;
            } while (!inputIsCorrect);
            if (exit)
                return null;
            return files[noOfFile - 1];
        }

        // Доступные кодировки.
        static readonly string[] encodes = { "UTF-8", "Unicode", "UTF-16", "UTF-32" };

        /// <summary>
        /// Метод выводит содержимое файла в выбранной кодировке.
        /// </summary>
        /// <param name="path"> Путь к текущей директории. </param>
        /// <param name="exit"> Показывает, нужно ли вернуться на стартовую страницу. </param>
        static void DisplayText(string path, ref bool exit)
        {
            var file = ChooseFile(path, ref exit);
            var noOfEncoding = ChooseEncoding(ref exit);
            if (exit) return;
            Console.Clear();
            var fileName = file[(file.LastIndexOf(Path.DirectorySeparatorChar) + 1)..];
            Console.WriteLine($"Содержимое файла {fileName}, прочитанного в кодировке {encodes[noOfEncoding]}");
            var sr = new StreamReader(file, Encoding.GetEncoding(encodes[noOfEncoding]));
            while (!sr.EndOfStream)
                Console.WriteLine(sr.ReadLine());
        }

        /// <summary>
        /// Метод позволяет выбрать одну из доступных кодировок.
        /// </summary>
        /// <param name="exit"> Показывает, нужно ли вернуться на стартовую страницу. </param>
        /// <returns> Возвращает номер кодировки из списка доступных. </returns>
        static int ChooseEncoding(ref bool exit)
        {
            Console.Clear();
            Console.WriteLine("Список доступных кодировок:");
            for (int i = 0; i < encodes.Length; i++)
                Console.WriteLine($"{i + 1}) {encodes[i]}");
            Console.WriteLine("Введите номер кодировки, кторую Вы хотите выбрать:");
            bool inputIsCorrect;
            int noOfEncoding;
            // Цикл будет повторяться, пока ввод не будет корректным, или пользователь не захочет вернуться.
            do
            {
                Console.Write("\b>>> ");
                var input = Console.ReadLine();
                inputIsCorrect = int.TryParse(input, out noOfEncoding) && (noOfEncoding >= 1) && (noOfEncoding <= encodes.Length + 1);
                if (!inputIsCorrect)
                {
                    Console.WriteLine("Выбранной опции не существует!\n" +
                        "Чтобы вернуться на стартовую страницу, нажмите ESC.\nЧтобы повторить ввод - любую другую клавишу.");
                    exit = inputIsCorrect = Console.ReadKey().Key == ConsoleKey.Escape;
                }
            } while (!inputIsCorrect);
            if (!exit)
            {
                Console.WriteLine("Кодировка успешно выбрана.\n" +
                    "Для продолжения нажмите любую клавишу.");
                Console.ReadKey();
            }
            return noOfEncoding - 1;
        }

        /// <summary>
        /// Метод позволяет выбрать одну из доступных директорий (в текущей).
        /// </summary>
        /// <param name="path"> Путь к текущей директории. </param>
        /// <param name="exit"> Показывает, нужно ли вернуться на стартовую страницу. </param>
        /// <param name="goBack"> Показывает, нужно ли подняться в директорию на уровень выше. </param>
        /// <returns> Возвращает true, если нужно остаться в текущей папке. </returns>
        static bool ChooseDirectory(ref string path, ref bool exit, out bool goBack)
        {
            // Получаем и выводим список доступных директорий.
            var folders = DisplayDirectories(path, "*", false);
            Console.WriteLine($"{folders.Length + 1}) Остаться в текущией папке.");
            Console.WriteLine($"{folders.Length + 2}) Подняться на уровень выше.");
            Console.WriteLine($"{folders.Length + 3}) Вернуться на стартовую страницу.");
            Console.WriteLine("Для перехода в директорию или другого действия введите соответствующее число.");
            bool inputIsCorrect;
            int noOfFolder;
            // Цикл будет повторяться, пока ввод не будет корректным, или пользователь не захочет вернуться.
            do
            {
                Console.Write("\b>>> ");
                var input = Console.ReadLine();
                inputIsCorrect = int.TryParse(input, out noOfFolder) && (noOfFolder >= 1) && (noOfFolder <= folders.Length + 3);
                goBack = noOfFolder == folders.Length + 2;
                exit = noOfFolder == folders.Length + 3;
                if (!inputIsCorrect)
                {
                    Console.WriteLine("Выбранной опции не существует!\n" +
                        "Чтобы вернуться на стартовую страницу, нажмите ESC.\nЧтобы повторить ввод - любую другую клавишу.");
                    exit = inputIsCorrect = exit || Console.ReadKey().Key == ConsoleKey.Escape;
                }
            } while (!inputIsCorrect);
            // Пользователь хочет подняться на уровень выше.
            if (goBack)
                return false;
            // Пользователь хочет остаться в текущей папке или вернуться.
            if (exit || noOfFolder == folders.Length + 1) 
                return true;
            path = folders[noOfFolder - 1];
            Console.WriteLine("Путь успешно установлен! Для продолжения нажмите любую клавишу.");
            Console.ReadKey();
            return false;
        }

        /// <summary>
        /// Метод получает путь к желаемой папке.
        /// </summary>
        /// <param name="path"> Путь к текущей директории. </param>
        /// <param name="exit"> Показывает, нужно ли вернуться на стартовую страницу. </param>
        static void GetToDirectory(ref string path, ref bool exit)
        {
            // Показывает, хочет ли пользователь остаться в текущей директории.
            bool stop,
                // Показывает, хочет ли пользователь подняться на уровень выше.
                goBack;
            do
            {
                stop = ChooseDirectory(ref path, ref exit, out goBack);
                if (goBack)
                    GetToRootDirectory(ref path, ref exit);
                if (exit)
                    return;

            } while (!stop);
            Console.Clear();
            Console.WriteLine($"Установлен следующий путь:\n{path}\n" +
                $"Для продолжения нажмите любую клавишу.");
            Console.ReadKey();
        }

        /// <summary>
        /// Метод позволяет подняться в директорию на уровень выше.
        /// </summary>
        /// <param name="path"> Путь к текущей директории. </param>
        /// <param name="exit"> Показывает, нужно ли вернуться на стартовую страницу. </param>
        static void GetToRootDirectory(ref string path, ref bool exit)
        {
            // Получаем индекс последнего разделителя.
            var posOfLastSeparator = path.LastIndexOf(Path.DirectorySeparatorChar);
            // Если он является последним символом в пути => это диск.
            if (posOfLastSeparator == path.Length - 1)
            {
                ChooseDrive(ref path, ref exit);
                return;
            }
            path = path[..posOfLastSeparator];
            if (path.IndexOf(Path.DirectorySeparatorChar) < 0)
                path = $"{path}{Path.DirectorySeparatorChar}";
        }

        /// <summary>
        /// Метод позволяет получить путь к желаемой папке шаг за шагом.
        /// </summary>
        /// <param name="exit"> Показывает, нужно ли вернуться на стартовую страницу. </param>
        /// <returns> Возвращает уть к желаемой папке. </returns>
        static string GetPathAutomaticly(ref bool exit)
        {            
            var path = new string("");
            ChooseDrive(ref path, ref exit);
            if (exit) 
                return null;
            GetToDirectory(ref path, ref exit);
            if (exit)
                return null;
            return path;
        }

        /// <summary>
        /// Метод позволяет получить путь одним из предложенных способов.
        /// </summary>
        /// <param name="exit"> Показывает, нужно ли вернуться на стартовую страницу. </param>
        /// <returns> Возвращает путь к желаемой папке. </returns>
        static string GetPath(ref bool exit)
        {
            Console.Clear();
            Console.WriteLine("1) Ввести путь вручную.\n2) Получить путь шаг за шагом.\n" +
                "3) Вернуться на стартовую страницу.\n" +
                "Для продолжения выберите один из предложенных вариантов.");
            bool inputIsCorrect;
            int option;
            // Цикл будет повторяться, пока ввод не будет корректным, или пользователь не захочет вернуться.
            do
            {
                Console.Write("\b>>> ");
                inputIsCorrect = int.TryParse(Console.ReadLine(), out option) && option >= 1 && option <= 3;
                if (!inputIsCorrect)
                {
                    Console.WriteLine("Выбранной опции не существует!\n" +
                        "Чтобы вернуться на стартовую страницу, нажмите ESC.\nЧтобы повторить ввод - любую другую клавишу.");
                    exit = inputIsCorrect = Console.ReadKey().Key == ConsoleKey.Escape;
                }
            } while (!inputIsCorrect);
            var path = "";
            // Выполняем действие в соответствии с выбранной опцией.
            switch (option)
            {
                case 1:
                    ChoosePathDirectly(ref path, ref exit);
                    break;
                case 2:
                    path = GetPathAutomaticly(ref exit);
                    break;
                case 3:
                    exit = true;
                    break;
            }
            if (exit)
                return null;
            return path;
        }

        /// <summary>
        /// Метод позволяет скопировать файл в выбранную директорию.
        /// </summary>
        /// <param name="path"> Путь к текущей директории. </param>
        /// <param name="exit"> Показывает, нужно ли вернуться на стартовую страницу. </param>
        static void CopyFile(string path, ref bool exit)
        {
            // Выбираем файл в текущей директории.
            var filePath = ChooseFile(path, ref exit);
            if (exit) return;
            Console.Clear();
            Console.WriteLine("Далее необходимо ввести путь директории,\n" +
                "в которую вы хотите скопировать файл.\n" +
                "(Для продолжения нажмите любую клавишу)");
            Console.ReadKey();
            // Выбираем директорию, в которую хотим скопировать файл.
            var newPath = GetPath(ref exit);
            if (exit) return;
            newPath = $"{newPath}{filePath[filePath.LastIndexOf(Path.DirectorySeparatorChar)..]}";
            File.Copy(filePath, newPath.ToString(), true);
            Console.Clear();
            Console.WriteLine("Файл успешно скопирован.");
        }

        /// <summary>
        /// Метод позволяет переместить файл в выбранную директорию.
        /// </summary>
        /// <param name="path"> Путь к текущей директории. </param>
        /// <param name="exit"> Показывает, нужно ли вернуться на стартовую страницу. </param>
        static void MoveFile(string path, ref bool exit)
        {
            // Выбираем файл в текущей директории.
            var filePath = ChooseFile(path, ref exit);
            if (exit) return;
            Console.Clear();
            Console.WriteLine("Далее необходимо ввести путь директории,\n" +
                "в которую вы хотите переместить файл.\n" +
                "(Для продолжения нажмите любую клавишу)");
            Console.ReadKey();
            // Выбираем директорию, в которую хотим переместить файл.
            var newPath = GetPath(ref exit);
            if (exit) return;
            newPath = $"{newPath}{filePath[filePath.LastIndexOf(Path.DirectorySeparatorChar)..]}";
            File.Move(filePath, newPath.ToString(), true);
            Console.Clear();
            Console.WriteLine("Файл был успешно перемещён.");
        }

        /// <summary>
        /// Метод позволяет удалить файл из текущей директории.
        /// </summary>
        /// <param name="path"> Путь к текущей директории. </param>
        /// <param name="exit"> Показывает, нужно ли вернуться на стартовую страницу. </param>
        static void DeleteFile(string path, ref bool exit)
        {
            // Выбираем файл, который хотим удалить.
            var filePath = ChooseFile(path, ref exit);
            if (exit) return;
            File.Delete(filePath);
            Console.Clear();
            Console.WriteLine("Файл был успешно удалён.");
            Console.ReadKey();
        }

        /// <summary>
        /// Метод позволяет ввести текст в консоль и сохранить его в виде файла в выбранной кодировке.
        /// </summary>
        /// <param name="path"> Путь к текущей директории. </param>
        /// <param name="exit"> Показывает, нужно ли вернуться на стартовую страницу. </param>
        static void InputFile(string path, ref bool exit)
        {
            Console.Clear();
            Console.WriteLine("Введите имя создаваемого файла: ");
            bool inputIsCorrect;
            // Список запрещённый в имени файла символов.
            var forbiddenChars = new char[]{ '/', '\\', '*', ':', '?', '|', '\"', '>', '<' };
            string fileName;
            // Цикл будет повторяться, пока ввод не будет корректным, или пользователь не захочет вернуться.
            do
            {
                Console.Write("\b>>> ");
                fileName = Console.ReadLine();
                inputIsCorrect = fileName.IndexOfAny(forbiddenChars) == -1;
                if (!inputIsCorrect)
                {
                    Console.WriteLine("Ошибка ввода!\nИмя файла не должно содержать следующих символов: /\\*:?|\"<>\n" +
                        "Для возвращения на стартовую страницу нажмите ESC, для повторного ввода - любую другую клавишу.");
                    exit = inputIsCorrect = Console.ReadKey().Key == ConsoleKey.Escape;
                }
            } while (!inputIsCorrect);
            if (exit) return;
            // Генерируем путь файла.
            var filePath = path.ToString() + Path.DirectorySeparatorChar + fileName;
            var noOfEncoding = ChooseEncoding(ref exit);
            if (exit) return;
            var fileWriter = new StreamWriter(filePath, false, Encoding.GetEncoding(encodes[noOfEncoding]));
            Console.Clear();
            Console.WriteLine("Введите содержимое файла. Для окончания ввода напишите </end>");
            // Построчно считываем строки с консоли и зхаписываем их в файл, пока пользователь не введёт необходимую команду.
            do
            {
                Console.Write("\b>>> ");
                var input = Console.ReadLine();
                if (input == "</end>")
                    break;
                fileWriter.WriteLine(input);
            } while (true);
            fileWriter.Close();
            Console.WriteLine("Файл успешно записан.");
            Console.ReadKey();
        }

        /// <summary>
        /// Метод позволяет прочитать содержимое нескольких файлов и вывести результат в консоль.
        /// </summary>
        /// <param name="path"> Путь к текущей директории. </param>
        /// <param name="exit"> Показывает, нужно ли вернуться на стартовую страницу. </param>
        static void ReadFiles(string path, ref bool exit)
        {
            // Список файлов, содержимое которых мы будем выводить.
            var files = new List<string> { ChooseFile(path, ref exit) };
            if (exit) return;
            bool stop;
            do
            {
                Console.Clear();
                Console.WriteLine("Выбранные файлы:");
                for (var i = 0; i < files.Count; i++)
                    Console.WriteLine($"{i + 1}) {files[i][(files[i].LastIndexOf(Path.DirectorySeparatorChar) + 1)..]}");
                stop = ReadFileChooseOption(ref exit);
                if (!stop)
                {
                    var nextFilePath = GetPath(ref exit);
                    if (exit) return;
                    files.Add(ChooseFile(path, ref exit));
                }
            } while (!stop);
            var noOfEncoding = ChooseEncoding(ref exit);
            if (exit) return;
            Console.Clear();
            Console.WriteLine("Содержимое выбранных файлов: ");
            foreach (var file in files)
            {
                var fileReader = new StreamReader(file, Encoding.GetEncoding(encodes[noOfEncoding]));
                while (!fileReader.EndOfStream)
                    Console.WriteLine(fileReader.ReadLine());
                fileReader.Close();
            }
        }

        /// <summary>
        /// Метод позволяет выбрать ещё один файл или вывести содержимое уже выбранных.
        /// </summary>
        /// <param name="exit"> Показывает, нужно ли вернуться на стартовую страницу. </param>
        /// <returns> Возвращает true, если пользователь хочет вывести содержимое выбранных файлов. </returns>
        static bool ReadFileChooseOption(ref bool exit)
        {
            bool inputIsCorrect;
            int option;
            Console.WriteLine("\nВыберите, следующее дествие:\n" +
                "1) Выбрать ещё один файл.\n2) Вывести содержимое выбранных файлов.");
            do
            {
                Console.Write("\b>>> ");
                inputIsCorrect = int.TryParse(Console.ReadLine(), out option) && option >= 1 && option <= 2;
                if (!inputIsCorrect)
                {
                    Console.WriteLine("Выбранной опции не существует.\n" +
                        "Для возвращения на стратовую страницу нажмите ESC, для повторного ввода - любую другую клавишу.");
                    exit = inputIsCorrect = Console.ReadKey().Key == ConsoleKey.Escape;
                }
            } while (!inputIsCorrect);
            if (exit) return false;
            return option == 2;
        }
    }
}
