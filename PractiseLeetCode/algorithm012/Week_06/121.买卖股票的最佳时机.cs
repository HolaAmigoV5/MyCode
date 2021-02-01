/*
 * @lc app=leetcode.cn id=121 lang=csharp
 *
 * [121] 买卖股票的最佳时机
 */

// @lc code=start
public class Solution {
    public int MaxProfit(int[] prices) {
        int len=prices.Length;
        if(len<2) return 0;
        // int[,] dp=new int[len, 2];
        // //base case
        // dp[0,0]=0;  // the first day without stock
        // dp[0,1]=-prices[0];  // the first day with stock
        // for(int i=1; i<len; i++){
        //     dp[i,0]=Math.Max(dp[i-1,0], dp[i-1,1]+prices[i]);
        //     dp[i,1]=Math.Max(dp[i-1,1], -prices[i]);
        // }
        // return dp[len-1,0];

        //M2:states compression
        //base case
        // int dp_i_0=0, dp_i_1=int.MinValue;

        // for(int i=0; i<len; i++){
        //     dp_i_0=Math.Max(dp_i_0, dp_i_1+prices[i]);
        //     dp_i_1=Math.Max(dp_i_1, -prices[i]);
        // }
        // return dp_i_0;

        //M3:brute force
        // int maxProfit=0;
        // for(int i=0; i<len; i++){
        //     for(int j=i+1; j<len; j++){
        //         maxProfit=Math.Max(maxProfit, prices[j]-prices[i]);
        //     }
        // }
        // return maxProfit;

        //M4:
        int minProfit=int.MaxValue;
        int maxProfit=0;
        for(int i=0; i<len; i++){
            if(prices[i]<minProfit)
                minProfit=prices[i];
            else if(prices[i]-minProfit>maxProfit)
                maxProfit=prices[i]-minProfit;
        }
        return maxProfit;
        
    }
}
// @lc code=end

