using System;

namespace Game.Net.Exceptions
{
    public class NotServerException : Exception
    {
        public NotServerException() : base("The operation is available only to the server")
        {
            
        }
    }
}