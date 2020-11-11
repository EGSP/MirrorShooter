using System;

namespace Game.Net.Exceptions
{
    public class InvalidUserConnection : Exception
    {
        public InvalidUserConnection() : base()
        {
        }

        public InvalidUserConnection(UserConnection uc)
            : base($"User connection is null : {uc.User.id} | {uc.User.name}")
        {
            
        }
    }
}