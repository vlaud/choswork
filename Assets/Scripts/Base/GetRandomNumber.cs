using System;
using System.Collections.Generic;
using System.Linq;

public static class GetRandomNumber
{
    /// <summary>
    /// 난수 리스트 생성
    /// </summary>
    /// <param name="StartNum">시작</param>
    /// <param name="EndNum">끝</param>
    /// <param name="ResultAmounts">숫자 개수</param>
    /// <param name="RemoveNums">지울 숫자들</param>
    /// <param name="seed">시드</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static List<int> GetRanNums(int StartNum, int EndNum, int ResultAmounts, int[] RemoveNums = null, int? seed = null)
    {
        // 시작 숫자가 끝 숫자보다 크거나 같으면 throw
        if (StartNum >= EndNum)
            throw new ArgumentException("StartNum은 EndNum보다 작아야 함.");

        // StartNum ~ EndNum-1까지 리스트 생성
        var range = Enumerable.Range(StartNum, EndNum - StartNum).ToList();

        // 지울 숫자들이 있으면 지움
        if (RemoveNums != null)
            foreach (var num in RemoveNums)
                range.Remove(num);

        // 반환해야 할 숫자 개수가 리스트보다 크면 throw
        if (ResultAmounts > range.Count)
            throw new ArgumentException($"ResultAmounts({ResultAmounts}) > available range size({range.Count}).");

        // 시드값이 존재하면 시드 위주로, 아니면 시스템 값으로
        var rng = seed.HasValue ? new Random(seed.Value) : new Random();

        // Fisher-Yates 셔플
        for (int i = range.Count - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (range[i], range[j]) = (range[j], range[i]);
        }

        return range.GetRange(0, ResultAmounts);
    }

    /// <summary>
    /// 단일 난수 생성
    /// </summary>
    /// <param name="StartNum"></param>
    /// <param name="EndNum"></param>
    /// <param name="RemoveNums"></param>
    /// <param name="seed"></param>
    /// <returns></returns>
    public static int GetRanNum(int StartNum, int EndNum, int[] RemoveNums = null, int? seed = null)
    {
        return GetRanNums(StartNum, EndNum, 1, RemoveNums, seed)[0];
    }
}
