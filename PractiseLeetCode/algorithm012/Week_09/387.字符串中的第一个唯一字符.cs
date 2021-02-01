/*
 * @lc app=leetcode.cn id=387 lang=csharp
 *
 * [387] 字符串中的第一个唯一字符
 */

// @lc code=start
public class Solution {
    public int FirstUniqChar(string s) {
        // int[] arr=new int[128];
        // foreach(var ch in s)
        //     arr[ch]++;
        
        // for(int i=0; i<s.Length; i++)
        //     if(arr[s[i]]==1)
        //         return i;
        // return -1;

        var dic=new Dictionary<char, int>();
        for(int i=0; i<s.Length; i++){
            if(dic.ContainsKey(s[i]))
                dic[s[i]]++;
            else
                dic[s[i]]=1;
        }

        for(int i=0; i<s.Length; i++)
            if(dic[s[i]]==1)
                return i;
        return -1;
    }
}
// @lc code=end

