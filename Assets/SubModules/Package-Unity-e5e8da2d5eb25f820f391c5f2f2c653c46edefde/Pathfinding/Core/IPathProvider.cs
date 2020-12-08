namespace Egsp.Core.Pathfinding
{
    public interface IPathProvider<TPoint>
    {
        IPathResponse<TPoint> RequestPath(TPoint entryPoint, TPoint endPoint);
    }
}