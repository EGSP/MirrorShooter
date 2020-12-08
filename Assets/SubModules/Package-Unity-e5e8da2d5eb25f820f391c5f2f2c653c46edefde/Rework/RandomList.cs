using System;
using System.Collections.Generic;

// TODO: REWORK!
namespace Gasanov.SpeedUtils.RandomUtilities
{
    /// <summary>
    /// Класс случайной последовательности,
    /// который всегда возвращает уникальные элементы изначальной последовательности
    /// </summary>
    /// <typeparam name="T">Тип элементов последовательности</typeparam>
    public class RandomList<T>
    {
        /// <summary>
        /// Изначальная коллекция. Ссылка на изначальную коллекцию
        /// </summary>
        public readonly IList<T> SourceCollection;
        
        public RandomList(IList<T> sourceCollection)
        {
            SourceCollection = sourceCollection;
            TemporaryCollection = new List<T>(SourceCollection);
        }
        
        /// <summary>
        /// Временная коллекция используемая при работе
        /// </summary>
        public IList<T> TemporaryCollection { get; private set; }
        
        public bool IsEmpty => TemporaryCollection.Count == 0;

        /// <summary>
        /// Возвращает случайный элемент последовательности.
        /// Если коллекция пуста, то возвращает default(T)
        /// </summary>
        /// <returns></returns>
        public T Next()
        {
            // Если коллекция пуста
            if (IsEmpty)
            {
                return default(T);
            }

            var randomIndex = new Random().Next(0, TemporaryCollection.Count);
            var randomElement = TemporaryCollection[randomIndex];
            
            TemporaryCollection.RemoveAt(randomIndex);

            return randomElement;
        }
 
        /// <summary>
        /// Возвращает true, если новый объект не является default(T)
        /// </summary>
        public bool Next(out T nextObject)
        {
            // Если коллекция изначально пуста
            if (IsEmpty)
            {
                nextObject = default(T);
                return false;
            }
            else
            {
                nextObject = Next();
                return true;
            }
        }
    }
}