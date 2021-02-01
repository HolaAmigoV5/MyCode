/*
 * @lc app=leetcode.cn id=205 lang=csharp
 *
 * [205] 同构字符串
 */

// @lc code=start
public class Solution {
    public bool IsIsomorphic(string s, string t) {
        //M1
        // int n = s.Length;
        // if (n == 0) return true;
        // var dic = new Dictionary<char, char>();

        // for (int i = 0; i < n; i++)
        // {
        //     var ch1 = s[i];
        //     var ch2 = t[i];

        //     if (dic.ContainsKey(ch1))
        //     {
        //         if (dic[ch1] != ch2)
        //             return false;
        //     }
        //     else if (dic.ContainsValue(ch2))
        //         return false;
        //     else
        //         dic[ch1] = ch2;
        // }
        // return true;

        //M2
        int n=s.Length;
        int[] mapS=new int[128];
        int[] mapT=new int[128];
        for(int i=0; i<n; i++){
            var c1=s[i];
            var c2=t[i];
            if(mapS[c1]!=mapT[c2])
                return false;
            else{
                if(mapS[c1]==0){
                    mapS[c1]=i+1;
                    mapT[c2]=i+1;
                }
            }
        }
        return true;

        //M3
        // int[] m=new int[512];
        // for(int i=0; i<s.Length; i++){
        //     if(m[s[i]]!=m[t[i]+256])
        //         return false;
        //     m[s[i]]=m[t[i]+256]=i+1;
        // }
        // return true;
    }
}
// @lc code=end

