using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace BSLib.Extensions
{
	public sealed class ExtensionCollection<T, X> : IExtensionCollection<T, X> where T : IExtensibleObject<T> where X : IExtension<T>
	{
		private T owner;
		private List<X> items;
		private object sync;

		public ExtensionCollection(T owner)
		{
			if (owner == null)
				throw new ArgumentNullException("owner");

			this.owner = owner;
			this.items = new List<X>();
			this.sync = new Object();
		}

		public int Count
		{
			get {
				lock (this.sync)
				{
					return this.items.Count;
				}
			}
		}

		public void Add(X item)
		{
			if (item == null)
				throw new ArgumentNullException("item");

			lock (this.sync)
			{
				int index = this.items.Count;

				item.Attach(this.owner);
				this.items.Insert(index, item);
			}
		}

		public bool Remove(X item)
		{
			lock (this.sync)
			{
				int index = this.items.IndexOf(item);
				if (index < 0)
					return false;

				this.items[index].Detach(this.owner);
				this.items.RemoveAt(index);

				return true;
			}
		}

		public void Clear()
		{
			X[] array;

			lock (this.sync)
			{
				array = new X[this.Count];
				this.items.CopyTo(array, 0);

				this.items.Clear();

				foreach (X extension in array)
				{
					extension.Detach(this.owner);
				}
			}
		}

		public bool Contains(X item)
		{
			lock (this.sync)
			{
				return this.items.Contains(item);
			}
		}


		public E Find<E>()
		{
			List<X> itemsList = this.items;

			lock (this.sync)
			{
				int count = itemsList.Count;
				for (int i = 0; i < count; i++)
				{
					IExtension<T> item = itemsList[i];
					if (item is E)
						return (E)item;
				}
			}

			return default(E);
		}

		public Collection<E> FindAll<E>()
		{
			Collection<E> result = new Collection<E>();
			List<X> itemsList = this.items;

			lock (this.sync)
			{
				int count = itemsList.Count;
				for (int i = 0; i < count; i++)
				{
					IExtension<T> item = itemsList[i];
					if (item is E)
						result.Add((E)item);
				}
			}

			return result;
		}
	}
}
