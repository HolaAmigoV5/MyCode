/*
 * @lc app=leetcode.cn id=115 lang=csharp
 *
 * [115] 不同的子序列
 */

// @lc code=start
public class Solution {
    public int NumDistinct(string s, string t) {
        int s_len=s.Length, t_len=t.Length;

        //dp[i,j] 代表t前i字符串可以由s j字符串组成最多个数.
        int[,] dp=new int[t_len+1, s_len+1];
        for(int j=0; j<=s_len; j++)
            dp[0,j]=1;
        
        for(int i=1; i<=t_len; i++){
            for(int j=1; j<=s_len; j++){
                if(t[i-1]==s[j-1])
                    dp[i,j]=dp[i-1, j-1]+dp[i,j-1];
                else
                    dp[i,j]=dp[i,j-1];
            }
        }
        return dp[t_len,s_len];
    }
}
// @lc code=end

