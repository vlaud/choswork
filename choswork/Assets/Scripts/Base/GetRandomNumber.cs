using System.Collections.Generic;

public static class GetRandomNumber
{
    /// <summary>
    /// 랜덤 숫자 목록 생성
    /// </summary>
    /// <param name="StartNum">시작 번호</param>
    /// <param name="EndNum">끝 번호 (배열, 리스트 사이즈)</param>
    /// <param name="ResultAmounts">받고 싶은 숫자의 개수</param>
    /// <param name="RemoveNums">제외할 숫자들</param>
    /// <returns></returns>
    public static List<int> GetRanNums(int StartNum, int EndNum, int ResultAmounts,
        int[] RemoveNums = null)
    {
        // 중복 숫자 방지
        List<int> Numbers = new List<int>();
        for (int i = StartNum; i < EndNum; ++i)
        {
            Numbers.Add(i);
        }

        if (RemoveNums != null)
        {
            for (int i = 0; i < RemoveNums.Length; ++i)
            {
                Numbers.Remove(RemoveNums[i]);
            }
        }

        List<int> LottoNumber = new List<int>();
        for (int i = 0; i < ResultAmounts; ++i)
        {
            int n = UnityEngine.Random.Range(0, Numbers.Count);
            LottoNumber.Add(Numbers[n]);
            Numbers.RemoveAt(n);
        }
        return LottoNumber;
    }

    /// <summary>
    /// 랜덤 숫자 1개 생성
    /// </summary>
    /// <param name="StartNum">시작 번호</param>
    /// <param name="EndNum">끝 번호</param>
    /// <param name="RemoveNums">제외할 숫자들</param>
    /// <returns></returns>
    public static int GetRanNum(int StartNum, int EndNum, int[] RemoveNums = null)
    {
        // 중복 숫자 방지
        List<int> Numbers = new List<int>();
        for (int i = StartNum; i < EndNum; ++i)
        {
            Numbers.Add(i);
        }

        if (RemoveNums != null)
        {
            for (int i = 0; i < RemoveNums.Length; ++i)
            {
                Numbers.Remove(RemoveNums[i]);
            }
        }

        return UnityEngine.Random.Range(0, Numbers.Count);
    }

    /// <summary>
    /// 랜덤 숫자 1개 생성
    /// </summary>
    /// <param name="StartNum">시작 번호</param>
    /// <param name="EndNum">시작 번호</param>
    /// <param name="RemoveNum">제외할 숫자</param>
    /// <returns></returns>
    public static int GetRanNum(int StartNum, int EndNum, int RemoveNum)
    {
        // 중복 숫자 방지
        List<int> Numbers = new List<int>();
        for (int i = StartNum; i < EndNum; ++i)
        {
            Numbers.Add(i);
        }

        Numbers.Remove(RemoveNum);
       
        return UnityEngine.Random.Range(0, Numbers.Count);
    }
}
