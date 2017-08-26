using System;

namespace Sift.Common
{
    public class Destination : IDestination
    {
        public string Id { get; set; }

        public string EndPoint { get; }

        public Destination(string endpoint)
        {
            EndPoint = endpoint;
            Id = string.Empty;
        }
    }
}
