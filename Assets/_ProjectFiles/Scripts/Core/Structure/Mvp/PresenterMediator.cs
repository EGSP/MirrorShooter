using System.Collections.Generic;

namespace Gasanov.Core.Mvp
{
    public static class PresenterMediator
    {
        static PresenterMediator()
        {
            _presenters = new List<IPresenter>();
        }
        
        /// <summary>
        /// Все существующие презентеры.
        /// </summary>
        private static List<IPresenter> _presenters;

        /// <summary>
        /// Добавление нового презентера.
        /// </summary>
        /// <param name="presenter"></param>
        public static void Register(IPresenter presenter)
        {
            if (_presenters.Contains(presenter))
                return;

            presenter.OnDispose += () => _presenters.Remove(presenter);
            _presenters.Add(presenter);
        }

        public static void Unregister(IPresenter presenter)
        {
            _presenters.Remove(presenter);
        }

        /// <summary>
        /// Отправляет сообщение всем остальным презентерам до первого положительного ответа.
        /// </summary>
        public static void Request(IPresenter sender,string message, object arg)
        {
            for (var i = 0; i < _presenters.Count; i++)
            {
                var presenter = _presenters[i];

                if (presenter != sender)
                {
                    // Если презентер ответил на сообщение
                    if(presenter.Response(message, arg))
                        break;
                }
            }
        }
    }
}