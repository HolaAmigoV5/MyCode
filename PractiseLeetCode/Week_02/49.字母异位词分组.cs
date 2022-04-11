/*
 * @lc app=leetcode.cn id=49 lang=csharp
 *
 * [49] 字母异位词分组
 */

// @lc code=start
public class Solution {
    public IList<IList<string>> GroupAnagrams(string[] strs) {
        //M1: Violence. O(nk), O(n)
        /* var result = new List<IList<string>>();
        bool[] used = new bool[strs.Length];
        for (int i = 0; i < strs.Length; i++)
        {
            if (!used[i])
            {
                var tem = new List<string>() { strs[i] };
                for (int j = i + 1; j < strs.Length; j++)
                {
                    if (!used[j] && IsAnagrams(strs[i], strs[j]))
                    {
                        used[j]=true;
                        tem.Add(strs[j]);
                    }
                }
                result.Add(tem);
            }
        }

        return result; */

        //M2:use Dic. O(nklogk) O(n+k)
        var dic=new Dictionary<string,IList<string>>();
        foreach(string str in strs){
            //string hashkey=GetHashKey(str);
            char[] temp=str.ToCharArray();
            Array.Sort(temp);
            string hashkey= new string(temp);
            if(dic.ContainsKey(hashkey))
                dic[hashkey].Add(str);
            else
                dic[hashkey]=new List<string>(){str};
        }
        return dic.Values.ToList();
    }

    public string GetHashKey(string str){
        int[] arr=new int[26];
        foreach(char ch in str){
            arr[ch-'a']++;
        }
        // for(int i=0; i<str.Length; i++)
        //     arr[str[i]-'a']++;
        return string.Join(',',arr);
    }

    public bool IsAnagrams(string s1, string s2){
        if (s1 == null || s1 == null)
            return false;
        if (s1.Length != s2.Length) return false;
        int[] arr = new int[26];
        for (int i = 0; i < s1.Length; i++)
        {
            arr[s1[i] - 'a']++;
            arr[s2[i] - 'a']--;
        }
        return !arr.Any(item => item != 0);
    }
}
// @lc code=end

