using System;

namespace Sift.Common
{
    public abstract class Group
    {
        public Guid Id { get; protected set; } 

        public IVoipProvider Provider { get; }
        public GroupType Type { get; }

        public Group(IVoipProvider provider, GroupType type)
        {
            Provider = provider;
            Type = type;
        }

        public abstract void Add(Caller c);

        public abstract void Remove(Caller c);
    }
}
