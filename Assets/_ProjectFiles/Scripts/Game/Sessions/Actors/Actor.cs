using Mirror;

namespace Game.Sessions.Actors
{
    /// <summary>
    /// Класс отвечающий за различную логику игрового процесса в сессии на стороне сервера.
    /// Отличие от Observer в том, что Actor работает с событиями.
    /// </summary>
    public abstract class Actor
    {
        protected readonly ServerSession Session;
        
        protected Actor(ServerSession session)
        {
            Session = session;
        }

        public abstract void Dispose();
    }
}