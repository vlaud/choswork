using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GetRandomNumber
{
    public static List<int> GetRanNum(int StartNum, int EndNum, int ResultAmounts, 
        bool RemoveStartnNum, int[] RemoveNums = null)
    {
        // 중복 숫자 방지
        List<int> Numbers = new List<int>();
        for (int i = StartNum; i < EndNum; ++i)
        {
            Numbers.Add(i);
        }
        if(RemoveStartnNum && RemoveNums != null)
        {
            for(int i = 0; i < RemoveNums.Length; ++i)
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
}
