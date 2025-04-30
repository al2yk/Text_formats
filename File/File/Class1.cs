using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Newtonsoft.Json;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;


namespace FilesDDL
{

    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Teacher { get; set; }
        public int CountStudent { get; set; }

        public override string ToString()
        {
            return $"Номер: {Id}, Название Дисциплины: {Name}, Преподаватель: {Teacher}, Количество студентов: {CountStudent}";
        }
    }

    public static class Files<T> where T : class
    {
        public static List<T> ReadFromFile(string file)
        {
            if (!File.Exists(file))
            {
                Console.WriteLine("Файл не найден");
                return null;
            }

            string extension = Path.GetExtension(file).ToLower();
            try
            {
                switch (extension)
                {
                    case ".csv":
                        return ReadCSV(file);
                    case ".json":
                        return ReadJSON(file);
                    case ".xml":
                        return ReadXML(file);
                    case ".yaml":
                        return ReadYAML(file);
                    default:
                        Console.WriteLine("Не верный формат");
                        return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при чтении файла: {ex.Message}");
                return null;
            }
        }
        public static void WriteFile(List<T> items, string file)
        {
            string extension = Path.GetExtension(file).ToLower();
            try
            {
                switch (extension)
                {
                    case ".csv":
                        WriteCSV(items, file);
                        break;
                    case ".json":
                        WriteJSON(items, file);
                        break;
                    case ".xml":
                        WriteXML(items, file);
                        break;
                    case ".yaml":
                        WriteYAML(items, file);
                        break;
                    default:
                        throw new NotSupportedException("Неподдерживаемый формат файла.");
                }
                Console.WriteLine($"Успешно записано в файл {extension.ToUpper()}: {file}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при записи в файл {extension.ToUpper()}: {ex.Message}");
            }
        }

        private static List<T> ReadCSV(string file)
        {
            List<T> items = new List<T>();
            try
            {
                using (var reader = new StreamReader(file))
                {
                    reader.ReadLine(); // Пропускаем заголовок

                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        string[] values = line.Split(',');

                        var item = Activator.CreateInstance<T>();
                        var pro = typeof(T).GetProperties();

                        for (int i = 0; i < pro.Length && i < values.Length; i++)
                        {
                            var pro1 = pro[i];
                            var value = values[i];

                            if (pro1.PropertyType == typeof(int))
                            {
                                pro1.SetValue(item, int.Parse(value));
                            }
                            else
                            {
                                pro1.SetValue(item, value);
                            }
                        }
                        items.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при чтении CSV файла: {ex.Message}");
            }
            return items;
        }

        private static List<T> ReadJSON(string file)
        {
            try
            {
                string jsonString = File.ReadAllText(file);
                return JsonConvert.DeserializeObject<List<T>>(jsonString);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при чтении JSON файла: {ex.Message}");
                return null;
            }
        }

        [XmlRoot("Courses")]
        public class CourseList
        {
            [XmlElement("Course")]
            public List<T> Courses { get; set; }
        }

        private static List<T> ReadXML(string file)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(CourseList));
                using (FileStream fileStream = new FileStream(file, FileMode.Open))
                {
                    CourseList courseList = (CourseList)serializer.Deserialize(fileStream);
                    return courseList.Courses;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при чтении XML файла: {ex.Message}");
                return null;
            }
        }

        private static List<T> ReadYAML(string file)
        {
            try
            {
                var deserializer = new DeserializerBuilder()
                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                    .Build();

                using (var reader = new StreamReader(file))
                {
                    return deserializer.Deserialize<List<T>>(reader);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при чтении YAML файла: {ex.Message}");
                return null;
            }
        }

        private static void WriteCSV(List<T> items, string file)
        {
            try
            {
                using (var writer = new StreamWriter(file))
                {
                    if (items == null || items.Count == 0)
                    {
                        Console.WriteLine("Нет данных для записи в CSV файл.");
                        return;
                    }

                    var properties = typeof(T).GetProperties();
                    var header = string.Join(",", properties.Select(p => p.Name));
                    writer.WriteLine(header);

                    foreach (var item in items)
                    {
                        var values = string.Join(",", properties.Select(p => p.GetValue(item)));
                        writer.WriteLine(values);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при записи CSV файла: {ex.Message}");
            }
        }

        private static void WriteJSON(List<T> items, string file)
        {
            try
            {
                string jsonString = JsonConvert.SerializeObject(items, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(file, jsonString);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при записи JSON файла: {ex.Message}");
            }
        }

        private static void WriteXML(List<T> items, string file)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<T>));
                using (FileStream fileStream = new FileStream(file, FileMode.Create))
                {
                    serializer.Serialize(fileStream, items);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при записи XML файла: {ex.Message}");
            }
        }

        private static void WriteYAML(List<T> items, string file)
        {
            try
            {
                var serializer = new SerializerBuilder()
                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                    .Build();

                string yamlString = serializer.Serialize(items);
                File.WriteAllText(file, yamlString);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при записи YAML файла: {ex.Message}");
            }
        }

    }

    public class Program
    {
        private static List<Course> courses = new List<Course>();

        public static void Main(string[] args)
        {
            bool running = true;
            while (running)
            {
                Console.WriteLine("Что будем делать?");
                Console.WriteLine("1. Чтение из файла");
                Console.WriteLine("2. Запись в файл");
                Console.WriteLine("3. Отображение данных");
                Console.WriteLine("4. Сортировка данных");
                Console.WriteLine("5. Поиск данных");
                Console.WriteLine("6. Добавление данных");
                Console.WriteLine("7. Удаление данных");
                Console.WriteLine("8. Изменение данных");
                Console.WriteLine("9. Выход");

                Console.Write("Введите ваш выбор: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ReadDataFromFile();
                        break;
                    case "2":
                        WriteFile();
                        break;
                    case "3":
                        View();
                        break;
                    case "4":
                        Sort();
                        break;
                    case "5":
                        Search();
                        break;
                    case "6":
                        Add();
                        break;
                    case "7":
                        Delete();
                        break;
                    case "8":
                        Modify();
                        break;
                    case "9":
                        running = false;
                        break;
                    default:
                        Console.WriteLine("Такого варианта не существует");
                        break;
                }
            }
            Console.WriteLine("Выход");
        }

        private static void ReadDataFromFile()
        {
            Console.Write("Введите путь к файлу для чтения: ");
            string filePath = Console.ReadLine();

            List<Course> coursesFromFile = Files<Course>.ReadFromFile(filePath);
            if (coursesFromFile != null)
            {
                courses.AddRange(coursesFromFile); // Добавляем прочитанные курсы к общему списку
                Console.WriteLine("Данные успешно прочитаны!");
            }
            else
            {
                Console.WriteLine("Не удалось прочитать данные из файла");
            }
        }



        private static void WriteFile()
        {
            Console.Write("Введите путь к файлу для записи: ");
            string filePath = Console.ReadLine();

            if (courses.Count == 0)
            {
                Console.WriteLine("Нет данных для записи. Сначала прочитайте данные.");
                return;
            }

            Files<Course>.WriteFile(courses, filePath); // Используем WriteItemToFile
            Console.WriteLine("Данные успешно записаны!");
        }


        private static void View()
        {
            if (courses.Count == 0)
            {
                Console.WriteLine("Нет данных для отображения. Сначала прочитайте данные.");
                return;
            }

            foreach (var course in courses)
            {
                Console.WriteLine(course);
            }
        }

        private static void Sort()
        {
            if (courses.Count == 0)
            {
                Console.WriteLine("Нет данных для сортировки. Сначала прочитайте данные.");
                return;
            }

            Console.WriteLine("\nСортировать по? (id, Название Дисциплины, Преподаватель, Количество студентов)");
            string sortBy = Console.ReadLine();

            switch (sortBy.ToLower())
            {
                case "id":
                    courses = courses.OrderBy(c => c.Id).ToList();
                    break;
                case "название дисциплины":
                    courses = courses.OrderBy(c => c.Name).ToList();
                    break;
                case "преподаватель":
                    courses = courses.OrderBy(c => c.Teacher).ToList();
                    break;
                case "количество студентов":
                    courses = courses.OrderBy(c => c.CountStudent).ToList();
                    break;
                default:
                    Console.WriteLine("Неверный параметр");
                    return;
            }

            Console.WriteLine("Данные успешно отсортированы!\n");
            View();
        }

        private static void Search()
        {
            Console.Write("Введите текст для поиска: ");
            string searchString = Console.ReadLine();

            var results = courses.Where(c =>
                c.Name.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0 ||
                c.Teacher.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0
            ).ToList();

            if (results.Count == 0)
            {
                Console.WriteLine("Данные не найдены");
            }
            else
            {
                Console.WriteLine("Результаты поиска:");
                foreach (var course in results)
                {
                    Console.WriteLine(course);
                }
            }
        }

        private static void Add()
        {
            Console.Write("Введите ID курса: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Неверный формат ID");
                return;
            }

            Console.Write("Введите название курса: ");
            string name = Console.ReadLine();

            Console.Write("Введите имя преподавателя: ");
            string teacher = Console.ReadLine();

            Console.Write("Введите количество студентов: ");
            if (!int.TryParse(Console.ReadLine(), out int countstudent))
            {
                Console.WriteLine("Неверный формат количества студентов");
                return;
            }

            Course newCourse = new Course { Id = id, Name = name, Teacher = teacher, CountStudent = countstudent };
            courses.Add(newCourse);

            Console.WriteLine("Курс успешно добавлен!");
        }

        private static void Delete()
        {
            Console.Write("Введите ID курса для удаления: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Неверный формат ID.");
                return;
            }

            var course = courses.FirstOrDefault(c => c.Id == id);
            if (course != null)
            {
                courses.Remove(course);
                Console.WriteLine("Курс успешно удален!");
            }
            else
            {
                Console.WriteLine("Курс не найден.");
            }
        }

        private static void Modify()
        {
            Console.Write("Введите ID курса для изменения: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Неверный формат ID.");
                return;
            }

            var course = courses.FirstOrDefault(c => c.Id == id);
            if (course == null)
            {
                Console.WriteLine("Курс не найден.");
                return;
            }

            Console.Write("Введите новое название курса (или нажмите Enter, чтобы оставить текущее): ");
            string newName = Console.ReadLine();
            if (!string.IsNullOrEmpty(newName))
            {
                course.Name = newName;
            }

            Console.Write("Введите новое имя преподавателя (или нажмите Enter, чтобы оставить текущее): ");
            string teacher = Console.ReadLine();
            if (!string.IsNullOrEmpty(teacher))
            {
                course.Teacher = teacher;
            }

            Console.Write("Введите новое количество студентов (или нажмите Enter, чтобы оставить текущее): ");
            string countStudent = Console.ReadLine();
            if (!string.IsNullOrEmpty(countStudent) && int.TryParse(countStudent, out int countStudent2))
            {
                course.CountStudent = countStudent2;
            }

            Console.WriteLine("Курс успешно изменен!");
        }
    }
}
