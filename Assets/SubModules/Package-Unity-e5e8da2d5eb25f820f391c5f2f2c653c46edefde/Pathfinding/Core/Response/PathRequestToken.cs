namespace Egsp.Core.Pathfinding
{
    /// <summary>
    /// Прослойка между запросом и поиском пути.
    /// </summary>
    public sealed class PathRequestToken<TPoint>
    {
        public bool IsReady { get; private set; }
        public IPath<TPoint> Path { get; private set; }
        
        public void Ready(IPath<TPoint> path)
        {
            if (path == null)
                throw new EmptyPathException();
            
            IsReady = true;
            Path = path;
        }        
    }
}