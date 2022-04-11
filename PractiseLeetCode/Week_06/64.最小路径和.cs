/*
 * @lc app=leetcode.cn id=64 lang=csharp
 *
 * [64] 最小路径和
 */

// @lc code=start
public class Solution {
    public int MinPathSum(int[][] grid) {
        int m=grid.Length;
        if(m==0) return 0;
        int n=grid[0].Length;

        //dp
        int[,] dp =new int[m,n];
        dp[0,0]=grid[0][0];
        for(int i=1; i<m; i++){
            dp[i,0]=dp[i-1,0]+grid[i][0];
        }
            
        for(int j=1; j<n; j++){
            dp[0,j]=dp[0,j-1]+grid[0][j];
        }
        
        for(int i=1; i<m; i++){
            for(int j=1; j<n; j++){
                dp[i,j]=Math.Min(dp[i-1,j], dp[i,j-1])+grid[i][j];
            }
        }
        return dp[m-1,n-1];

        //juse use the grid[i][j] as DP
        // for(int i=1; i<m; i++){
        //     grid[i][0]+=grid[i-1][0];
        // }
            
        // for(int j=1; j<n; j++){
        //     grid[0][j]+=grid[0][j-1];
        // }
        
        // for(int i=1; i<m; i++){
        //     for(int j=1; j<n; j++){
        //         grid[i][j]+=Math.Min(grid[i-1][j], grid[i][j-1]);
        //     }
        // }
        // return grid[m-1][n-1];

        // for(int i=0; i<m; i++){
        //     for(int j=0; j<n; j++){
        //         if(i==0 && j==0)
        //             continue;
        //         else if(i==0)
        //             grid[i][j]=grid[i][j-1]+grid[i][j];
        //         else if(j==0)
        //             grid[i][j]=grid[i-1][j]+grid[i][j];
        //         else
        //             grid[i][j]=Math.Min(grid[i-1][j], grid[i][j-1])+grid[i][j];
        //     }
        // }
        // return grid[m-1][n-1];
    }
}
// @lc code=end

