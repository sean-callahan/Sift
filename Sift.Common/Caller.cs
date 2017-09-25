using System;

namespace Sift.Common
{
    public class Caller
    {
        public string Id { get; }
        public string Number { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string Comment { get; set; }
        public DateTime Created { get; set; }

        public Caller(string id)
        {
            Id = id;
        }
    }
}
