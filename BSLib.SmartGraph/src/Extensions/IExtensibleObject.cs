using System;

namespace BSLib.Extensions
{
	public interface IExtensibleObject<T> where T : IExtensibleObject<T>
	{
		ExtensionCollection<T, IExtension<T>> Extensions { get; }
	}
}
