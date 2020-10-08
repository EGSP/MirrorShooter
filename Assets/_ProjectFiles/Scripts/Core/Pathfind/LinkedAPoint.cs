using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gasanov.Pathfind
{
    public class LinkedAPoint
    {
        /// <param name="x">Позиция точки по X</param>
        /// <param name="y">Позиция точки по Y</param>
        /// <param name="weight">Вес точки</param>
        public LinkedAPoint(int x, int y, int weight = 0)
        {
            X = x;
            Y = y;
            Weight = weight;

            IsWalkable = true;
        }

        /// <param name="x">Позиция точки по X</param>
        /// <param name="y">Позиция точки по Y</param>
        /// <param name="isWalkable">Можно ли передвигаться по этой точке</param>
        public LinkedAPoint(int x, int y, bool isWalkable)
        {
            X = x;
            Y = y;
            IsWalkable = isWalkable;
        }

        /// <summary>
        /// Позиция ячейки по X
        /// </summary>
        public int X { get; private set; }

        /// <summary>
        /// Позиция ячейки по Y
        /// </summary>
        public int Y { get; private set; }

        /// <summary>
        /// Вес точки
        /// </summary>
        public int Weight { get; set; }

        /// <summary>
        /// Можно ли передвигаться по этой точке
        /// </summary>
        public bool IsWalkable { get; private set; }

        /// <summary>
        /// Сосед слева
        /// </summary>
        public LinkedAPoint Left { get; set; }

        /// <summary>
        /// Сосед справа
        /// </summary>
        public LinkedAPoint Right { get; set; }

        /// <summary>
        /// Сосед сверху
        /// </summary>
        public LinkedAPoint Top { get; set; }

        /// <summary>
        /// Сосед снизу
        /// </summary>
        public LinkedAPoint Bottom { get; set; }

        /// <param name="weight">Вес точки</param>
        public void SetIsWalkableTrue(int weight)
        {
            Weight = weight;
            IsWalkable = true;
        }

        public void SetIsWalkableFalse()
        {
            IsWalkable = false;
        }

        public override string ToString()
        {
            return $"{X}:{Y} + {IsWalkable}";
        }

        public override bool Equals(object obj)
        {
            var objCasted = obj as LinkedAPoint;
            if (objCasted != null)
            {
                if (objCasted.X != X)
                    return false;

                if (objCasted.Y != Y)
                    return false;

                return true;
            }

            return false;
        }
    }
}