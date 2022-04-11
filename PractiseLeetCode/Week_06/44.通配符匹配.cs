/*
 * @lc app=leetcode.cn id=44 lang=csharp
 *
 * [44] 通配符匹配
 */

// @lc code=start
public class Solution {
    public bool IsMatch(string s, string p) {
        int m=s.Length, n=p.Length;
        bool[,] dp=new bool[m+1,n+1];

        dp[0,0]=true;

        //base case: s为空字符串时与p进行匹配，只要p的前j个字符均为*，才为true
        for(int i=1; i<=n; i++){
            dp[0,i]=dp[0,i-1] && p[i-1]=='*';
        }

        for(int i=1; i<=m; i++){
            for(int j=1; j<=n; j++){
                if(s[i-1]==p[j-1] || p[j-1]=='?')
                    dp[i,j]=dp[i-1,j-1];
                else if(p[j-1]=='*'){
                    //dp[i-1,j]：当前*匹配0个(空)字符, 例如ab, ab*
                    //dp[i,j-1]：当前*匹配s[j-1]字符， 例如abc, ab*
                    dp[i,j]=dp[i-1,j] || dp[i,j-1];
                }
            }
        }
        return dp[m,n];
    }
}
// @lc code=end

