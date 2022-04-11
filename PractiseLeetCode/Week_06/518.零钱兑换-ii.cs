/*
 * @lc app=leetcode.cn id=518 lang=csharp
 *
 * [518] 零钱兑换 II
 */

// @lc code=start
public class Solution {
    public int Change(int amount, int[] coins) {
        //Dp1:
        int m=coins.Length;
    
        int[,] dp=new int[m+1,amount+1];
        dp[0,0]=1;

        for(int i=1; i<=m; i++){
            dp[i,0]=1;
            for(int j=1; j<=amount; j++){
                dp[i,j]=dp[i-1,j]+(j>=coins[i-1]?dp[i,j-coins[i-1]]:0);
            }
        }
        return dp[m,amount];

        //Dp2: O(mn), O(m)
        // int[] dp=new int[amount+1];
        // dp[0]=1;
        // foreach(var coin in coins){
        //     for(int i=1; i<=amount; i++){
        //         if(i<coin)
        //             continue;
        //         dp[i]+=dp[i-coin];
        //     }
        // }
        // return dp[amount];
    }
}
// @lc code=end

