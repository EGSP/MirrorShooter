using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Gasanov.Extensions.Linq
{
    public static class LinqExtensions
    {
        /// <summary>
        /// Возвращает первый объект заданного типа. Может вернуть null.
        /// </summary>
        public static T FindType<T>(this IEnumerable source) where T : class
        {
            T coincidence = null;

            foreach (var obj in source)
            {
                coincidence = obj as T;

                // Если есть совпадение
                if (coincidence != null)
                {
                    return coincidence;
                }
            }

            return coincidence;
        }

        /// <summary>
        /// Возвращает все типы заданного типа. Может вернуть пустой список.
        /// </summary>
        public static List<T> FindTypes<T>(this IEnumerable source) where T : class
        {    
            var coincidences = new List<T>();

            foreach (var obj in source)
            {
                var coincidence = source as T;

                // Если тип совпадает
                if (coincidence != null)
                {
                    coincidences.Add(coincidence);
                }
            }

            return coincidences;
        }
        
        /// <summary>
        /// Возвращает индекс наибольшего элемента.
        /// Если в коллекции нет элементов, то будет возвращено -1
        /// </summary>
        public static int MaxIndex<T>(this IEnumerable<T> sequence, Func<T,IComparable> selector)
        {
            var maxIndex = -1;
            var maxValue = default(T);

            var index = 0;
            foreach (var value in sequence)
            {
                if (selector(value).CompareTo(maxValue) > 0 || maxIndex == -1)
                {
                    maxIndex = index;
                    maxValue = value;
                }
                index++;
            }
            return maxIndex;
        }
        
        /// <summary>
        /// Возвращает индекс наименьшего элемента.
        /// Если в коллекции нет элементов, то будет возвращено -1
        /// </summary>
        public static int MinIndex<T>(this IEnumerable<T> sequence, Func<T,IComparable> selector)
        {
            // Изменить Func - сделать сравнение в Func
            
            var minIndex = -1;
            var minValue = default(T);

            var index = 0;
            foreach (var value in sequence)
            {
                if (minValue == null)
                {
                    minValue = value;
                    minIndex = index;
                }
                else if (selector(value).CompareTo(selector(minValue)) < 0 || minIndex == -1)
                {
                    minIndex = index;
                    minValue = value;
                }
                index++;
            }
            return minIndex;
        }

        /// <summary>
        /// Возвращает случайный элемент
        /// </summary>
        public static T Random<T>(this IEnumerable<T> collection)
        {
            var randomIndex = new System.Random().Next(0,collection.Count());

            return collection.ElementAt(randomIndex);
        }

        /// <summary>
        /// Проходит по всем элементам и выполняет действие.
        /// </summary>
        public static IEnumerable ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (var item in collection)
            {
                action(item);
            }

            return collection;
        }
    }
}