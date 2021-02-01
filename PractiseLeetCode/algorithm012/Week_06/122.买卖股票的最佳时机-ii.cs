/*
 * @lc app=leetcode.cn id=122 lang=csharp
 *
 * [122] 买卖股票的最佳时机 II
 */

// @lc code=start
public class Solution {
    public int MaxProfit(int[] prices) {
        int len=prices.Length;
        if(len<2) return 0;
        
        //greedy
        // int profit=0;
        // for(int i=1; i<len; i++)
        //     profit+=Math.Max(0, prices[i]-prices[i-1]);
        // return profit;

        //dp
        // int[,] dp=new int[len,2];
        // dp[0,0]=0;
        // dp[0,1]=-prices[0];
        // for(int i=1; i<len; i++){
        //     dp[i,0]=Math.Max(dp[i-1,0], dp[i-1,1]+prices[i]);
        //     dp[i,1]=Math.Max(dp[i-1,1], dp[i-1,0]-prices[i]);
        // }
        // return dp[len-1,0];

        //dp2: compression space
        int dp_i0=0, dp_i1=-prices[0];
        for(int i=1; i<len; i++){
            dp_i0=Math.Max(dp_i0, dp_i1+prices[i]);
            dp_i1=Math.Max(dp_i1, dp_i0-prices[i]);
        }
        return dp_i0;
    }
}
// @lc code=end

