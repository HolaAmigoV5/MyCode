/*
 * @lc app=leetcode.cn id=746 lang=csharp
 *
 * [746] 使用最小花费爬楼梯
 */

// @lc code=start
public class Solution {
    public int MinCostClimbingStairs(int[] cost) {
        if(cost==null || cost.Length==0)
            return 0;
        
        int len=cost.Length;
        // int[] dp=new int[len];
        // dp[1]=Math.Min(cost[0], cost[1]);

        // for(int i=2; i<len; i++){
        //     dp[i]=Math.Min(dp[i-1]+cost[i], dp[i-2]+cost[i-1]);
        // }
        // return dp[len-1];

        //M2
        for(int i=2; i<len; i++){
            cost[i]+=Math.Min(cost[i-1], cost[i-2]);
        }
        return Math.Min(cost[len-1], cost[len-2]);
    }
}
// @lc code=end

