using Game.Entities.Modules;

namespace Game.Entities.States
{
    public abstract class LogicState<TState, TModule>
        where TState : LogicState<TState, TModule>
        where TModule : LogicModule<TState, TModule>
    {
        /// <summary>
        /// Модуль которому принадлежит состояние.
        /// </summary>
        protected readonly TModule Module;
        
        protected LogicState(TModule module)
        {
            Module = module;
        }
        
        /// <summary>
        /// Следующее состояние. Может быть пустым, тогда состояние само определяет следующее.
        /// </summary>
        protected TState NextState { get; private set; }

        public void SetNext(TState nextState)
        {
            NextState = nextState;
        }

        public abstract TState ReturnThis();

        public virtual TState UpdateOnServer(float deltaTime)
        {
            return ReturnThis();
        }
        
        public virtual TState FixedUpdateOnServer(float deltaTime)
        {
            return ReturnThis();
        }

    }
}