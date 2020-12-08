using System;
using System.Collections.Generic;
using Egsp.Utils.MeshUtilities;
using Sirenix.Serialization;
using UnityEngine;

namespace Egsp.Extensions.Graphs
{
    [SelectionBase]
    public class Vertex2DAdapter : VertexAdapter<Vertex2DAdapter, Vertex2D>
    {
        public override void SetupVertex()
        {
            Vertex.Position = transform.position;
            base.SetupVertex();
        }

        public void Render()
        {
            if (AdapterConnections.Count == 0)
                return;

            for (var i = 0; i < AdapterConnections.Count; i++)
            {
                MeshUtils.DrawLine(transform.position, AdapterConnections[i].transform.position, 0.01f);
            }
        }

        public void Render(float width, Material lineMaterial)
        {
            if (AdapterConnections.Count == 0)
                return;

            for (var i = 0; i < AdapterConnections.Count; i++)
            {
                MeshUtils.DrawLine(transform.position, AdapterConnections[i].transform.position, width,
                    lineMaterial);
            }
        }
    }
}