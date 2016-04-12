using System;
using System.Collections.Generic;

namespace BSLib.SparseArray
{
	/// <summary>
	/// Description of SparseArray.
	/// </summary>
	public class SparseArray<T>
	{
		private SortedList<ulong, T> fArray;
		private uint fMinCol;
		private uint fMaxCol;
		private uint fMinRow;
		private uint fMaxRow;

		public SparseArray()
		{
			this.fArray = new SortedList<ulong, T>();
		}

		public void Clear()
		{
			this.fArray.Clear();
			this.fMinRow = 0;
			this.fMaxRow = 0;
			this.fMinCol = 0;
			this.fMaxCol = 0;
		}

		public ulong GetKey(uint row, uint col)
		{
			ulong key = row << 32 | col;
			return key;
		}

		public T GetItem(uint row, uint col)
		{
			ulong key = this.GetKey(row, col);

			T result;
			if (this.fArray.TryGetValue(key, out result)) {
				return result;
			}

			return default(T);
		}

		public void SetItem(uint row, uint col, T value)
		{
			ulong key = this.GetKey(row, col);

			this.fArray[key] = value;

			this.AdjustBounds(row, col);
		}

		private void AdjustBounds(uint row, uint col)
		{
			if (this.fMinRow > row) this.fMinRow = row;
			if (this.fMaxRow < row) this.fMaxRow = row;
			if (this.fMinCol > col) this.fMinCol = col;
			if (this.fMaxCol < col) this.fMaxCol = col;
		}

		public IEnumerable<T> GetItemsByRow(uint row)
		{
			for (uint col = this.fMinCol; col <= this.fMaxCol; col++)
			{
				T item = this.GetItem(row, col);
				if (item != null) {
					yield return item;
				}
			}
			yield break;
		}

		public IEnumerable<T> GetItemsByCol(uint col)
		{
			for (uint row = this.fMinRow; row <= this.fMaxRow; row++)
			{
				T item = this.GetItem(row, col);
				if (item != null) {
					yield return item;
				}
			}
			yield break;
		}

		public IEnumerable<T> GetAllItems()
		{
			return this.fArray.Values;
		}
	}
}
