namespace Gasanov.Core.ObjectPooling
{
    public interface IPoolContainer
    {
        /// <summary>
        /// Название пула
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Чистит пул. Удаляет все экземпляры объектов пула
        /// </summary>
        void Clean();

        /// <summary>
        /// Уничтожает пул. Однако необходимо убрать все ссылки на пул
        /// </summary>
        void Destroy();
    }
}