using System;

namespace Sift.Common
{
    public interface IGroup
    {
        Guid Id { get; } 

        IVoipProvider Provider { get; }
        GroupType Type { get; }
        
        void Add(Caller c);

        void Remove(Caller c);

        bool Contains(Caller c);
    }
}
