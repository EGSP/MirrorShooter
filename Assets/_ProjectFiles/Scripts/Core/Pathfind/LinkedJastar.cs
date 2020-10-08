using System.Collections.Generic;
using System;
using System.Linq;

using Gasanov.Extensions;
using Gasanov.Extensions.Linq;

namespace Gasanov.Pathfind
{
    public class LinkedJastar
    {
        public LinkedJastar(int width, int height)
        {
            Width = width;
            Height = height;
            
            Grid = new LinkedAPoint[Width][];

            for (var x = 0; x < Width; x++)
            {
                Grid[x] = new LinkedAPoint[Height];

                for (var y = 0; y < Height; y++)
                {
                    var point = new LinkedAPoint(x,y);

                    if (x > 0)
                    {
                        var previousPoint = Grid[x - 1][y];
                        point.Left = previousPoint;
                        previousPoint.Right = point;
                    }

                    if (y > 0)
                    {
                        var previousPoint = Grid[x][y - 1];
                        point.Bottom = previousPoint;
                        previousPoint.Top = point;
                    }

                    Grid[x][y] = point;
                }
            }
        }
        
        /// <summary>
        /// Сетка ячеек
        /// </summary>
        public LinkedAPoint[][] Grid { get; private set; }
        
        /// <summary>
        /// Ширина сетки
        /// </summary>
        public int Width { get; private set; }
        
        /// <summary>
        /// Высота сетки
        /// </summary>
        public int Height { get; private set; }



        /// <summary>
        /// Поиск оптимального пути
        /// </summary>
        /// <param name="start">Стартовая точка</param>
        /// <param name="goal">Конечная точка</param>
        /// <exception cref="Exception">Появляется при больших рассчетах</exception>
        public List<LinkedAPoint> FindPath(LinkedAPoint start, LinkedAPoint goal)
        {
            var closedSet = new List<LinkedPathNode>();
            var openSet = new List<LinkedPathNode>();

            var startNode = new LinkedPathNode()
            {
                Point = start,
                ComeFrom = null,
                PathLengthFromStart = 0,
                HeuristicPathLength = GetHeuristicPathLength(start, goal)
            };
            openSet.Add(startNode);

            int exception = 0;
            while (openSet.Count > 0)
            {
                exception++;
                if (exception > 10000)
                    throw new System.Exception("Поиск пути был обработан более 10_000 тысяч раз");

                // Потенциально ближайший узел
                var nearestNodeIndex = openSet.MinIndex(x 
                    => x.FullPathLength);
                LinkedPathNode nearestNode = openSet[nearestNodeIndex];

                // Если ближайший узел является нашей целью
                if (goal.Equals(nearestNode.Point))
                {
                    var path = GetPathForNode(nearestNode);
                    path.Reverse();
                    return path;
                }
                
                // Убираем узел из открытого списка и помещаем в закрытый
                openSet.Remove(nearestNode);
                closedSet.Add(nearestNode);

                // Получаем всех соседей
                var neighbours = GetNodeNeighbours(nearestNode, goal);

                for (var i = 0; i < neighbours.Count; i++)
                {
                    var neighbourNode = neighbours[i];

                    // Если данный узел был рассмотрен ранее
                    if (closedSet.FirstOrDefault(x
                        => x.Point == neighbourNode.Point) != null)
                        continue;

                    // Находится ли сосед в открытом списке
                    var coincidenceNode = openSet.FirstOrDefault(x
                        => x.Point == neighbourNode.Point);

                    // Если сосед не был на рассмотрении (не существует в открытом списке)
                    if (coincidenceNode == null)
                    {
                        openSet.Add(neighbourNode);
                    }
                    else
                    {
                        // Если совпадение не было на рассмотрении, но предыдуший путь был больше текущего.
                        // Точка у coincidence и neighbour одна, но путь и рассчеты могут быть разными
                        if (coincidenceNode.PathLengthFromStart > neighbourNode.PathLengthFromStart)
                        {
                            coincidenceNode.ComeFrom = neighbourNode.ComeFrom;
                            coincidenceNode.PathLengthFromStart = neighbourNode.PathLengthFromStart;
                        }
                    }
                }
            }

            // Пути не существует
            return new List<LinkedAPoint>();
        }
        
        /// <summary>
        /// Получаем все точки от конца до начала
        /// </summary>
        /// <param name="node">Узел от которого получаем весь путь</param>
        /// <returns></returns>
        public List<LinkedAPoint> GetPathForNode(LinkedPathNode node)
        {
            var path = new List<LinkedAPoint>();

            while (node != null)
            {
                path.Add(node.Point);
                
                // Получаем предшествующий узел, который может быть null
                node = node.ComeFrom;
            }

            return path;
        }

        /// <summary>
        /// Получает всех соседей узла
        /// </summary>
        /// <param name="node">Текущий узел</param>
        /// <param name="goal">Конечная точка</param>
        public List<LinkedPathNode> GetNodeNeighbours(LinkedPathNode node, LinkedAPoint goal)
        {
            var result = new List<LinkedPathNode>();

            var neighbours = new LinkedAPoint[4];
            neighbours[0] = node.Point.Left;
            neighbours[1] = node.Point.Right;
            neighbours[2] = node.Point.Top;
            neighbours[3] = node.Point.Bottom;

            for (var i = 0; i < neighbours.Length; i++)
            {
                var point = neighbours[i];

                if (point == null)
                    continue;
            
                if (point.IsWalkable == false)
                    continue;


                var neighbour = new LinkedPathNode()
                {
                    Point = point,
                    ComeFrom = node,
                    PathLengthFromStart = node.PathLengthFromStart + GetDistanceBetweenNeighbours(),
                    HeuristicPathLength = GetHeuristicPathLength(point, goal)
                };

                result.Add(neighbour);
            }

            return result;
        }
        
        public static int GetHeuristicPathLength(LinkedAPoint from,LinkedAPoint to)
        {
            return Math.Abs(from.X - to.X) + Math.Abs(from.Y - to.Y);
        }

        public static int GetDistanceBetweenNeighbours()
        {
            return 1;
        }
    }
}