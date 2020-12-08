using Egsp.Core.Ui;

namespace Egsp.Core.Ui
{
    public interface IPresenter<TView, TModel> : IPresenter where TView : IView
    {
        TView View { get; }
        TModel Model { get; }
        
        string Key { get;}
    }

    public interface IPresenter
    {
        /// <summary>
        /// Презентер говорит о своем существовании.
        /// </summary>
        void Share();
        
        /// <summary>
        /// Передается ключ для активации из вне и аргументы.
        /// </summary>
        bool Response(string key, object arg);

        /// <summary>
        /// Вызывается после создания.
        /// </summary>
        void OnAwake();
    }
}