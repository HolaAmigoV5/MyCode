/*
 * @lc app=leetcode.cn id=213 lang=csharp
 *
 * [213] 打家劫舍 II
 */

// @lc code=start
public class Solution {
    public int Rob(int[] nums) {
        int len = nums.Length;
        if(len == 0 ) return 0;
        if(len == 1 ) return nums[0];

        return Math.Max(DpRob(nums, 0, len-2), DpRob(nums, 1, len-1));
    }

    private int DpRob(int[] nums, int start, int end){
        int[] dp=new int[nums.Length];
        if(start==0){
            dp[0]=nums[0];
            dp[1]=Math.Max(nums[0], nums[1]);
        }
        else
            dp[1]=nums[1];
        for(int i=2; i<=end; i++)
            dp[i]=Math.Max(dp[i-1], dp[i-2]+nums[i]);

        return dp[end];

        // int curr = 0 ,pre = 0 ,temp;
        // for(int i = start ; i < length ; i++){
        //     temp = curr;
        //     curr = Math.Max(curr,pre+nums[i]);
        //     pre = temp;
        // }
        // return curr;
    }
}
// @lc code=end

