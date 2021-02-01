/*
 * @lc app=leetcode.cn id=309 lang=csharp
 *
 * [309] 最佳买卖股票时机含冷冻期
 */

// @lc code=start
public class Solution {
    public int MaxProfit(int[] prices) {
        int len=prices.Length;
        if(len<2) return 0;

        //dp
        int[,] dp=new int[len, 2];
        //base case
        dp[0,0]=0; 
        dp[0,1]=-prices[0];

        for(int i=1; i<len; i++){
            dp[i,0]=Math.Max(dp[i-1,0], dp[i-1,1]+prices[i]);
            dp[i,1]=Math.Max(dp[i-1,1], (i>=2?dp[i-2,0]:0)-prices[i]); //buy stocks one day later
        }
        return dp[len-1,0];

        // int dp_i_0=0, dp_i_1=int.MinValue;
        // int dp_pre_0=0; //代表dp[i-2,0];

        // for(int i=1; i<len; i++){
        //     int temp=dp_i_0;
        //     dp_i_0=Math.Max(dp_i_0,dp_i_1+prices[i]);
        //     dp_i_1=Math.Max(dp_i_1, dp_pre_0-prices[i]);
        //     dp_pre_0=temp;
        // }
        // return dp_i_0;
    }
}
// @lc code=end

