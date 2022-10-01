using ConsoleApp9.Models;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace ConsoleApp9
{

    public class Program
    {
        private static ApplicationContext? db;
        public static void Main(string[] args)
        {
            Menu();

        }
        private static void Menu()
        {
            Console.WriteLine();
            Console.WriteLine("------------------------------------------------------------");
            Console.WriteLine("Menu:\n 1. Создание или подключение к таблице. \n 2. Создание записи. \n 3. Вывод всех строк с уникальным значением ФИО+дата \n 4. Заполнение автоматически n строк \n 5.  Результат выборки из таблицы по критерию");
            Console.WriteLine("Выберите нужный пункт");
            string? ans = Console.ReadLine();
            if (ans == "1")
            {
                CreateBD();
            }
            else if (ans == "2")
            {
                AddUser();
            }
            else if (ans == "3")
            {
                GetAllUniqUsers();
            }
            else if (ans == "4")
            {
                AutoFill();
            }
            else if (ans == "5")
            {
                SpecParam();
            }
            else
            {
                Console.WriteLine("try again");
            }
            Menu();
        }

        private static void CreateBD()
        {
            Console.WriteLine("Create(1) or Connect(2), Go Back to menu(3):");
            string? ans = Console.ReadLine();
            if (ans == "1")
            {
                db = new ApplicationContext(true);
            }
            else if (ans == "2")
            {
                db = new ApplicationContext(false);
            }
            else if (ans == "3")
            {
                return;
            }
            else
            {
                Console.WriteLine("try again");
                CreateBD();
            }
            Console.WriteLine("Press any key to return tp the menu...");
            Console.ReadKey();
        }
        private static void AddUser()
        {
            Console.WriteLine("Enter name:");
            string? name = Console.ReadLine();
            while (name is null)
            {
                Console.WriteLine("Try again");
                name = Console.ReadLine();
            }
            Console.WriteLine("Enter gender(F or M)");
            string? gender = Console.ReadLine();
            while ((gender is null) || ((gender[0] != 'F') && (gender[0] != 'M')))
            {
                Console.WriteLine("Try again, enter F or M");
                gender = Console.ReadLine();
            }
            Console.WriteLine("Enter Year(yyyy)");
            int year = int.Parse(Console.ReadLine());
            Console.WriteLine("Enter Month(mm)");
            int month = int.Parse(Console.ReadLine());
            Console.WriteLine("Enter Day(dd)");
            int day = int.Parse(Console.ReadLine());
            db.Users.Add(new User { Name = name, DateOfBirth = new DateTime(year, month, day), Gender = gender[0] });
            db.SaveChanges();
            Console.WriteLine("Объект успешно сохранен");
            Console.WriteLine("Press any key to return tp the menu...");
            Console.ReadKey();
        }

        private static void GetAllUniqUsers()
        {
            var users = db.Users.ToList();
            users.GroupBy(s => s.Name + s.DateOfBirth.ToShortDateString()).Select(g => g.First());
            users.Sort(new ComparerUser());
            Console.WriteLine("Список объектов:");
            foreach (User u in users)
            {
                Console.WriteLine($"{u.Name} - {u.DateOfBirth.ToShortDateString()}, Gender: {u.Gender}, Age: {(int)((DateTime.Now - u.DateOfBirth).TotalDays / 365)}");
            }
            Console.WriteLine("Press any key to return tp the menu...");
            Console.ReadKey();
        }
        public class ComparerUser : Comparer<User>
        {
            public override int Compare(User x, User y) => x.Name.CompareTo(y.Name);
        }
        public static void AutoFill()
        {
            Console.WriteLine("Enter num from 0 to 1000000(enter 0 if you want to return to the menu)");
            int num = 0;
            while (!int.TryParse(Console.ReadLine(), out num))
            {
                Console.WriteLine("Try again");
            }
            Random rand = new Random();
            string alph = "QWERTYUIOPASDFGHJKLZXCVBNM";
            for (int i = 0; i < num; i++)
            {
                char[] preName = new char[rand.Next() % 10 + 5];
                for (int j = 0; j < preName.Length; j++)
                {
                    preName[j] = alph[rand.Next() % alph.Length];
                }
                db.Users.Add(new User { Name = new string(preName), 
                    DateOfBirth = new DateTime(rand.Next() % 80 + 1940, rand.Next() % 12 + 1, rand.Next() % 28 + 1),
                    Gender = (rand.Next() % 2 == 0) ? 'F' : 'M' });
            }
            db.SaveChanges();
            Console.WriteLine("Объекты успешно сохранены");
            Console.WriteLine("Press any key to return tp the menu...");
            Console.ReadKey();
        }
        public static void SpecParam()
        {
            Console.WriteLine("Enter search gender(F or M): ");
            string? gender = Console.ReadLine();
            while ((gender is null) || ((gender[0] != 'F') && (gender[0] != 'M')))
            {
                Console.WriteLine("Try again, enter F or M");
                gender = Console.ReadLine();
            }
            Console.WriteLine("Name starts with: ");
            string? firstLetter = Console.ReadLine();
            while (firstLetter is null)
            {
                Console.WriteLine("Try again, enter letter");
                firstLetter = Console.ReadLine();
            }
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            var users = db.Users.ToList().Where(s => s.Gender == gender[0] && s.Name[0] == firstLetter[0]);
            stopWatch.Stop();
            Console.WriteLine("Выборка:");
            foreach (User u in users)
            {
                Console.WriteLine($"{u.Id}. {u.Name} - {u.DateOfBirth.ToShortDateString()}, Gender: {u.Gender}");
            }
            Console.WriteLine($"Запрос выполнился за {stopWatch.Elapsed.Milliseconds}ms");
            Console.WriteLine("Press any key to return tp the menu...");
            Console.ReadKey();
        }

    }
}