/*
 * @lc app=leetcode.cn id=123 lang=csharp
 *
 * [123] 买卖股票的最佳时机 III
 */

// @lc code=start
public class Solution {
    public int MaxProfit(int[] prices) {
        if(prices==null || prices.Length==0)
            return 0;

        int len=prices.Length;

        //Dp1
        // int[,,] dp=new int[len,3,2];
        
        // //base case
        // dp[0,1,0]=0; dp[0,1,1]=-prices[0];
        // dp[0,2,0]=0; dp[0,2,1]=-prices[0];

        // for(int i=1; i<len; i++){
        //     dp[i,2,0]=Math.Max(dp[i-1,2,0], dp[i-1,2,1]+prices[i]);
        //     dp[i,2,1]=Math.Max(dp[i-1,2,1], dp[i-1,1,0]-prices[i]);
        //     dp[i,1,0]=Math.Max(dp[i-1,1,0], dp[i-1,1,1]+prices[i]);
        //     dp[i,1,1]=Math.Max(dp[i-1,1,1], -prices[i]);

        //     // for(int j=1; j<max_k+1;j++){
        //     //     dp[i,j,0]=Math.Max(dp[i-1,j,0], dp[i-1,j,1]+prices[i]);
        //     //     dp[i,j,1]=Math.Max(dp[i-1,j,1], dp[i-1,j-1,0]-prices[i]);
        //     // }
        // }
        // return dp[len-1,2,0];

        //Dp2
        int dp_i10=0, dp_i11=-prices[0];
        int dp_i20=0, dp_i21=-prices[0];
        
        for(int i=1; i<len; i++){
            dp_i20=Math.Max(dp_i20, dp_i21+prices[i]);
            dp_i21=Math.Max(dp_i21, dp_i10-prices[i]);
            dp_i10=Math.Max(dp_i10, dp_i11+prices[i]);
            dp_i11=Math.Max(dp_i11, -prices[i]);
        }
        return dp_i20;
    }
}
// @lc code=end

