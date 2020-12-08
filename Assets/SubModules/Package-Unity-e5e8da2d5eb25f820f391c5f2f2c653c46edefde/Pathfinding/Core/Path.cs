using System;
using System.Collections;
using System.Collections.Generic;

namespace Egsp.Core.Pathfinding
{
    public class Path<TPoint> : IPath<TPoint>
    {
        /// <summary>
        /// Проверяет необходимость смены текущей точки.
        /// </summary>
        protected readonly Func<TPoint, TPoint, bool> ContinueFunc;
        protected readonly IEnumerator<TPoint> PointsEnumerator;

        public TPoint Current { get; protected set; }
        public IEnumerable<TPoint> Points { get; protected set; }
        
        /// <summary>
        /// Продолжено ли движение по пути.
        /// </summary>
        public bool Continued { get; protected set; }

        public Path(IEnumerable<TPoint> points, Func<TPoint, TPoint, bool> continueFunc)
        {
            if(points == null)
                throw new EmptyPathException();
            
            ContinueFunc = continueFunc;
            PointsEnumerator = points.GetEnumerator();
            
            Points = points;
            
            // Изначально указатель никуда не указывает и его нужно сдвинуть на первый элемент.
            MovePointer();
        }

        /// <summary>
        /// Проверка на возможность продолжения перемещения.
        /// </summary>
        public bool Continue(TPoint origin)
        {
            // Если нужно двигаться к следующей точке.
            if (ContinueFunc(origin, Current))
            {
                MovePointer();
            }

            // Продолжен ли путь.
            return Continued;
        }

        /// <summary>
        /// Передвигаем указатель на следующий элемент. 
        /// </summary>
        protected void MovePointer()
        {
            // Если есть элементы в коллекции точек.
            if (PointsEnumerator.MoveNext())
            {
                Current = PointsEnumerator.Current;

                Continued = true;
            }
            else
            {
                Continued = false;
            }
        }
    }
    
    public class EmptyPath<TPoint> : IPath<TPoint>
    {
        public TPoint Current => default(TPoint);
        public IEnumerable<TPoint> Points => null;
        
        public bool Continue(TPoint origin)
        {
            return false;
        }
    }

    public class EmptyPathException : Exception
    {
        public EmptyPathException()
            : base("Path is empty! Try using EmptyPath<TPoint> instead of Path<TPoint>")
        {
        }
    }
}