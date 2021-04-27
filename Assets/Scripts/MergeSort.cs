using System;
using System.Collections.Generic;
using System.Linq;

namespace PE.Utils
{
	public class MergeSort
	{
		public static IEnumerable<T> Sort<T>(IEnumerable<T> table, Comparison<T> compare)
		{
			// 比較対象がいないのでそのまま
			int tableCount = table.Count();
			if (tableCount <= 1)
			{
				return table;
			}

			// 配列を左右に分割
			int compareCount = tableCount / 2;
			var leftTable = table.Take(compareCount).ToArray();
			var rightTable = table.Skip(compareCount).ToArray();

			// 左右分割分をソートしてマージ
			return Marge(
				Sort(leftTable, compare),
				Sort(rightTable, compare),
				compare
			);
		}

		static IEnumerable<T> Marge<T>(
			IEnumerable<T> leftTable,
			IEnumerable<T> rightTable,
			Comparison<T> compare
		)
		{
			var leftTargets = leftTable.GetEnumerator();
			var rightTargets = rightTable.GetEnumerator();
			var isNextLeft = leftTargets.MoveNext();
			var isNextRight = rightTargets.MoveNext();

			// 比較並び替え
			while( isNextLeft && isNextRight )
			{
				T leftParam = leftTargets.Current;
				T rightParam = rightTargets.Current;

				if (compare( leftParam, rightParam ) < 0)
				{
					yield return leftParam;
					isNextLeft = leftTargets.MoveNext();
				}
				else
				{
					yield return rightParam;
					isNextRight = rightTargets.MoveNext();
				}
			}

			// 配列をインクリメント
			while (isNextLeft)
			{
				yield return leftTargets.Current;
				isNextLeft = leftTargets.MoveNext();
			}
			while(isNextRight)
			{
				yield return rightTargets.Current;
				isNextRight = rightTargets.MoveNext();
			}
		}
	}
}