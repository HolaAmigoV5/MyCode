/*
 * @lc app=leetcode.cn id=153 lang=csharp
 *
 * [153] 寻找旋转排序数组中的最小值
 */

// @lc code=start
public class Solution {
    public int FindMin(int[] nums) {
        //O(logn), O(1)
        int left=0;
        int right=nums.Length-1;
        // while (left<right)
        // {
        //     int mid=left+(right-left)/2;
        //     if(nums[mid]>nums[right])
        //         left=mid+1;
        //     else
        //         right=mid;
        // }
        // return nums[left];

        while (left<right)
        {
            if(nums[left]<nums[right])
                return nums[left];
            
            int mid=left+(right-left)/2;
            if(nums[mid]>=nums[left])
                left=mid+1;
            else
                right=mid;
        }
        return nums[left];

        // use Array api:Min()
        //return nums.Min();

        //O(n) O(1)
        // int min=nums[0];
        // foreach(var num in nums)
        //     min=Math.Min(num,min);
        // return min;
    }
}
// @lc code=end

