
using UnityEngine;

namespace Egsp.Utils.MeshUtilities
{
    public static class MeshUtils
    {
        private static readonly Shader StandardShader;
        private static readonly Material StandardMaterial;
        
        static MeshUtils()
        {
            StandardShader = Shader.Find("Sprites/Default");
            StandardMaterial = new Material(StandardShader);
        }
        
        public static Mesh CreateEmptyMesh()
        {
            Mesh mesh = new Mesh();
            return mesh;
        }

        /// <summary>
        /// Создает меш квадрата с заданными шириной и высостой
        /// </summary>
        public static Mesh Quad(float width, float height)
        {
            Vector3[] vertices;
            Vector2[] uv;
            int[] triangles;
            
            CreateEmptyQuadData(1,out vertices,out uv,out triangles);

            vertices[0] = Vector3.zero;
            vertices[1] = new Vector3(0,height);
            vertices[2] = new Vector3(width,height);
            vertices[3] = new Vector3(width,0);
            
            uv[0] = Vector2.zero;
            uv[1] = Vector2.up;
            uv[2] = Vector2.one;
            uv[3] = Vector2.right;

            triangles[0] = 0;
            triangles[1] = 1;
            triangles[2] = 2;
            
            triangles[3] = 0;
            triangles[4] = 2;
            triangles[5] = 3;

            var mesh = new Mesh();

            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.triangles = triangles;
            return mesh;
        }
        
        /// <summary>
        /// Создает пустые массивы данных меша в зависимости от количества квадратов
        /// </summary>
        public static void CreateEmptyQuadData(int quadCount,out Vector3[] vertices, out Vector2[] uv, out int[] triangles)
        {
            vertices = new Vector3[4*quadCount];
            uv = new Vector2[4*quadCount];
            triangles = new int[6*quadCount];
        }

        /// <summary>
        /// Создает игровой объект квадрат без материала
        /// </summary>
        public static GameObject CreateQuadObject(float width, float height)
        {
            var mesh = Quad(width, height);
            var meshObject = new GameObject("QuadObject");

            var meshFilter = meshObject.AddComponent<MeshFilter>();
            meshFilter.mesh = mesh;
            
            return meshObject;
        }

        /// <summary>
        /// Создает игровой объект квадрат с материалом
        /// </summary>
        public static GameObject CreateQuadObject(float width, float height, Material material)
        {
            var meshObject = CreateQuadObject(width, height);
            var meshRenderer = meshObject.AddComponent<MeshRenderer>();
            meshRenderer.material = material;

            return meshObject;
        }

        /// <summary>
        /// Изменяет размер квадрата
        /// </summary>
        /// <param name="quad">Меш квадрата</param>
        /// <param name="width">Ширина</param>
        /// <param name="height">Высота</param>
        public static void ChangeSizeQuadMesh(Mesh quad, float width, float height)
        {
            Vector3[] vertices;

            vertices = quad.vertices;

            vertices[0] = Vector3.zero;
            vertices[1] = Vector3.up * height;
            vertices[2] = new Vector3(width, height);
            vertices[3] = Vector3.right * width;

            quad.vertices = vertices;
        }


        /// <summary>
        /// Создает меш в форме ромба.
        /// </summary>
        public static Mesh Diamond(float horizontalSize, float verticalSize)
        {
            Vector3[] vertices;
            Vector2[] uv;
            int[] triangles;
            
            CreateEmptyQuadData(1,out vertices, out uv, out triangles);
            
            // Относительно центра
            // Снизу
            vertices[0] = new Vector3(0,-(verticalSize/2f));
            // Слева
            vertices[1] = new Vector3(-(horizontalSize/2f),0);
            // Сверху
            vertices[2] = new Vector3(0,+(verticalSize/2f));
            // Справа
            vertices[3] = new Vector3(+(horizontalSize/2f),0);

            uv[0] = Vector2.zero;
            uv[1] = new Vector2(0,1);
            uv[2] = new Vector2(1,1);
            uv[3] = new Vector2(1,0);

            triangles[0] = 0;
            triangles[1] = 1;
            triangles[2] = 2;

            triangles[3] = 0;
            triangles[4] = 2;
            triangles[5] = 3;
            
            var mesh = new Mesh();

            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.triangles = triangles;

            return mesh;
        }

        
        
        /// <param name="plane">Для 2D нужно использовать Vector3.forward (перпендикуляр направлен вниз).</param>
        /// <param name="worldPosition">Начало линии будет в точке from при значении true.</param>
        public static Mesh Line(Vector3 from, Vector3 to, Vector3 plane, bool worldPosition, float width = 1f)
        {
            Vector3[] vertices;
            Vector2[] uv;
            int[] triangles;
            
            CreateEmptyQuadData(1,out vertices,out uv, out triangles);
        
            var direction = (to - from).normalized;
            var distance = Vector3.Distance(from, to);
            var perpendicular = Vector3.Cross(direction, plane).normalized;
            
            var offset = Vector3.zero;
            if (worldPosition)
                offset += from;

            vertices[0] =  offset + perpendicular * width;
            vertices[1] =  offset - perpendicular * width;
            vertices[2] = direction * distance + offset - perpendicular * width;
            vertices[3] = direction * distance + offset + perpendicular * width;
            
            uv[0] = Vector2.zero;
            uv[1] = new Vector2(0,1);
            uv[2] = new Vector2(1,1);
            uv[3] = new Vector2(1,0);

            triangles[0] = 0;
            triangles[1] = 1;
            triangles[2] = 2;

            triangles[3] = 0;
            triangles[4] = 2;
            triangles[5] = 3;
            
            var mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.triangles = triangles;

            return mesh;
        }

        public static void DrawLine(Vector3 from, Vector3 to, float width = 1f)
        {
            var lineMesh = Line(from, to, Vector3.forward, true, width);
            Graphics.DrawMesh(lineMesh,Vector3.zero,Quaternion.identity,StandardMaterial,0);
        }

        public static void DrawLine(Vector3 from, Vector3 to, float width, Material material)
        {
            var lineMesh = Line(from, to, Vector3.forward, true, width);
            Graphics.DrawMesh(lineMesh,Vector3.zero,Quaternion.identity,material,0);

        }

        /// <summary>
        /// Создает объект-носитель для меша.
        /// </summary>
        public static GameObject ToGameObject(this Mesh mesh, string name = "FromMesh",
            Material material = null )
        {
            var gameObject = new GameObject(name);

            var meshFilter = gameObject.AddComponent<MeshFilter>();
            meshFilter.mesh = mesh;

            if (material == null)
                material = StandardMaterial;
            
            var meshRenderer = gameObject.AddComponent<MeshRenderer>();
            meshRenderer.material = material;

            return gameObject;
        }
        
        /// <summary>
        /// Создает объект-носитель для меша.
        /// </summary>
        public static GameObject ToGameObject(this Mesh mesh, Material material, string name = "FromMesh")
        {
            return ToGameObject(mesh, name, material);
        }
        
        /// <summary>
        /// Создает объект-носитель для меша.
        /// </summary>
        public static RendererObject ToRenderer(this Mesh mesh, string name = "FromMesh",
            Material material = null )
        {
            var gameObject = new GameObject(name, typeof(RendererObject));
            var rendererObject = gameObject.GetComponent<RendererObject>();

            var meshFilter = gameObject.AddComponent<MeshFilter>();
            meshFilter.mesh = mesh;

            if (material == null)
                material = StandardMaterial;
            
            var meshRenderer = gameObject.AddComponent<MeshRenderer>();
            
            meshRenderer.material = material;
            
            rendererObject.renderer = meshRenderer;

            return rendererObject;
        }
        
        /// <summary>
        /// Создает объект-носитель для меша.
        /// </summary>
        public static RendererObject ToRenderer(this Mesh mesh, Material material, string name = "FromMesh")
        {
            return ToRenderer(mesh, name, material);
        }
    }
}