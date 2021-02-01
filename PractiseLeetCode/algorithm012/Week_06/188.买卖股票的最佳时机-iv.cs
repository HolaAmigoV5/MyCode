/*
 * @lc app=leetcode.cn id=188 lang=csharp
 *
 * [188] 买卖股票的最佳时机 IV
 */

// @lc code=start
public class Solution {
    public int MaxProfit(int k, int[] prices)
    {
        int len = prices.Length;
        if (len < 2)
            return 0;
        if (k >= len / 2)
        {
            //greedy
            int profit = 0;
            for (int i = 1; i < len; i++)
                profit += Math.Max(0, prices[i] - prices[i - 1]);
            return profit;
        }
        else
        {
            // int[,,]dp=new int[len, k+1, 2];
            // //base case
            // for(int i=1; i<=k; i++)
            //     dp[0,i,1]=-prices[0];

            // for(int i=1; i<len; i++){
            //     for(int j=1; j<k+1; j++){
            //         dp[i,j,0]=Math.Max(dp[i-1,j,0], dp[i-1,j,1]+prices[i]);
            //         dp[i,j,1]=Math.Max(dp[i-1,j,1], dp[i-1,j-1,0]-prices[i]);
            //     }
            // }
            // return dp[len-1, k, 0];

            int[,] dp = new int[k + 1, 2];
            for (int i = 1; i <= k; i++)
                dp[i, 1] = -prices[0];
            for (int i = 1; i < len; i++)
            {
                for (int j = 1; j <= k; j++)
                {
                    dp[j, 0] = Math.Max(dp[j, 0], dp[j, 1] + prices[i]);
                    dp[j, 1] = Math.Max(dp[j, 1], dp[j - 1, 0] - prices[i]);
                }
            }
            return dp[k, 0];
        }
    }
}
// @lc code=end

