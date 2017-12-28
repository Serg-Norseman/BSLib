using System;

namespace BSLib.Extensions
{
	public interface IExtensibleObject<T> where T : IExtensibleObject<T>
	{
		IExtensionCollection<T, IExtension<T>> Extensions { get; }
	}
}
