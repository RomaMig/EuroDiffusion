using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eurodiffusion
{
    /// <summary>
    /// Предоставляет валидатор, используемый для проверки данных массива стран
    /// </summary>
    internal class Validator
    {
        private Country[] countries;

        public Validator(Country[] countries)
        {
            this.countries = countries;
        }

        public bool Validate()
        {
            bool isValid = true;
            for (int i = 0; i < countries.Length - 1; i++)
            {
                for (int j = i + 1; j < countries.Length; j++)
                {
                    isValid &= !isIntersect(countries[i].CountryRect, countries[j].CountryRect);
                }
            }
            return isValid;
        }

        private bool isIntersect((int X, int Y, int Width, int Height) rect0, (int X, int Y, int Width, int Height) rect1) 
        {
            var overlapByX = IsValueBetween(rect0.X, rect1.X, rect1.X + rect1.Width) || IsValueBetween(rect1.X, rect0.X, rect0.X + rect0.Width);
            var overlapByY = IsValueBetween(rect0.Y, rect1.Y, rect1.Y + rect1.Height) || IsValueBetween(rect1.Y, rect0.Y, rect0.Y + rect0.Height);
            return overlapByX && overlapByY;
        }

        private bool IsValueBetween(int value, int min, int max) 
        {
            return value >= min && value <= max;
        }
    }
}
