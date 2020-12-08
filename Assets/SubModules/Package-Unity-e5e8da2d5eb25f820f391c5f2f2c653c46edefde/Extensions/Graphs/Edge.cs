using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace Egsp.Extensions.Graphs
{
    [Serializable]
    public class Edge<TVertex>
        where TVertex : Vertex<TVertex>
    {
        public Edge(TVertex from, TVertex to)
        {
            From = from;
            To = to;
        }

        [ReadOnly][OdinSerialize]
        public TVertex From { get; private set; }
        
        [ReadOnly][OdinSerialize]
        public TVertex To { get; private set; }
    }
}