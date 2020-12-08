using UnityEngine;

namespace Egsp.Extensions.Graphs
{
    public class Vertex2D : Vertex<Vertex2D>
    {
        public Vector2 Position;

        public override float DistanceTo(Vertex2D vertex)
        {
            return Vector2.Distance(Position, vertex.Position);
        }
    }
}