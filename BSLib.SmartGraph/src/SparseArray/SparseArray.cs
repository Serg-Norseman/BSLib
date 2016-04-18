using System;
using System.Collections.Generic;

namespace BSLib.SparseArray
{
	/// <summary>
	/// Description of SparseArray.
	/// </summary>
	public class SparseArray<T> where T : class
	{
		private SortedList<ulong, T> fArray;
		private uint fMinCol;
		private uint fMaxCol;
		private uint fMinRow;
		private uint fMaxRow;

		public SparseArray()
		{
			this.fArray = new SortedList<ulong, T>();
			this.ResetBounds();
		}

		public void Clear()
		{
			this.fArray.Clear();
			this.ResetBounds();
		}

		public bool IsEmpty()
		{
			return (this.fArray.Count <= 0);
		}

		public ulong GetKey(uint row, uint col)
		{
			ulong key = (row << 31 | col);
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

		public void RemoveItem(uint row, uint col)
		{
			ulong key = this.GetKey(row, col);
			this.fArray.Remove(key);
		}

		private void ResetBounds()
		{
			this.fMinRow = uint.MaxValue;
			this.fMaxRow = uint.MinValue;
			this.fMinCol = uint.MaxValue;
			this.fMaxCol = uint.MinValue;
		}

		private void AdjustBounds(uint row, uint col)
		{
			if (this.fMinRow > row) this.fMinRow = row;
			if (this.fMaxRow < row) this.fMaxRow = row;
			if (this.fMinCol > col) this.fMinCol = col;
			if (this.fMaxCol < col) this.fMaxCol = col;
		}

		/// <summary>
		/// This function provides fast access to a values of the matrix's row.
		/// </summary>
		/// <param name="row"></param>
		/// <returns></returns>
		public IEnumerable<T> GetRow(uint row)
		{
			int first_index = this.fArray.IndexOfKey(this.GetKey(row, this.fMinCol));
			int last_index = this.fArray.IndexOfKey(this.GetKey(row, this.fMaxCol));

			IList<T> values = this.fArray.Values;
			for (int col = first_index; col <= last_index; col++)
			{
				yield return values[col];
			}
			yield break;
		}

		/// <summary>
		/// This function provides slow access to a values of the matrix's row.
		/// </summary>
		/// <param name="row"></param>
		/// <returns></returns>
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

		public void RemoveItemsByRow(uint row)
		{
			for (uint col = this.fMinCol; col <= this.fMaxCol; col++)
			{
				this.SetItem(row, col, null);
			}
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

		public void RemoveItemsByCol(uint col)
		{
			for (uint row = this.fMinRow; row <= this.fMaxRow; row++)
			{
				this.SetItem(row, col, null);
			}
		}

		public IEnumerable<T> GetAllItems()
		{
			return this.fArray.Values;
		}
	}
}
