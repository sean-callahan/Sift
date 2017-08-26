using System;

namespace Sift.Common
{
    public interface ILink : IDisposable
    {
        Guid Id { get; }

        Caller Originator { get; }

        IDestination Destination { get; }

        void Start();
    }

    public interface IProviderLink<T> : ILink where T : IVoipProvider
    {
        T Provider { get; }
    }
}
