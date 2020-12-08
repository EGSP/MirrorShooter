using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Egsp.Extensions.Graphs
{
    public abstract class GraphAdapter<TVertexAdapter, TConcreteVertex > : SerializedMonoBehaviour
        where TVertexAdapter : VertexAdapter<TVertexAdapter, TConcreteVertex>
        where TConcreteVertex : Vertex<TConcreteVertex>
    {
        public Graph<TConcreteVertex> Graph { get; protected set; }
        
        [OdinSerialize]
        public List<TVertexAdapter> VertexAdapters { get; protected set; }

        protected virtual void Awake()
        {
            if(VertexAdapters == null)
                return;

            if(Graph == null)
                Graph = new Graph<TConcreteVertex>();
            
            // Заполняем список вершин.
            for (int i = 0; i < VertexAdapters.Count; i++)
            {
                VertexAdapters[i].SetupVertex();
                Graph.Vertices.Add(VertexAdapters[i].Vertex);
            }

            // Запекаем ребра. Устанавливаем ребра между соединениям вершин.
            var verticies = Graph.Vertices;
            for (var i = 0; i < Graph.Vertices.Count; i++)
            {
                verticies[i].BakeEdges();
            }
            
            Graph.ExtractEdges();
        }
        
        public void SearchAdapters()
        {
            var adapters = GetComponentsInChildren<TVertexAdapter>();
            VertexAdapters = adapters.ToList();
        }
        
        [Button("Clear adapters")]
        public virtual void ClearAdapters()
        {
            if (VertexAdapters == null)
                return;

            for (var i = 0; i < VertexAdapters.Count; i++)
            {
                VertexAdapters[i].Clear();   
            }
        }
        
        [Button("Clear adapters connections")]
        public virtual void ClearAdaptersConnections()
        {
            if (VertexAdapters == null)
                return;

            for (var i = 0; i < VertexAdapters.Count; i++)
            {
                VertexAdapters[i].Clear();   
            }
        }
    }
}