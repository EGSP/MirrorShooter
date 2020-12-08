namespace Egsp.Core.Pathfinding
{
    /// <summary>
    /// Данному типу ответа не нужен токен. 
    /// </summary>
    public sealed class PathResponseSync<TPoint> : IPathResponse<TPoint>
    {
        public PathResponseSync(IPath<TPoint> path)
        {
            if(path == null)
                throw new EmptyPathException();

            Path = path;
            Ready = true;
        }
        
        public bool Ready { get; private set; }
        public IPath<TPoint> Path { get; private set; }
    }
}