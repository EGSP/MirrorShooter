using System;
using Gasanov.Extensions.Mono;
using Mirror;

namespace Game.Net
{
    public class TelepathyAdapter : TransportAdapter
    {
        public TelepathyTransport transport;

        private void Awake()
        {
            this.ValidateComponentRef(ref transport);
            if(transport == null)
                throw new NullReferenceException();
        }

        public override void SetPort(string port)
        {
            ushort portValue;
            if (ushort.TryParse(port, out portValue))
                transport.port = portValue;
        }
    }
}