using System;
using System.Collections.Generic;
using System.Linq;

namespace Eurodiffusion
{
    /// <summary>
    /// Предоставляет диффузор для пошаговой диффузии монет по городам
    /// </summary>
    internal class Diffuser
    {
        /// <summary>
        /// Список записей предоставляющих информацию о завершённых странах и число дней, необходимых для их завершения
        /// </summary>
        private List<(Country Country, int Days)> Records;
        /// <summary>
        /// Массив стран, учавствующих в диффузии монет
        /// </summary>
        private Country[] countries;
        /// <summary>
        /// Число пройденных дней процесса диффузии
        /// </summary>
        private int days;

        /// <summary>
        /// Инициализирует новый экземпляр класса Diffuser
        /// </summary>
        /// <param name="countries">Массив стран, учавствующих в диффузии монет</param>
        public Diffuser(Country[] countries)
        {
            this.countries = countries;
            Array.ForEach(countries, country => country.OnCompleted += RecordDate);
            Records = new List<(Country Country, int Days)>(countries.Length);
        }
        /// <summary>
        /// Симулирует один день передачи монет между городами
        /// </summary>
        /// <returns>Булево значение, хранящее состояние завершености процесса диффузии (true - диффузия завершена; false - диффузия не завершена)</returns>
        public bool Step()
        {
            days++;
            Array.ForEach(countries, country => country.PassCitiesAndDo((x, y) => country[x, y].PreparePortions()));
            Array.ForEach(countries, country => country.PassCitiesAndDo((x, y) => country[x, y].TransferPortions()));
            return Records.Count == countries.Length;
        }
        /// <summary>
        /// Возвращает отсортированный по дням и названиям стран список записей
        /// </summary>
        /// <returns>Отсортированный по дням и названиям стран список записей</returns>
        public List<(Country Country, int Days)> GetRecords()
        {
            return Records.OrderBy(r => r.Country.Name).OrderBy(r => r.Days).ToList();
        }

        /// <summary>
        /// Обработчик события завершения страны
        /// </summary>
        /// <param name="country">Завершённая страна, вызвавшая событие OnComplete</param>
        private void RecordDate(Country country)
        {
            Records.Add((country, days));
        }
    }
}
