using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace laba_4
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("введите количество элементов списка:");
            int n;
            while (!Int32.TryParse(Console.ReadLine(), out n) || n <= 0)
            {
                Console.WriteLine("Введите корректное положительное число:");
            }

            Console.WriteLine("Введите тип данных (int, double, string):");
            string dataType = Console.ReadLine()?.ToLower();

            if (dataType == "int")
            {
                ProcessList<int>(n, Convert.ToInt32);
            }
            else if (dataType == "double")
            {
                ProcessList<double>(n, Convert.ToDouble);
            }
            else if (dataType == "string")
            {
                ProcessList<string>(n, value => value);
            }
            else
            {
                Console.WriteLine("Неподдерживаемый тип данных.");
            }

            // Добавляем задачу с HashSet
            Console.WriteLine("\nЗадача 3 с перечнем стран, популярных у туристов:");
            TouristCountriesTask();
            Console.WriteLine("\nЗадача 4 с текстовым файлом:");
            Consent();
            Console.WriteLine("\nЗадача 5:");
            Student();
        }

        private static void ProcessList<T>(int n, Func<string, T> converter)
        {
            List<LIST<T>> L = new List<LIST<T>>();
            Console.WriteLine("введите элементы списка:");
            for (int i = 0; i < n; i++)
            {
                string input = Console.ReadLine();
                try
                {
                    L.Add(new LIST<T>(converter(input)));
                }
                catch
                {
                    Console.WriteLine("Некорректный ввод. Повторите:");
                    i--;
                }
            }

            Console.WriteLine("Ваш список:");
            foreach (var l in L)
            {
                Console.WriteLine(l.ToString());
            }
            Console.WriteLine("Введите элемент после которого вы хотите удалить следующий:");
            string elementInput = Console.ReadLine();
            T E;
            try
            {
                E = converter(elementInput);
            }
            catch
            {
                Console.WriteLine("Некорректный ввод.");
                return;
            }

            for (int i = 0; i < L.Count; i++)
            {
                if (EqualityComparer<T>.Default.Equals(L[i].l_n, E))
                {
                    if (i + 1 < L.Count && !EqualityComparer<T>.Default.Equals(L[i + 1].l_n, E))
                    {
                        L.RemoveAt(i + 1);
                    }
                }
            }

            Console.WriteLine("Список после удаления:");
            foreach (var l in L)
            {
                Console.WriteLine(l.ToString());
            }
            if (Circle(L))
            {
                Console.WriteLine("В списке есть хотя бы один элемент, равный следующему за ним по кругу.");
            }
            else
            {
                Console.WriteLine("В списке нет элементов, равных следующему за ним по кругу.");
            }
        }

        private static bool Circle<T>(List<LIST<T>> list)
        {
            // Преобразуем List<LIST<T>> в LinkedList<T>
            LinkedList<T> linkedList = new LinkedList<T>();
            foreach (var item in list)
            {
                linkedList.AddLast(item.l_n);
            }

            // Проверяем элементы по кругу
            var current = linkedList.First;
            do
            {
                var next = current.Next ?? linkedList.First; // Следующий узел или первый (для кольцевой проверки)
                if (EqualityComparer<T>.Default.Equals(current.Value, next.Value))
                {
                    return true;
                }
                current = current.Next;
            } 
            while (current != null && current != linkedList.First);

            return false;
        }

        // Класс для работы со списками
        public class LIST<T>
        {
            public T l_n { get; private set; }

            public LIST(T value)
            {
                l_n = value;
            }

            public override string ToString()
            {
                return l_n?.ToString();
            }
        }

        // Класс для работы со странами
        public class Country
        {
            public string Name { get; set; }
            public bool IsVisited { get; set; }

            public Country(string name)
            {
                Name = name;
                IsVisited = false;
            }
        }

        // Метод для задачи с HashSet
        private static void TouristCountriesTask()
        {
            // Ввод списка всех стран
            Console.WriteLine("Введите список всех стран (через запятую):");
            string[] countryNames = Console.ReadLine().Split(' ');
            List<Country> allCountries = new List<Country>();
            foreach (string name in countryNames)
            {
                allCountries.Add(new Country(name.Trim().ToLower()));
            }

            Console.Write("Введите количество туристов: ");
            int n;
            while (!Int32.TryParse(Console.ReadLine(), out n) || n <= 0)
            {
                Console.WriteLine("Введите корректное положительное число:");
            }

            // Создание списка стран для каждого туриста
            List<HashSet<string>> touristCountries = new List<HashSet<string>>();

            for (int i = 0; i < n; i++)
            {
                Console.WriteLine($"Введите страны, посещенные туристом {i + 1} (через пробел): ");
                string[] countries = Console.ReadLine().Split(' ');
                HashSet<string> touristSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                foreach (var country in countries)
                {
                    touristSet.Add(country.ToLower());
                }
                touristCountries.Add(touristSet);

                // Отмечаем посещенные страны
                foreach (var country in touristSet)
                {
                    var countryObj = allCountries.Find(c => c.Name == country);
                    if (countryObj != null)
                    {
                        countryObj.IsVisited = true;
                    }
                }
            }

            // Определяем страны, которые посетили все туристы
            HashSet<string> allTouristsVisited = new HashSet<string>(touristCountries[0], StringComparer.OrdinalIgnoreCase);
            foreach (var touristSet in touristCountries)
            {
                allTouristsVisited.IntersectWith(touristSet);
            }

            // Определяем страны, которые посетили некоторые туристы
            HashSet<string> someTouristsVisited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var touristSet in touristCountries)
            {
                someTouristsVisited.UnionWith(touristSet);
            }

            // Вывод результатов
            Console.WriteLine("\nСтраны, которые посетили все туристы:");
            foreach (var country in allTouristsVisited)
            {
                Console.WriteLine(country);
            }

            Console.WriteLine("\nСтраны, которые посетили некоторые туристы:");
            foreach (var country in someTouristsVisited)
            {
                Console.WriteLine(country);
            }

            Console.WriteLine("\nСтраны, которые не посетил ни один турист:");
            foreach (var country in allCountries)
            {
                if (!country.IsVisited)
                {
                    Console.WriteLine(country.Name);
                }
            }
        }
        private static void Consent()
        {
            string file = @"C:\Users\admin\Desktop\C#\laba_4_list\текст.txt";
            string text = File.ReadAllText(file);
            Console.WriteLine(text);

            // Разделяем текст на слова
            var words = text.Split(new char[] { ' ', ',', '.', '!', '?', ':', ';' }, StringSplitOptions.RemoveEmptyEntries);

            // Глухие согласные буквы
            HashSet<char> deaf = new HashSet<char> { 'п', 'к', 'т', 'с', 'ф', 'х', 'ц', 'ч' };
            Dictionary<char, int> consonantWordCounts = new Dictionary<char, int>();

            // Подсчитываем количество слов, в которых встречается каждая глухая согласная
            foreach (var word in words)
            {
                // Извлекаем уникальные глухие согласные в текущем слове
                var uniqueConsonantsInWord = word
                    .Where(c => deaf.Contains(char.ToLower(c)))
                    .Select(char.ToLower)
                    .Distinct();

                // Увеличиваем счётчик для каждой глухой согласной
                foreach (var consonant in uniqueConsonantsInWord)
                {
                    if (consonantWordCounts.ContainsKey(consonant))
                    {
                        consonantWordCounts[consonant]++;
                    }
                    else
                    {
                        consonantWordCounts[consonant] = 1;
                    }
                }
            }

            // Общее количество слов в тексте
            int totalWords = words.Length;

            // Отбираем согласные, которые отсутствуют ровно в одном слове
            var resultConsonants = consonantWordCounts
                .Where(kvp => totalWords - kvp.Value == 1) // Условие: отсутствует ровно в одном слове
                .Select(kvp => kvp.Key)
                .OrderBy(c => c); // Сортируем в алфавитном порядке

            // Выводим результат
            Console.WriteLine("\nГлухие согласные буквы, которые встречаются во всех словах, кроме одного:");
            foreach (var consonant in resultConsonants)
            {
                Console.WriteLine(consonant);
            }
        }
       

        private static void Student()
        {
            Console.WriteLine("\nВведите количество абитуриентов:");
            int n;
            while (!Int32.TryParse(Console.ReadLine(), out n) || n <= 0)
            {
                Console.WriteLine("Введите корректное положительное число:");
            }
            Dictionary<string, (string Name, int Score1, int Score2)> applicants = new Dictionary<string, (string, int, int)>();

            Console.WriteLine("Введите данные об абитуриентах в формате: фамилия имя баллы1 баллы2 ");
            for (int i = 0; i < n; i++)
            {
                string input = Console.ReadLine();
                string[] parts = input.Split(' ');

                // Извлечение данных
                string surname = parts[0];
                string name = parts[1];
                int score1 = int.Parse(parts[2]);
                int score2 = int.Parse(parts[3]);

                // Добавление абитуриента в словарь
                applicants[surname] = (name, score1, score2);
            }

            // Список абитуриентов неудачников
            SortedList<string, string> failure = new SortedList<string, string>();

            // Проверка на успешность
            foreach (var applicant in applicants)
            {
                if (applicant.Value.Score1 < 30 || applicant.Value.Score2 < 30)
                {
                    failure.Add(applicant.Key, applicant.Value.Name);
                }
            }

            // Вывод результата
            Console.WriteLine("Не допущенные к сдаче экзаменов:");
            foreach (var applicant in failure)
            {
                Console.WriteLine($"{applicant.Key} {applicant.Value}");
            }
        }
    }
}
