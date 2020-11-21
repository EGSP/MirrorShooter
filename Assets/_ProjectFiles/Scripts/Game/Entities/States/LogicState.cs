using Game.Entities.Modules;

namespace Game.Entities.States
{
    public abstract class LogicState
    {
        protected LogicState()
        {
            CachedId = ID;
        }
        
        /// <summary>
        /// Идентификатор состояния.
        /// </summary>
        public string ID => this.GetType().Name;
        public string CachedId { get; private set; }
    }
    
    public abstract class LogicState<TState, TModule> : LogicState
        where TState : LogicState<TState, TModule>
        where TModule : LogicModule<TState, TModule>
    {
        /// <summary>
        /// Модуль которому принадлежит состояние.
        /// </summary>
        protected readonly TModule Module;
        
        protected LogicState(TModule module) : base()
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