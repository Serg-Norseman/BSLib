using System;
using System.Collections.ObjectModel;

namespace BSLib.Extensions
{
	public interface IExtensionCollection<T, X> where T : IExtensibleObject<T> where X : IExtension<T>
	{
		int Count
		{
			get;
		}

		void Add(X item);
		void Clear();
		bool Contains(X item);
		bool Remove(X item);

		E Find<E>();
		Collection<E> FindAll<E>();
	}
}
