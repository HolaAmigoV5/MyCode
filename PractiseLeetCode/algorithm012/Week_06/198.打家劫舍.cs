/*
 * @lc app=leetcode.cn id=198 lang=csharp
 *
 * [198] 打家劫舍
 */

// @lc code=start
public class Solution {
    public int Rob(int[] nums) {
        if(nums==null || nums.Length==0)
            return 0;
        
        //O(n), O(n)
        int n=nums.Length;
        if(n==1) return nums[0];

        int[] dp=new int[n];
        dp[0]=nums[0];
        dp[1]=Math.Max(nums[0],nums[1]);

        for(int i=2; i<n; i++)
            dp[i]=Math.Max(dp[i-1], dp[i-2]+nums[i]);
        
        return dp[n-1];

        //M2: states Compressing. O(n), O(1)
        // int prev=0, curr=0;
        // foreach(var num in nums){
        //     int temp=Math.Max(curr, prev+num);
        //     prev=curr;
        //     curr=temp;
        // }
        // return curr;
    }
}
// @lc code=end

