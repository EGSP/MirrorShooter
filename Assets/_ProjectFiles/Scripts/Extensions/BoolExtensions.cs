namespace Gasanov.Extensions.Primitives
{
    public static class BoolExtensions
    {
        /// <summary>
        /// Возвращает число типа int. true - 1, false - 0
        /// </summary>
        public static int ToInt(this bool b)
        {
            return b ? 1 : 0;
        }
    }
}