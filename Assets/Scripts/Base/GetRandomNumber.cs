using System.Collections.Generic;

public static class GetRandomNumber
{
    /// <summary>
    /// ���� ���� ��� ����
    /// </summary>
    /// <param name="StartNum">���� ��ȣ</param>
    /// <param name="EndNum">�� ��ȣ (�迭, ����Ʈ ������)</param>
    /// <param name="ResultAmounts">�ް� ���� ������ ����</param>
    /// <param name="RemoveNums">������ ���ڵ�</param>
    /// <returns></returns>
    public static List<int> GetRanNums(int StartNum, int EndNum, int ResultAmounts,
        int[] RemoveNums = null)
    {
        // �ߺ� ���� ����
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
    /// ���� ���� 1�� ����
    /// </summary>
    /// <param name="StartNum">���� ��ȣ</param>
    /// <param name="EndNum">�� ��ȣ</param>
    /// <param name="RemoveNums">������ ���ڵ�</param>
    /// <returns></returns>
    public static int GetRanNum(int StartNum, int EndNum, int[] RemoveNums = null)
    {
        // �ߺ� ���� ����
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
    /// ���� ���� 1�� ����
    /// </summary>
    /// <param name="StartNum">���� ��ȣ</param>
    /// <param name="EndNum">���� ��ȣ</param>
    /// <param name="RemoveNum">������ ����</param>
    /// <returns></returns>
    public static int GetRanNum(int StartNum, int EndNum, int RemoveNum)
    {
        // �ߺ� ���� ����
        List<int> Numbers = new List<int>();
        for (int i = StartNum; i < EndNum; ++i)
        {
            Numbers.Add(i);
        }

        Numbers.Remove(RemoveNum);
       
        return UnityEngine.Random.Range(0, Numbers.Count);
    }
}
