using System;
using System.Diagnostics;
using System.Linq;

namespace ProcessManager
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Clear(); //Очистка консоли
                ShowProcesses(); // Отображение запущенных процессов
                Console.WriteLine("\nВыберите действие:");
                Console.WriteLine("1. Найти процессы по имени");
                Console.WriteLine("2. Убить процесс по ID");
                Console.WriteLine("3. Убить процесс по имени");
                Console.WriteLine("0. Выход");
                var choice = Console.ReadLine(); // Чтение выбора пользователя

                // Выполнение действия в зависимости от выбора пользователя
                switch (choice)
                {
                    case "1":
                        FindProcessesByName(); // Поиск процессов по имени
                        break;
                    case "2":
                        KillProcessById(); // Завершение процесса по ID
                        break;
                    case "3":
                        KillProcessByName(); // Завершение процесса по имени
                        break;
                    case "0":
                        return; // Выход из программы
                    default:
                        Console.WriteLine("Некорректный выбор. Попробуйте снова."); // Обработка неверного выбора

                        break;
                }
            }
        }

        // Метод для отображения запущенных процессов
        static void ShowProcesses()
        {
            var currentProcess = Process.GetCurrentProcess(); // Получение текущего процесса
            var processes = Process.GetProcesses(); // Получение всех процессов

            Console.WriteLine("Список запущенных процессов:");
            foreach (var process in processes)
            {
                // Отображение текущего процесса с меткой
                if (process.Id == currentProcess.Id)
                {
                    Console.WriteLine($"-> {process.ProcessName} (ID: {process.Id}) [Текущий процесс]");
                }
                else
                {
                    Console.WriteLine($"{process.ProcessName} (ID: {process.Id})");
                }
            }
        }

        // Метод для поиска процессов по имени
        static void FindProcessesByName()
        {
            Console.Write("Введите имя процесса (часть имени разрешена): ");
            var name = Console.ReadLine(); // Чтение имени процесса от пользователя

            // Проверка, пустое ли имя процесса
            if (string.IsNullOrEmpty(name))
            {
                Console.WriteLine("Имя процесса не может быть пустым.");
                return;
            }

            // Получение процессов по имени
            var processes = Process.GetProcessesByName(name);
            var allProcesses = Process.GetProcesses()
                .Where(p => p.ProcessName.Contains(name, StringComparison.OrdinalIgnoreCase));

            if (!allProcesses.Any())
            {
                Console.WriteLine("Процессы не найдены.");
            }
            else
            {
                Console.WriteLine("Найденные процессы:");
                // Отображение найденных процессов
                foreach (var process in allProcesses)
                {
                    Console.WriteLine($"{process.ProcessName} (ID: {process.Id})");
                }
            }
            Console.ReadKey(); // Ожидание нажатия клавиши
        }

        // Метод для убийства процесса по ID
        static void KillProcessById()
        {
            Console.Write("Введите ID процесса для завершения: ");
            // Попытка преобразовать ввод в целое число
            if (int.TryParse(Console.ReadLine(), out var id))
            {
                try
                {
                    var process = Process.GetProcessById(id); // Получение процесса по ID
                    process.Kill(); // Завершение процесса
                    Console.WriteLine($"Процесс {process.ProcessName} (ID: {process.Id}) завершен.");  // Подтверждение завершения
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("Процесс с таким ID не найден."); // Обработка случая, если процесс не найден
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}"); // Общая обработка ошибок
                }
            }
            else
            {
                Console.WriteLine("Некорректный ID."); // Обработка случая, если введён некорректный ID
            }
            Console.ReadKey(); // Ожидание нажатия клавиши
        }

        // Метод для убийства процесса по его имени
        static void KillProcessByName()
        {
            Console.Write("Введите имя процесса для завершения: ");
            var name = Console.ReadLine(); // Чтение имени процесса от пользователя
            var processes = Process.GetProcessesByName(name); // Получение процесса по имени

            if (processes.Length == 0)
            {
                Console.WriteLine("Процессы не найдены."); // Обработка случая, если процессы не найдены
            }
            else
            {
                foreach (var process in processes)
                {
                    try
                    {
                        process.Kill(); // Завершение процесса
                        Console.WriteLine($"Процесс {process.ProcessName} (ID: {process.Id}) завершен.");  // Подтверждение завершения
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Не удалось завершить процесс {process.ProcessName} (ID: {process.Id}): {ex.Message}");
                    }
                }
            }
            Console.ReadKey(); // Ожидание нажатия клавиши
        }
    }
}