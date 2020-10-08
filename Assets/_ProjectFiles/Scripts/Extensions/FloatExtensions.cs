
namespace Gasanov.Extensions.Primitives
{
    public static class FloatExtensions
    {
        /// <summary>
        /// Самый эффективный способ округления числа и перевода его в строку
        /// </summary>
        /// <param name="f">Число</param>
        /// <param name="digits">Количество знаков после запятой</param>
        /// <returns></returns>
        public static string ToString(this float f, int digits)
        {
            return System.Math.Round(f, digits).ToString();
        }

        /// <summary>
        /// Возвращает число относительно промежутка (min--max) (0--1)
        /// </summary>
        public static float ToNormalized(this float f, float min, float max)
        {
            var normalized = (f - min) / (max - min);

            return normalized;
        }
    }
}