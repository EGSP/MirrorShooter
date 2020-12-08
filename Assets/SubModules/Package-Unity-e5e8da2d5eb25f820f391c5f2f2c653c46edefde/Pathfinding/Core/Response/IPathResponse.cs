namespace Egsp.Core.Pathfinding
{
    public interface IPathResponse<TPoint>
    {
        bool Ready { get; }
        
        IPath<TPoint> Path { get; }
    }
}