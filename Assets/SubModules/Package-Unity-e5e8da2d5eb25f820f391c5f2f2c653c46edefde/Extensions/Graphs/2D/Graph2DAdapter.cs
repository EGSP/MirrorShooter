using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Egsp.Extensions.Graphs
{
    public class Graph2DAdapter : GraphAdapter<Vertex2DAdapter, Vertex2D>
    {
        [OdinSerialize]
        public Material ConnectionMaterial { get; private set; }
        
        [OdinSerialize]
        public float LineWidth { get; private set; }
        
        // Нужен только для теста.
        [OdinSerialize]
        public Transform EdgeMarker { get; private set; }

        public float markerTime;

        private IEnumerator _markerRoutine;
        private int _edgeIndex;
        private Vector3 _markerPosition;
        
        private void Update()
        {
            RenderConnections();
            RenderEdgeMarker();
        }

        public void RenderConnections()
        {
            for (var i = 0; i < VertexAdapters.Count; i++)
            {
                VertexAdapters[i].Render(LineWidth, ConnectionMaterial);
            }
        }

        public void RenderEdgeMarker()
        {
            if (EdgeMarker == null)
                return;
            
            // Рендер картинки.
            EdgeMarker.position = _markerPosition;

            // Снизу смена позиции.
            if (_markerRoutine != null)
                return;
            
            Debug.Log("RenderMarker");
            var edges = Graph.Edges;

            if (edges.Count == 0)
                return;
            
            if (_edgeIndex >= edges.Count)
                _edgeIndex = 0;

            var edge = edges[_edgeIndex];
            _edgeIndex++;
            
            _markerRoutine = MarkerRoutine(edge);
            StartCoroutine(_markerRoutine);
        }

        private IEnumerator MarkerRoutine(Edge<Vertex2D> edge)
        {
            _markerPosition = Vector3.Lerp(edge.From.Position, edge.To.Position, 0.5f);
            yield return new WaitForSeconds(markerTime);
            _markerRoutine = null;
        }
    }
}