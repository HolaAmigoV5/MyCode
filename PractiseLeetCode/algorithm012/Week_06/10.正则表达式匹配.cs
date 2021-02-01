/*
 * @lc app=leetcode.cn id=10 lang=csharp
 *
 * [10] 正则表达式匹配
 */

// @lc code=start
public class Solution {
    public bool IsMatch(string s, string p) {
        if(s==null || p==null)
            return false;
        
        int m=s.Length, n=p.Length;

        //dp[i,j] 表示s的前i个是否能被p的前j个匹配
        bool[,] dp=new bool[m+1,n+1];

        //base case
        // 1. dp[0,0] = true, since empty string matches empty pattern
        dp[0,0]=true;
        // 2. M[i][0] = false(default value) since empty pattern cannot match non-empty string
		// 3. M[0][j]: what pattern matches empty string ""? 
        //It should be #*#*#*#*..., or (#*)* if allow me to represent regex using regex :P, 
        //偶数长度的p匹配规则。奇数长度无法匹配默认为false
         for(int j=2; j<=n; j+=2){
             //dp[0,j]=dp[0,j-2] && p[j-1]=='*';

             if(p[j-1]=='*')
                dp[0,j]=dp[0,j-2];
         }
            

        for(int i=1; i<=m; i++){
            for(int j=1; j<=n; j++){
                if(s[i-1]==p[j-1] || p[j-1]=='.')
                    dp[i,j]=dp[i-1,j-1];
                else if(p[j-1]=='*' && j>1){
                    if(p[j-2]!='.' && p[j-2]!=s[i-1])
                        dp[i,j]=dp[i,j-2];
                    else
                        dp[i,j]=(dp[i,j-2] || dp[i-1,j-2] || dp[i-1,j]);
                }
            }
        }
        return dp[m,n];
    }
}
// @lc code=end

