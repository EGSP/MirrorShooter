using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Linq;

using Sirenix.Serialization;

namespace Gasanov.Extensions.Collections
{
    public class Grid<TObject>
    {
        [OdinSerialize]
        /// <summary>
        /// Количество ячеек по горизонтали
        /// </summary>
        public int Width { get; protected set; }

        [OdinSerialize]
        /// <summary>
        ///  Количество ячеек по вертикали
        /// </summary>
        public int Height { get; protected set; }

        [OdinSerialize]
        /// <summary>
        ///  Размер ячейки по горизонтали
        /// </summary>
        public float CellSizeHorizontal { get; set; }

        [OdinSerialize]
        /// <summary>
        /// Размер ячейки по вертикали
        /// </summary>
        public float CellSizeVertical { get; set; }

        [OdinSerialize]
        /// <summary>
        /// Массив объектов сетки
        /// </summary>
        protected List<List<TObject>> GridList2D { get; set; }

        /// <summary>
        /// Количество ячеек.
        /// </summary>
        public int Count => Width * Height;

        /// <summary>
        /// Вызывается при изменении значения сетки. Передает два индекса и изменяемы объект
        /// </summary>
        public event Action<int,int,TObject> OnGridObjectChanged = delegate { };

        /// <summary>
        /// Вызывается при удалении объекта из сетки
        /// </summary>
        public event Action<int, int, TObject> OnGridObjectDeleted = delegate { };

        /// <summary>
        /// Вызывается при создании объекта. Объект является значением default(TObject)
        /// </summary>
        public event Action<int, int, TObject> OnGridObjectCreated = delegate { };

        public TObject this[int x, int y]
        {
            get => GridList2D[x][y];
            set => GridList2D[x][y] = value;
        }

        protected Grid()
        {
            // parameterless constructor
        }

        public Grid(int width, int height, float cellSizeHorizontal = 1, float cellSizeVertical = 1)
        {
            this.Width = width;
            this.Height = height;

            this.CellSizeHorizontal = cellSizeHorizontal;
            this.CellSizeVertical = cellSizeVertical;

            GridList2D = new List<List<TObject>>(Width);

            for (var x = 0; x < Width; x++)
            {
                GridList2D.Add(new List<TObject>(Height));
                for (var y = 0; y < Height; y++)
                {
                    GridList2D[x].Add(default(TObject));
                }
            }

        }

        /// <param name="createTObject">Функция создания объекта</param>
        public Grid(int width, int height, Func<TObject> createTObject)
            : this(width, height)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    GridList2D[x][y] = createTObject();
                }
            }
        }

        /// <param name="createTObject">Функция создания объекта</param>
        public Grid(int width, int height, float cellSizeHorizontal, float cellSizeVertical, Func<TObject> createTObject)
            : this(width, height, cellSizeHorizontal, cellSizeVertical)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    GridList2D[x][y] = createTObject();
                }
            }
        }
        
        /// <param name="createTObject">Функция создания объекта</param>
        public Grid(int width, int height, Func<int,int,TObject> createTObject)
            : this(width, height)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    GridList2D[x][y] = createTObject(x,y);
                }
            }
        }

        /// <param name="createTObject">Функция создания объекта. Получает координаты ячейки</param>
        public Grid(int width, int height, float cellSizeHorizontal, float cellSizeVertical, Func<int, int, TObject> createTObject)
            : this(width, height, cellSizeHorizontal, cellSizeVertical)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    GridList2D[x][y] = createTObject(x, y);
                }
            }
        }




        /// <summary>
        /// Получение позиции в мировых координатах
        /// </summary>
        public Vector3 GetWorldPosition(int x,int y)
        {
            return new Vector3(x * CellSizeHorizontal, y * CellSizeVertical);
        }

        /// <summary>
        /// Получение позиции центра ячейки в мировых координатах
        /// </summary>
        public Vector3 GetCentralizedWorldPostion(int x,int y)
        {
            return new Vector3(x * CellSizeHorizontal, y * CellSizeVertical) + new Vector3(CellSizeHorizontal, CellSizeVertical) * 0.5f;
        }

        /// <summary>
        /// Возвращает мировую позицию нижнего левого угла
        /// </summary>
        public Vector3 GetBottomLeftCornerPosition(int x, int y)
        {
            return new Vector3(x * CellSizeHorizontal, y * CellSizeVertical);
        }

        /// <summary>
        /// Возвращает мировую позицию верхнего правого угла
        /// </summary>
        public Vector3 GetTopRightCornerPosition(int x, int y)
        {
            return new Vector3(x * CellSizeHorizontal, y * CellSizeVertical) + new Vector3(CellSizeHorizontal, CellSizeVertical);
        }

        /// <summary>
        /// Получение индексов из мировой позиции. 
        /// Возвращает отрицательные числа тоже
        /// </summary>
        public void WorldToIndex(Vector3 worldPos, out int x, out int y)
        {
            x = Mathf.FloorToInt(worldPos.x / CellSizeHorizontal);
            y = Mathf.FloorToInt(worldPos.y / CellSizeVertical);
        }
        
        /// <summary>
        /// Находятся ли индексы в пределах сетки
        /// </summary>
        public bool InBounds(int x, int y)
        {
            if (x >= 0 && y >= 0 && x < Width && y < Height)
                return true;

            return false;
        }
        
        /// <summary>
        /// Находится ли мировая позиция в пределах сетки
        /// </summary>
        public bool InBounds(Vector3 worldPos)
        {
            int x, y;
            WorldToIndex(worldPos,out x,out y);

            return InBounds(x, y);
        }

        //
        // Я сделал анонимные методы для того,
        // чтобы при отсутствии объекта или некорректных координатах не возвращать throw или null
        //  
        // Рекомендуется использовать PopObject вместо прямого получения объекта с помощью GetObject
        //

        /// <summary>
        /// Вызывает метод popAction, передавая объект полученный по индексам. 
        /// Если объект не может быть получен, то popAction не вызовется
        /// </summary>
        /// <param name="popAction">Выполняемый метод</param>
        public void PopObject(int x, int y, Action<TObject> popAction)
        {
            if (InBounds(x,y))
                popAction(GridList2D[x][y]);
        }

        /// <summary>
        /// Вызывает метод popAction, передавая объект полученный по индексам. 
        /// Если объект не может быть получен, то popAction не вызовется
        /// </summary>
        /// <param name="popAction">Выполняемый метод</param>
        public void PopObject(Vector3 worldPos, Action<TObject> popAction)
        {
            int x, y;
            WorldToIndex(worldPos,out x,out y);
            // Проверка на отрицательный индекс внутри
            PopObject(x, y, popAction);
        }

        /// <summary>
        /// Вызывает метод popAction, передавая объект полученный по индексам. 
        /// Если объект не может быть получен, то popAction не вызовется
        /// </summary>
        /// <param name="popAction">Передает кроме объекта индексы в сетке</param>
        public void PopObject(int x,int y, Action<int,int,TObject> popAction)
        {
            if (InBounds(x,y))
                popAction(x, y, GridList2D[x][y]);
        }

        /// <summary>
        /// Вызывает метод popAction, передавая объект полученный по индексам. 
        /// Если объект не может быть получен, то popAction не вызовется
        /// </summary>
        /// <param name="popAction">Передает кроме объекта индексы в сетке</param>
        public void PopObject(Vector3 worldPos, Action<int,int,TObject> popAction)
        {
            int x, y;
            WorldToIndex(worldPos, out x, out y);
            // Проверка на отрицательный индекс внутри
            PopObject(x, y, popAction);
        }

        /// <summary>
        /// Получение объекта по индексу
        /// </summary>
        public TObject GetObject(int x,int y)
        {
            if (InBounds(x,y))
                return GridList2D[x][y];

            return default(TObject);
        }

        /// <summary>
        /// Получение объекта по мировым координатам
        /// </summary>
        public TObject GetObject(Vector3 worldPos)
        {
            int x, y;
            WorldToIndex(worldPos, out x, out y);
            return GetObject(x, y);
        }

        /// <summary>
        /// Устанавливает новый объект (ссылочный тип).
        /// Изменяет значение (значимый тип)
        /// </summary>
        public void SetObject(int x,int y, TObject newObject)
        {
            if (InBounds(x,y))
            {
                // Debug.Log($"{x},{y} : {Width},{Height} : {GridArray.Count},{GridArray[x].Count}");
                GridList2D[x][y] = newObject;
                OnGridObjectChanged(x, y, newObject);
            }
        }

        /// <summary>
        /// Устанавливает новый объект (ссылочный тип).
        /// Изменяет значение (значимый тип)
        /// </summary>
        public void SetObject(Vector3 worldPos, TObject newObject)
        {
            int x, y;
            WorldToIndex(worldPos,out x, out y);
            SetObject(x, y, newObject);
        }

        /// <summary>
        /// Проходится по каждой ячейке и вызывает метод.
        /// </summary>
        /// <param name="action">Вызываемый метод. Аргументами являются индексы в сетке</param>
        public void ForEach(Action<int,int,TObject> action)
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    action(x, y,GridList2D[x][y]);
                }
            }
        }

        /// <summary>
        /// Проходится по каждой ячейке и устанавливает значение initFunction.
        /// </summary>
        /// <param name="initFunction"></param>
        public void ForEachSet(Func<int, int, TObject> initFunction)
        {
            for (var x = 0; x < Width; x++)
            {
                for (var y = 0; y < Height; y++)
                {
                    var tObject = initFunction(x, y);

                    GridList2D[x][y] = tObject;
                }
            }
        }

        /// <summary>
        /// Вызывать при изменении объекта (ссылочный тип)
        /// </summary>
        /// <param name="value">Изменяемый объект</param>
        public void GridObjectChanged(int x,int y,TObject value)
        {
            OnGridObjectChanged(x,y,value);
        }

        /// <summary>
        /// Изменяет размер сетки
        /// </summary>
        /// <param name="newWidth">Новая ширина</param>
        /// <param name="newHeight">Новая длина</param>
        public void Resize(int newWidth, int newHeight)
        {
            var oldWidth = Width;
            var oldHeight = Height;

            Width = newWidth;
            Height = newHeight;
            
            // Высоту менять перед шириной, т.к. при изменении высоты на не нужно создавать новые столбцы
            List<Action> onGridObjectCreatedAction = new List<Action>();
            List<Action> onGridObjectDeletedAction = new List<Action>();
            
            if (newHeight != oldHeight)
            {
                // Новая высота меньше текущей
                if (newHeight < oldHeight)
                {
                    // Проходимся по всем столбцам
                    for (var x = 0; x < oldWidth; x++)
                    {
                        // Проходимся сверху вниз у столбца
                        for (var y = oldHeight - 1; y > newHeight - 1; y--)
                        {
                            var lx = x;
                            var ly = y;
                            OnGridObjectDeletedHandler(lx,ly, GridList2D[lx][ly]);
                            
                            GridList2D[x].RemoveAt(y);
                        }
                        GridList2D[x].Capacity = newHeight;
                    }
                }
                else // Новая высота больше текущей
                {
                    for (var x = 0; x < oldWidth; x++)
                    {
                        GridList2D[x].Capacity = newHeight;
                        for (var s = newHeight - oldHeight; s > 0; s--)
                        {
                            // Добавляем пустышки в столбец
                            GridList2D[x].Add(default(TObject));
                            
                            var lx = x;
                            var ly = newHeight - s;
                            OnGridObjectCreatedHandler(lx, ly, default(TObject));
                        }
                    }
                }
            }
           
            
            // Изменяем ширину
            if (newWidth != oldWidth)
            {
                // Новая ширина меньше текущей
                if (newWidth < oldWidth)
                {
                    // Удаляем столбцы
                    for (var x = oldWidth-1; x > newWidth-1; x--)
                    {
                        for (var y = 0; y < newHeight; y++)
                        {
                            var lx = x;
                            var ly = y;
                            // Оповещаем об удалении
                            OnGridObjectDeletedHandler(lx, ly, GridList2D[lx][ly]);
                        }
                        // Удаляем столбец (от конца)
                        GridList2D.RemoveAt(x);
                    }
                }
                else // Новая ширина больше текущей
                {
                    // Добавляем новые столбцы
                    for (var s = newWidth - oldWidth; s > 0; s--)
                    {
                        var newColumn = new List<TObject>(newHeight);
                        GridList2D.Add(newColumn);
                        // Оповещаем о создании ячеек
                        for (var y = 0; y < newHeight; y++)
                        {
                            var lx = newWidth - s;
                            var ly = y;
                            
                            GridList2D[lx].Add(default(TObject));
                            // Debug.Log($"{lx}:{ly} lxly");
                            OnGridObjectCreatedHandler(lx, ly, default(TObject));
                        }
                    }
                }

                GridList2D.Capacity = newWidth;

            }
        }

        private void OnGridObjectCreatedHandler(int x,int y,TObject createdObject)
        {
            OnGridObjectCreated(x, y, createdObject);
        }

        private void OnGridObjectDeletedHandler(int x,int y,TObject deletedObject)
        {
            OnGridObjectDeleted(x, y, deletedObject);
        }

        /// <summary>
        /// Возвращает двумерный массив элементов сетки.
        /// </summary>
        public TObject[,] ToArray2D()
        {
            var array = new TObject[Width,Height];
            
            ForEach((x,y,obj) =>
            {
                array[x, y] = obj;
            });

            return array;
        }
        
        
        
        
        
        
        
        /// <summary>
        /// Возвращает позицию в зависимости от размерности ячеек
        /// </summary>
        /// <param name="position">Позиция в мировом пространстве</param>
        public Vector3 SnapToGridDimension(Vector3 position)
        {
            position.x = Mathf.Floor(position.x / CellSizeHorizontal)
                         * CellSizeHorizontal;
            
            position.y = Mathf.Floor(position.y / CellSizeVertical)
                         * CellSizeVertical;

            return position;
        }
        
        /// <summary>
        /// Возвращает позицию в зависимости от размерности ячеек, но центрированную
        /// </summary>
        public Vector3 SnapToGridDimensionCentralized(Vector3 position)
        {
            position.x = Mathf.Floor(position.x / CellSizeHorizontal)
                * CellSizeHorizontal+CellSizeHorizontal*0.5f;
            
            position.y = Mathf.Floor(position.y / CellSizeVertical)
                * CellSizeVertical+CellSizeVertical*0.5f;

            return position;
        }

        /// <summary>
        /// Возвращает позицию ближайшей ячейки
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public Vector3 GetNearestCellCentralized(Vector3 position)
        {
            position = SnapToGridDimensionCentralized(position);
            
            int x, y;
            WorldToIndex(position,out x,out y);
            
            if (InBounds(x,y))
                return position;

            x = Mathf.Clamp(x, 0, Width-1);
            y = Mathf.Clamp(y, 0, Height-1);

            return GetCentralizedWorldPostion(x, y);

        }
        
        public Vector3 GetNearestCellCentralized(Vector3 position,out int x,out int y)
        {
            position = SnapToGridDimensionCentralized(position);
            
            WorldToIndex(position,out x,out y);
            
            if (InBounds(x,y))
                return position;

            x = Mathf.Clamp(x, 0, Width-1);
            y = Mathf.Clamp(y, 0, Height-1);

            return GetCentralizedWorldPostion(x, y);
        }
        
    }
}