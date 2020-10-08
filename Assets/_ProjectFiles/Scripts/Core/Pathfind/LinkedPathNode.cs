namespace Gasanov.Pathfind
{
    public class LinkedPathNode
    {
        /// <summary>
        /// Точка с координатами на сетке
        /// </summary>
        public LinkedAPoint Point { get; set; }

        /// <summary>
        /// Узел из которого пришли
        /// </summary>
        public LinkedPathNode ComeFrom { get; set; }

        /// <summary>
        /// Длинна пути от старта "G"
        /// </summary>
        public int PathLengthFromStart { get; set; }

        /// <summary>
        /// Примерное расстояние до цели "H"
        /// </summary>
        public int HeuristicPathLength { get; set; }

        /// <summary>
        /// Ожидаемое полное расстояние до цели "F"
        /// </summary>
        public int FullPathLength
            => PathLengthFromStart + HeuristicPathLength;

        public override string ToString()
        {
            return "PathNode: " + Point.ToString();
        }
    }
    
}