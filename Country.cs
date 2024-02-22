using System;

namespace Eurodiffusion
{
    /// <summary>
    /// Предоставляет описание сущности Страны
    /// </summary>
    class Country
    {
        /// <summary>
        /// Города, входящие в состав страны
        /// </summary>
        private City[,] cities;
        /// <summary>
        /// Счетчик завершенных городов
        /// </summary>
        private int countComplete;

        /// <summary>
        /// Событие завершения страны
        /// </summary>
        public event Action<Country> OnCompleted;
        /// <summary>
        /// Прямоугольник описывающий рамки страны
        /// </summary>
        public (int X, int Y, int Width, int Height) CountryRect { get; }
        /// <summary>
        /// Название страны
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Индексатор предоставляющий доступ к городам страны
        /// </summary>
        /// <param name="x">Положение города по оси X на поверхности</param>
        /// <param name="y">Положение города по оси Y на поверхности</param>
        /// <returns>Город по заданным координатам</returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public City this[int x, int y]
        {
            get
            {
                int xc = x - CountryRect.X;
                int yc = y - CountryRect.Y;
                if (xc < 0 || yc < 0 || xc >= CountryRect.Width || yc >= CountryRect.Height)
                {
                    throw new IndexOutOfRangeException($"Город с координатами ({x}:{y}) не найден");
                }
                return cities[xc, yc];
            }
        }
        /// <summary>
        /// Инициализирует новый экземпляр класса Country
        /// </summary>
        /// <param name="name">Название страны</param>
        /// <param name="xl">Левый край страны</param>
        /// <param name="yl">Нижний край страны</param>
        /// <param name="xh">Правый край страны</param>
        /// <param name="yh">Верхний край страны</param>
        /// <param name="countriesNum">Число стран на поверхности</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public Country(string name, int xl, int yl, int xh, int yh, int countriesNum)
        {
            if (name == null)
                throw new ArgumentNullException("Отсутствует название страны");
            if (name.Length == 0 || name.Length > 25)
                throw new ArgumentException("Передано некорректное название страны");
            if (xl < 0 || yl < 0 || xh < xl || yh < yl || xh > 10 || yh > 10)
                throw new ArgumentException("Переданы некорректные координаты городов");
            if (countriesNum < 0)
                throw new ArgumentException("Передано некорректное кол-во стран");

            Name = name;
            CountryRect = (xl, yl, xh - xl + 1, yh - yl + 1);
            cities = new City[CountryRect.Width, CountryRect.Height];

            for (int x = 0; x < CountryRect.Width; x++)
            {
                for (int y = 0; y < CountryRect.Height; y++)
                {
                    City city = new City(countriesNum);
                    city.OnCompleted += OnCityComplete;
                    cities[x, y] = city;
                }
            }
        }

        /// <summary>
        /// Проходится по всем городам страны и выполняет действие action(x, y)
        /// </summary>
        /// <param name="action">действие над городом</param>
        public void PassCitiesAndDo(Action<int, int> action)
        {
            // Проходимся по всей ширине страны
            for (int x = CountryRect.X; x < CountryRect.X + CountryRect.Width; x++)
            {
                // Проходимся по всей высоте страны
                for (int y = CountryRect.Y; y < CountryRect.Y + CountryRect.Height; y++)
                {
                    action(x, y);
                }
            }
        }
        /// <summary>
        /// Обработчик события завершения города
        /// </summary>
        private void OnCityComplete()
        {
            if (++countComplete == cities.Length)
                OnCompleted(this);
        }
    }
}
