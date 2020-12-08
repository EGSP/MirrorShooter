namespace Egsp.Core.Pathfinding
{
    /// <summary>
    /// Ответ на запрос поиска пути.
    /// </summary>
    public sealed class PathResponse<TPoint> : IPathResponse<TPoint>
    {
        private readonly PathRequestToken<TPoint> _token;

        public bool Ready => _token.IsReady;
        public IPath<TPoint> Path => _token.Path;

        public PathResponse(PathRequestToken<TPoint> token)
        {
            _token = token;
        }

        /// <summary>
        /// Метод для упрощенного создания объектов ответа и токена.
        /// </summary>
        public static void New(out PathRequestToken<TPoint> token, out PathResponse<TPoint> response)
        {
            token = new PathRequestToken<TPoint>();
            response = new PathResponse<TPoint>(token);
        }
    }
}