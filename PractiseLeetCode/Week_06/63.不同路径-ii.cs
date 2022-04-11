/*
 * @lc app=leetcode.cn id=63 lang=csharp
 *
 * [63] 不同路径 II
 */

// @lc code=start
public class Solution {
    public int UniquePathsWithObstacles(int[][] obstacleGrid) {
        if(obstacleGrid==null || obstacleGrid.Length==0)
            return 0;
        // int m=obstacleGrid.Length;
        // int n=obstacleGrid[0].Length;
        // int[,] dp=new int[m,n];
        // for(int i=0; i<m && obstacleGrid[i][0]==0;i++)
        //     dp[i,0]=1;
        // for(int j=0; j<n && obstacleGrid[0][j]==0; j++)
        //     dp[0,j]=1;
        
        // for(int i=1; i<m; i++){
        //     for(int j=1; j<n; j++){
        //         if(obstacleGrid[i][j]==0)
        //             dp[i,j]=dp[i-1,j]+dp[i,j-1];
        //     }
        // }
        // return dp[m-1, n-1];

        //M2:dp, compression
        int width=obstacleGrid[0].Length;
        int[] dp=new int[width];
        dp[0]=1;
        foreach(int[] row in obstacleGrid){
            for(int j=0; j<width; j++){
                if(row[j]==1)
                    dp[j]=0;
                else if(j>0)
                    dp[j]+=dp[j-1];
            }
        }
        return dp[width-1];
    }
}
// @lc code=end

