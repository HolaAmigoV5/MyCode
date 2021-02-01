/*
 * @lc app=leetcode.cn id=53 lang=csharp
 *
 * [53] 最大子序和
 */

// @lc code=start
public class Solution {
    public int MaxSubArray(int[] nums) {
        if(nums==null || nums.Length==0)
            return 0;
        
        //brute force
        // int max=int.MinValue;
        // int m=nums.Length;
        // for(int i=0; i<m; i++){
        //     int sum=0;
        //     for(int j=i; j<m; j++){
        //         sum+=nums[j];
        //         max=Math.Max(max, sum);
        //     }
        // }
        // return max;

        // int ans=nums[0];
        // int sum=0;
        // foreach (var num in nums)
        // {
        //     if(sum>0)
        //         sum+=num;
        //     else
        //         sum=num;
        //     ans=Math.Max(ans, sum);
        // }
        // return ans;

        //dp
        // int[] dp=new int[nums.Length];
        // dp[0]=nums[0];
        // int max=nums[0];
        // for(int i=1; i<nums.Length; i++){
        //     dp[i]=Math.Max(dp[i-1]+nums[i], nums[i]);
        //     max=Math.Max(max,dp[i]);
        // }
        // return max;

        //dp2
        int pre=0;
        int max=nums[0];
        foreach(var num in nums){
            pre=Math.Max(num, pre+num);
            max=Math.Max(max, pre);
        }
        return max;

        //divide and conquer. O(nlogn), o(1)
        //return MaxSubArray(nums,0,nums.Length-1);
    }
    private int MaxSubArray(int[] nums, int start, int end){
        if(start==end)  //only one element
            return nums[start];
        
        int mid=(start+end)/2;
        int leftMax=MaxSubArray(nums, start,mid);
        int rightMax=MaxSubArray(nums, mid+1,end);

        // 下面计算横跨两个子序列的最大值

        // 计算包含左侧子序列最后一个元素的子序列最大值
        int leftCrossMax=int.MinValue;
        int leftCrossSum=0;
        for(int i=mid; i>=start; i--){
            leftCrossSum+=nums[i];
            leftCrossMax=Math.Max(leftCrossMax,leftCrossSum);
        }

        // 计算包含右侧子序列最后一个元素的子序列最大值
        int rightCrossMax = nums[mid+1];
        int rightCrossSum = 0;
        for (int i = mid + 1; i <= end ; i ++) {
            rightCrossSum += nums[i];
            rightCrossMax = Math.Max(rightCrossSum, rightCrossMax);
        }

        // 计算跨中心的子序列的最大值
        int crossMax = leftCrossMax + rightCrossMax;

        // 比较三者，返回最大值
        return Math.Max(crossMax, Math.Max(leftMax, rightMax));   
    }
}
// @lc code=end

