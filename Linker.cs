using System;
using System.Collections.Generic;
using System.Linq;

namespace Eurodiffusion
{
    /// <summary>
    /// Предоставляет поверхность, по которой можно произвести связывание соседних городов между собой
    /// </summary>
    internal class Linker
    {
        /// <summary>
        /// Массив стран на поверхности
        /// </summary>
        private Country[] countries;
        /// <summary>
        /// Хранит информацию о том, какая страна присутствует в точке поверхности
        /// </summary>
        private Dictionary<(int X, int Y), City> plane;
        /// <summary>
        /// Массив сдвигов для получения координат соседних городов
        /// </summary>
        private (int X, int Y)[] offsets = { (-1, 0), (1, 0), (0, -1), (0, 1) };              

        /// <summary>
        /// Инициализирует новый экземпляр класса Linker
        /// </summary>
        /// <param name="countries">Массив стран на поверхности</param>
        public Linker(Country[] countries)
        {
            this.countries = countries;
            plane = new Dictionary<(int X, int Y), City>();

            Array.ForEach(countries, 
                country => country.PassCitiesAndDo(
                    (x, y) => plane.Add((x, y), country[x, y])));
        }
        /// <summary>
        /// Связывает города на поверхности
        /// </summary>
        public void LinkCities()
        {
            foreach (Country country in countries)
            {
                country.PassCitiesAndDo((x, y) =>
                {
                    for (int i = 0; i < offsets.Length; i++)
                    {
                        var neighbor = (x - offsets[i].X, y - offsets[i].Y);
                        plane.TryGetValue(neighbor, out country[x, y].Neighbors[i]);
                    }
                });
            }
        }

        /// <summary>
        /// Проверка достижимости всех городов
        /// </summary>
        /// <returns>Возможно ли достичь все города</returns>
        public bool IsAchievableAll()
        {
            int allCityNumber = countries.Sum(country => country.CountryRect.Width * country.CountryRect.Height);
            var rect = countries[0].CountryRect;
            int allAchievableCityNumber = City.BFS(countries[0][rect.X, rect.Y]);

            return allAchievableCityNumber == allCityNumber;
        }
    }
}
