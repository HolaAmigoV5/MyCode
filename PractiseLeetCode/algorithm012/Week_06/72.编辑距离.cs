/*
 * @lc app=leetcode.cn id=72 lang=csharp
 *
 * [72] 编辑距离
 */

// @lc code=start
public class Solution {
    public int MinDistance(string word1, string word2) {
        //O(mn), O(mn)
        int m =word1.Length, n=word2.Length;
        if(m*n==0) return m+n;
        //dp[i][j] 代表 word1到 i位置转换成 word2到j位置需要最少步数
        int[,] dp=new int[m+1,n+1];

        //base case
        for(int i=1; i<=m; i++)
            dp[i,0]=i;
        for(int j=1; j<=n; j++)
            dp[0,j]=j;
        
        //from bottom to up
        for(int i=1; i<=m; i++){
            for(int j=1; j<=n; j++){
                if(word1[i-1]==word2[j-1])
                    dp[i,j]=dp[i-1,j-1];
                else{
                    dp[i,j]=Math.Min(Math.Min(dp[i-1,j], dp[i,j-1]), dp[i-1,j-1])+1;
                }
            }
        }
        return dp[m,n];
    }
}
// @lc code=end

