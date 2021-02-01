/*
 * @lc app=leetcode.cn id=714 lang=csharp
 *
 * [714] 买卖股票的最佳时机含手续费
 */

// @lc code=start
public class Solution {
    public int MaxProfit(int[] prices, int fee) {
        if(prices==null || prices.Length==0)
            return 0;
        
        int len=prices.Length;
        //Dp1, pay the fee when buy stocks
        // int[,] dp=new int[len, 2];
        // dp[0,0]=0; dp[0,1]=-prices[0]-fee;
        // for(int i=1; i<len; i++){
        //     dp[i,0]=Math.Max(dp[i-1,0], dp[i-1,1]+prices[i]);
        //     dp[i,1]=Math.Max(dp[i-1,1], dp[i-1,0]-prices[i]-fee);
        // }
        // return dp[len-1,0];

        //Dp2, pay the fee when sell stocks
        // int[,] dp=new int[len,2];
        // dp[0,0]=0; dp[0,1]=-prices[0];
        // for(int i=1; i<len; i++){
        //     dp[i,0]=Math.Max(dp[i-1,0], dp[i-1,1]+prices[i]-fee);
        //     dp[i,1]=Math.Max(dp[i-1,1], dp[i-1,0]-prices[i]);
        // }
        // return dp[len-1,0];

        //Dp3, compression space
        int dp_i0=0, dp_i1=-prices[0]-fee;
        for(int i=0; i<len; i++){
            dp_i0=Math.Max(dp_i0, dp_i1+prices[i]);
            dp_i1=Math.Max(dp_i1, dp_i0-prices[i]-fee);
        }
        return dp_i0;
    }
}
// @lc code=end

