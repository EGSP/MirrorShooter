using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace Egsp.Extensions.Graphs
{
    /// <summary>
    /// Представление вершины графа.
    /// </summary>
    [Serializable]
    public abstract class Vertex<TConcreteVertex>
        where TConcreteVertex : Vertex<TConcreteVertex>
    {
        protected Vertex()
        {
            if(Connections == null)
                Connections = new List<TConcreteVertex>();
            
            if(Out == null)
                Out = new List<Edge<TConcreteVertex>>();
        }
        
        /// <summary>
        /// Связи с другими вершинами.
        /// Связь указывает на то, что из этой вершины можно попасть в соединенную.
        /// </summary>
        [OdinSerialize]
        public List<TConcreteVertex> Connections { get; protected set; }

        
        /// <summary>
        /// Ребра выходов. Запекаются на основе соединений.
        /// </summary>
        [ReadOnly][OdinSerialize]
        public List<Edge<TConcreteVertex>> Out { get; protected set; }

        /// <summary>
        /// Расстояние между текущей вершиной и передаваемой.
        /// </summary>
        public abstract float DistanceTo(TConcreteVertex vertex);

        /// <summary>
        /// Соединение текущей вершины с передаваемой.
        /// По стандарту передаваемая вершина тоже будет подключена к текущей.
        /// </summary>
        public virtual void Connect(TConcreteVertex vertex)
        {
            // Запись в собственный список.
            AddConnection(vertex);
            // Попытка связать другую вершину с собой.
            vertex.AcceptVertex(vertex);
        }

        protected virtual void AcceptVertex(TConcreteVertex vertex)
        {
            AddConnection(vertex);
        }

        protected void AddConnection(TConcreteVertex vertex)
        {
            if (Connections.Contains(vertex))
                return;

            Connections.Add(vertex);
        }

        public virtual bool BakeEdges()
        {
            if (Connections == null)
                return false;

            if (Out == null)
            {
                Out = new List<Edge<TConcreteVertex>>();
            }
            else
            {
                Out.Clear();
            }
            
            for (var i = 0; i < Connections.Count; i++)
            {
                var edge = new Edge<TConcreteVertex>(This(), Connections[i]);
                Out.Add(edge);
            }

            return true;
        }

        public virtual void Clear()
        {
            if(Connections != null)
                Connections.Clear();
            
            if(Out != null)
                Out.Clear();
        }

        protected TConcreteVertex This()
        {
            var concrete = this as TConcreteVertex;
            
            if(concrete == null)
                throw new InvalidCastException();
            
            return concrete;
        }
    }
}