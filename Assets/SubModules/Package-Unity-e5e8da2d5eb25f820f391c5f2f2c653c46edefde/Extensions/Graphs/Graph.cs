using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.Serialization;

namespace Egsp.Extensions.Graphs
{

    [Serializable]
    public class Graph<TVertex>
        where TVertex : Vertex<TVertex>
    {
        public Graph()
        {
            Vertices = new List<TVertex>();
            Edges = new List<Edge<TVertex>>();
        }

        /// <summary>
        /// Вершины графа.
        /// </summary>
        [OdinSerialize]
        public List<TVertex> Vertices { get; protected set; }
        
        /// <summary>
        /// Ребра графа.
        /// Так то все ребра можно найти через вертексы,
        /// поэтому данный список нужен лишь для конкретного взаимодействия с ребрами.
        /// </summary>
        public List<Edge<TVertex>> Edges { get; protected set; }

        public void ExtractEdges()
        {
            if (Vertices == null)
                return;

            if (Edges == null)
            {
                Edges = new List<Edge<TVertex>>();
            }
            else
            {
                Edges.Clear();
            }

            // Проход по каждому вертексу.
            for (var v = 0; v < Vertices.Count; v++)
            {
                var vertex = Vertices[v];

                // Добавление ребер.
                for (var e = 0; e < vertex.Out.Count; e++)
                {
                    Edges.Add(vertex.Out[e]);
                }
            }
        }
    }
}