using System;
using Egsp.Extensions.Mono;
using Mirror;

namespace Game.Net
{
    public class IgnoranceAdapter : TransportAdapter
    {
        public IgnoranceThreaded transport;

        private void Awake()
        {
            this.ValidateComponentRef(ref transport);
            if(transport == null)
                throw new NullReferenceException();
        }

        public override void SetPort(string port)
        {
            int portValue;
            if (int.TryParse(port, out portValue))
                transport.CommunicationPort = portValue;
        }
    }
}