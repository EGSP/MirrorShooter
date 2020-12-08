using UnityEngine;

namespace Egsp.Utils.MeshUtilities
{
    public class RendererObject : MonoBehaviour
    {
        /// <summary>
        /// Рендер компонент игрового объекта.
        /// </summary>
        public Renderer renderer { get; set; }

        /// <summary>
        /// Возвращает посредника для установки значений материалу.
        /// </summary>
        public PropertyBlockProxy GetProxy() => new PropertyBlockProxy(renderer);
    }
}