/*
 * @lc app=leetcode.cn id=62 lang=csharp
 *
 * [62] 不同路径
 */

// @lc code=start
public class Solution {
    public int UniquePaths(int m, int n) {
        //O(mn), O(mn)
        // int[,] dp=new int[m,n];
        // for(int i=0;i<n; i++)
        //     dp[0,i]=1;
        // for(int j=0;j<m; j++)
        //     dp[j,0]=1;
        // for(int i=1; i<m; i++){
        //     for(int j=1; j<n;j++){
        //         dp[i,j]=dp[i-1,j]+dp[i,j-1];
        //     }
        // }
        // return dp[m-1,n-1];

        //Dp
        if(m*n==0) return 0;
        int[,] dp=new int[m, n];

        for(int i=0; i<m; i++){
            for(int j=0; j<n; j++){
                dp[i,j]=(i==0 || j==0)?1:dp[i-1,j]+dp[i,j-1];
            }
        }
        return dp[m-1, n-1];

        //dynamic programming2：O(mn), O(n)
        // int[] dp=new int[n];
        // dp[0]=1;

        // for(int i=0; i<m; i++){
        //     for(int j=1; j<n; j++)
        //         dp[j]+=dp[j-1];
        // }

        // return dp[n-1];
    }
}
// @lc code=end

