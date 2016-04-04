using System;
using System.Collections.ObjectModel;

namespace BSLib.Extensions
{
	public sealed class ExtensionCollection<T, X> : IExtensionCollection<T, X> where T : IExtensibleObject<T> where X : IExtension<T>
	{
		private T _owner;
		private X[] _items;
		private int _size;
		private object _syncRoot;

		public ExtensionCollection(T owner)
		{
			if (owner == null)
				throw new ArgumentNullException("owner");

			this._owner = owner;
			this._items = new X[0];
			this._size = 0;
			this._syncRoot = new Object();
		}

		public int Count
		{
			get {
				lock (this._syncRoot)
				{
					return this._size;
				}
			}
		}

		public void Add(X item)
		{
			if (item == null)
				throw new ArgumentNullException("item");

			lock (this._syncRoot)
			{
				X[] array = new X[this._size + 1];
				if (this._size > 0)
				{
					Array.Copy(this._items, 0, array, 0, this._size);
				}
				this._items = array;
				this._items[this._size++] = item;

				item.Attach(this._owner);
			}
		}

		public bool Remove(X item)
		{
			lock (this._syncRoot)
			{
				int index = this.IndexOf(item);
				if (index < 0)
					return false;

				this._items[index].Detach(this._owner);

				this._size--;
				if (index < this._size)
				{
					Array.Copy(this._items, index + 1, this._items, index, this._size - index);
				}
				this._items[this._size] = default(X);

				return true;
			}
		}

		private int IndexOf(X item)
		{
			for (int i = 0; i < this._size; i++)
			{
				if (Equals(this._items[i], item))
				{
					return i;
				}
			}
			return -1;
		}

		public bool Contains(X item)
		{
			lock (this._syncRoot)
			{
				return (this.IndexOf(item) >= 0);
			}
		}

		public void Clear()
		{
			lock (this._syncRoot)
			{
				for (int i = 0; i < this._size; i++)
				{
					this._items[i].Detach(this._owner);
				}

				if (this._size > 0)
				{
					Array.Clear(this._items, 0, this._size);
					this._size = 0;
				}
			}
		}


		public E Find<E>()
		{
			lock (this._syncRoot)
			{
				for (int i = 0; i < this._size; i++)
				{
					IExtension<T> item = _items[i];
					if (item is E)
						return (E)item;
				}
			}

			return default(E);
		}

		public Collection<E> FindAll<E>()
		{
			Collection<E> result = new Collection<E>();

			lock (this._syncRoot)
			{
				for (int i = 0; i < this._size; i++)
				{
					IExtension<T> item = _items[i];
					if (item is E)
						result.Add((E)item);
				}
			}

			return result;
		}
	}
}