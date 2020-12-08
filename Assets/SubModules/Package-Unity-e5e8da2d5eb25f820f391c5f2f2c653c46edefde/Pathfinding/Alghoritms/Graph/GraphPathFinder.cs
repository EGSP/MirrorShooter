using System;
using System.Collections.Generic;
using System.Linq;
using Egsp.Extensions.Graphs;
using Egsp.Extensions.Linq;
using UnityEditor.SceneManagement;

namespace Egsp.Core.Pathfinding
{
    /// <summary>
    /// Поиск пути по графу, использующий граф этого пакета.
    /// </summary>
    /// <typeparam name="TVertex"></typeparam>
    public sealed class GraphPathFinder<TVertex> 
        where TVertex : Vertex<TVertex>
    {
        public List<TVertex> FindPath(TVertex start, TVertex endVertex)
        {
            
            if (start == null || endVertex == null)
                return new List<TVertex>();

            var openNodes = new List<GraphPathNode<TVertex>>();
            var closedNodes = new List<GraphPathNode<TVertex>>();
            
            var startNode = new GraphPathNode<TVertex>(start, null)
            {
                PathLength = 0f,
                DistanceToTarget = start.DistanceTo(endVertex),
                ElementsBefore = 0
            };
            openNodes.Add(startNode);

            int exception = 0;
            while (openNodes.Count > 0)
            {
                exception++;
                if(exception > 10_000)
                    throw new Exception("Infinity path finding!");

                // Индекс ближайшей точки к конечной.
                var nearestNodeIndex = openNodes.MinIndex(x 
                    => x.FullLength);
                var nearestNode = openNodes[nearestNodeIndex];

                if (nearestNode.Vertex == endVertex)
                {
                    // Создаем список и возвращаем его.
                    var path = FormPath(nearestNode);
                    return path;
                }
                
                openNodes.RemoveAt(nearestNodeIndex);
                closedNodes.Add(nearestNode);

                var neighbours = GetNeighbourNodes(nearestNode, endVertex);

                for (var i = 0; i < neighbours.Count; i++)
                {
                    var neighbour = neighbours[i];

                    // Если сосед уже был на рассмотрении.
                    if (closedNodes.Exists(x => x.Vertex == neighbour.Vertex))
                        continue;

                    // Если сосед уже был в откртом списке.
                    var originalNode = openNodes.FirstOrDefault(x 
                        => x.Vertex == neighbour.Vertex);

                    if (originalNode == null)
                    {
                        openNodes.Add(neighbour);
                    }
                    else
                    {
                        // Если длина пути оригинальной ноды больше текущей.
                        if (originalNode.PathLength > neighbour.PathLength)
                        {
                            originalNode.ComeFrom = neighbour.ComeFrom;
                            originalNode.PathLength = neighbour.PathLength;
                            originalNode.ElementsBefore = neighbour.ElementsBefore;
                        }
                    }

                }
            }
            
            return new List<TVertex>();
        }

        /// <summary>
        /// Получение соседних вертексов и формирование из них нод.
        /// </summary>
        private List<GraphPathNode<TVertex>> GetNeighbourNodes(GraphPathNode<TVertex> node, 
            TVertex endVertex)
        {
            var nodes = new List<GraphPathNode<TVertex>>();
            var verticies = node.Vertex.Connections;

            for (var i = 0; i < verticies.Count; i++)
            {
                var neighbourNode = new GraphPathNode<TVertex>(verticies[i], node)
                {
                    Vertex = verticies[i],
                    ComeFrom = node,
                    PathLength = node.PathLength + node.Vertex.DistanceTo(verticies[i]),
                    DistanceToTarget = node.Vertex.DistanceTo(endVertex),
                    ElementsBefore = node.ElementsBefore + 1
                };
                
                nodes.Add(neighbourNode);
            }

            return nodes;
        }

        /// <summary>
        /// Формирование пути.
        /// </summary>
        private List<TVertex> FormPath(GraphPathNode<TVertex> pathNode)
        {
            var array = new TVertex[pathNode.ElementsBefore + 1];
            
            for (var i = array.Length - 1; i < array.Length; i--)
            {
                array[i] = pathNode.Vertex;
                pathNode = pathNode.ComeFrom;
            }

            return array.ToList();
        }

        public sealed class GraphPathNode<TVertex>
            where TVertex : Vertex<TVertex>
        {
            // Вертекс ноды.
            public TVertex Vertex { get; set; }
            
            // Нода из которой пришли. Если значение null, то это стартовая нода.
            public GraphPathNode<TVertex> ComeFrom { get; set; }

            // Длина пути до этой ноды. (G)
            public float PathLength { get; set; }
            
            // Расстояние между вертексом и конечной точкой. (H)
            public float DistanceToTarget { get; set; }
            
            public int ElementsBefore { get; set; }

            /// <summary>
            /// Расстояние G + H
            /// </summary>
            public float FullLength => PathLength + DistanceToTarget;
            
            public GraphPathNode(TVertex vertex, GraphPathNode<TVertex> comeFrom)
            {
                Vertex = vertex;
                ComeFrom = comeFrom;
            }

            public override string ToString()
            {
                return Vertex.ToString();
            }
        }
    }
}