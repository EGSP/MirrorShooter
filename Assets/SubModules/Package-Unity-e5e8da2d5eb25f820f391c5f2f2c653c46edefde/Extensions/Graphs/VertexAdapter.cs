using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace Egsp.Extensions.Graphs
{
    public abstract class VertexAdapter<TVertexAdapter, TConcreteVertex> : SerializedMonoBehaviour
        where TVertexAdapter : VertexAdapter<TVertexAdapter,TConcreteVertex>
        where TConcreteVertex : Vertex<TConcreteVertex>
    {
        [OdinSerialize]
        public TConcreteVertex Vertex { get; protected set; }
        
        [OdinSerialize]
        public List<TVertexAdapter> AdapterConnections { get; protected set; }

        public virtual void SetupVertex()
        {
            for (var i = 0; i < AdapterConnections.Count; i++)
            {
                // Добавление вручную в список для того, чтобы вертексы не подсоидиняли друг друга несколько раз.
                Vertex.Connections.Add(AdapterConnections[i].Vertex);   
            }
        }
        
        public virtual void Connect(TVertexAdapter adapter)
        {
            AddConnection(adapter);
            adapter.AddConnection(This());
        }

        private void AddConnection(TVertexAdapter adapter)
        {
            if (adapter == null && AdapterConnections.Contains(adapter))
                return;
            
            AdapterConnections.Add(adapter);
        }

        public void Remove(TVertexAdapter adapter)
        {
            RemoveConnection(adapter);
            adapter.RemoveConnection(This());
        }

        private void RemoveConnection(TVertexAdapter adapter)
        {
            if (adapter == null && AdapterConnections.Contains(adapter) == false)
                return;

            AdapterConnections.Remove(adapter);
        }

        [Button("Clear connections")]
        public virtual void Clear()
        {
            if (AdapterConnections == null)
                return;
            
            AdapterConnections.Clear();
        }

        protected TVertexAdapter This()
        {
            var concrete = this as TVertexAdapter;
            
            if(concrete == null)
                throw new InvalidCastException();

            return concrete;
        }
    }
}