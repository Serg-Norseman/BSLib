using System;

namespace BSLib.Extensions
{
    public interface IExtension<T> where T : IExtensibleObject<T>
    {
        void Attach(T owner);
        void Detach(T owner);
    }
}
