using System;
using System.Collections.Generic;

namespace Eurodiffusion
{
    /// <summary>
    /// Предоставляет описание сущности Города
    /// </summary>
    internal class City
    {
        /// <summary>
        /// Число монет с соответствующим мотивом
        /// </summary>
        private int[] coins;
        /// <summary>
        /// Порция монет соответствующего мотива
        /// </summary>
        private int[] portions;
        /// <summary>
        /// Счетчик полученных уникальных монет
        /// </summary>
        private int countComplete;

        /// <summary>
        /// Событие завершения города
        /// </summary>
        public event Action OnCompleted;
        /// <summary>
        /// Соседи данного города
        /// </summary>
        public City[] Neighbors { get; } = new City[4];
        /// <summary>
        /// Индексатор предоставляющий доступ к монетам города
        /// </summary>
        /// <param name="i">Индекс мотива монеты</param>
        /// <returns>Число монет с заданным мотивом, имеющееся в данном городе</returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public int this[int i]
        {
            get
            {
                if (i < 0 || i > coins.Length)
                    throw new IndexOutOfRangeException($"Монеты с мотивом под номером {i} не найдены");
                return coins[i];
            }
            set
            {
                if (i < 0 || i > coins.Length)
                    throw new IndexOutOfRangeException($"Монеты с мотивом под номером {i} не найдены");
                if (value < 0)
                    throw new ArgumentException("Кол-во монет не может быть отрицательным");

                // Если впервые появились монеты с i-ым мотивом, увеличиваем счетчик полученных монет и проверяем получены ли все монеты
                if (coins[i] == 0 && value > 0 && ++countComplete == coins.Length)
                    OnCompleted();                              // Если все монеты получены, город считается завершенным

                coins[i] = value;
            }
        }

        /// <summary>
        /// Инициализирует новый экземпляр класса City
        /// </summary>
        /// <param name="countriesNum">Число стран на поверхности</param>
        public City(int countriesNum)
        {
            portions = new int[countriesNum];
            coins = new int[countriesNum];
        }

        /// <summary>
        /// Подготавливает порции для каждого города на основе утреннего кол-ва монет
        /// </summary>
        public void PreparePortions()
        {
            for (int i = 0; i < portions.Length; i++)
            {
                portions[i] = coins[i] / 1000;
            }
        }
        /// <summary>
        /// Передает заготовленную утром порцию монет соседям
        /// </summary>
        public void TransferPortions()
        {
            for (int i = 0; i < portions.Length; i++)
            {
                foreach (var neighbor in Neighbors)
                {
                    if (neighbor != null)
                    {
                        coins[i] -= portions[i];
                        neighbor[i] += portions[i];
                    }
                }
            }
        }

        /// <summary>
        /// Проход по всем городам на основе БФС для выявления несвязных графов 
        /// </summary>
        /// <param name="city">Корневой узел</param>
        /// <returns>Чосло посещенных городов</returns>
        public static int BFS(City city)
        {
            List<City> visited = new List<City>();
            Queue<City> queue = new Queue<City>();
            queue.Enqueue(city);
            while (queue.Count > 0)
            {
                var first = queue.Dequeue();
                if (!visited.Contains(first))
                {
                    visited.Add(first);
                    foreach (var neighbor in first.Neighbors)
                    {
                        if (neighbor != null)
                        {
                            if (!visited.Contains(neighbor))
                                queue.Enqueue(neighbor);
                        }
                    }
                }
            }
            return visited.Count;
        }
    }
}
