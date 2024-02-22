#define CONSOLE
#define CONNECTIVITY_CONDITION

using System;
using System.IO;
using System.Text;

namespace Eurodiffusion
{
    internal class Program
    {
        /// <summary>
        /// Начальное число монет в городе
        /// </summary>
        private const int INIT_NUM_COINS = 1000000;

        static void Main(string[] args)
        {
            TextReader reader = null;
            TextWriter writer = null;
            try
            {
                // Потоки ввода/вывода
#if CONSOLE     // Консольный ввод данных
                reader = Console.In;
                writer = Console.Out;
#elif FILE      // Чтение и запись данных через текстовые файлы
                reader = new StreamReader("input.txt");
                writer = new StreamWriter("output.txt");
#endif
                // Билдер для построения результирующей строки
                StringBuilder Output = new StringBuilder();
            
                // Считывание массива стран
                var countries = Read(reader);
                // Пока массив не null рассматриваем текущий случай
                for (int caseNumber = 1; countries != null; caseNumber++)
                {
                    // Создание объектов Linker и Diffuser на основе полученных стран
                    var validator = new Validator(countries);

                    // Проверяем корректность данных
                    if (!validator.Validate())
                        throw new InvalidDataException("Страны пересекаются в один и тех же городах");

                    var linker = new Linker(countries);
                    var diffuser = new Diffuser(countries);

                    // Инициализируем города начальным числом монет
                    for (int matifIndex = 0; matifIndex < countries.Length; matifIndex++)
                    {
                        countries[matifIndex].PassCitiesAndDo((x, y) => countries[matifIndex][x, y][matifIndex] = INIT_NUM_COINS);
                    }

                    // Связываем города
                    linker.LinkCities();
                    // По условию задачи гарантируется, что граф городов будет связным. Однако на случай отсутствия данной гаранитии написана следующая проверка 
#if CONNECTIVITY_CONDITION
                    // Проверка достижимости всех городов
                    if (!linker.IsAchievableAll())
                        throw new InvalidDataException("Граф городов не является связным. Полная диффузия недостижима");
#endif
                    // Производим диффузию до состояния заершённости всех стран
                    while (!diffuser.Step()) ;

                    // Получаем записи по проведенной диффузии
                    var records = diffuser.GetRecords();

                    // Оформляем вывод информации для текущего случая
                    Output.AppendLine($"Case Number {caseNumber}");
                    records.ForEach(s => Output.AppendLine($"{s.Country.Name} {s.Days}"));

                    // Считываем массив стран для следующего случая
                    countries = Read(reader);
                }
                // Строим результирующую строку и записываем ее в поток вывода
                writer.WriteLine(Output.ToString());
            }
            // В случае, когда граф не является связным или страны пересекаются в один и тех же городах
            catch (InvalidDataException ex)
            {
                Console.WriteLine(ex.Message);
            }
            // В случае, когда не был найден файл input.txt
            catch (FileNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
            }
            // Закрываем потоки
            finally
            {
                reader?.Close();
                writer?.Close();
            }
        }

        /// <summary>
        /// Считывание данных из потока
        /// </summary>
        /// <param name="reader">Поток, из которого производится чтение</param>
        /// <returns>Массив стран для конкретного случая, null - если при считывании произошла ошибка</returns>
        static Country[] Read(TextReader reader)
        {
            Country[] countries;
            try
            {
                // Считываем строку с кол-вом стран
                int countiesNumber = Convert.ToInt32(reader.ReadLine());
                // Если число стран равно 0 или больше 20 завершаем чтение
                if (countiesNumber == 0 || countiesNumber > 20) return null;
                countries = new Country[countiesNumber];

                // Считываем данные по странам
                for (int i = 0; i < countiesNumber; i++)
                {
                    string name = reader.ReadLine();
                    var data = name.Split(' ');
                    name = data[0];

                    int[] coord = new int[4];
                    for (int j = 0; j < coord.Length; j++)
                        coord[j] = Convert.ToInt32(data[j + 1]);

                    // Создаем экземпляр страны по полученным данным и записываем ссылку на него в массив
                    countries[i] = new Country(name, coord[0], coord[1], coord[2], coord[3], countiesNumber);
                }
            }
            // В случае, когда отсутствует название страны
            catch (ArgumentNullException ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
            // В случае, когда переданы некорректные данные
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
            // В случае, когда индекс вышел за границы массива
            catch (IndexOutOfRangeException ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
            // В случае, когда на ввод ожидалось значение другого формата
            catch (FormatException ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
            return countries;
        }
    }
}
