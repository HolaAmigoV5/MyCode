/*
 * @lc app=leetcode.cn id=14 lang=csharp
 *
 * [14] 最长公共前缀
 */

// @lc code=start
public class Solution {
    public string LongestCommonPrefix(string[] strs) {
        if(strs.Length==0) return "";
        string ans=strs[0];

        // for(int i=1; i<strs.Length; i++){
        //     int j=0;
        //     for(; j<ans.Length && j<strs[i].Length; j++){
        //         if(ans[j]!=strs[i][j])
        //             break;
        //     }
        //     ans=ans.Substring(0,j);
        //     if(ans.Equals(""))
        //         return ans;
        // }
        // return ans;

        for(int i=1; i<strs.Length; i++){
             while(strs[i].IndexOf(ans)!=0)
                ans=ans.Substring(0, ans.Length-1);
        }
        return ans;
    }
}
// @lc code=end

