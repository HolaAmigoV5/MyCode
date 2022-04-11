/*
 * @lc app=leetcode.cn id=33 lang=csharp
 *
 * [33] 搜索旋转排序数组
 */

// @lc code=start
public class Solution {
    public int Search(int[] nums, int target) {
        if(nums==null || nums.Length==0)
            return -1;
        
        int left=0, right=nums.Length-1;
        while(left<=right){
            var mid =left+(right-left)/2;
            if(nums[mid]==target)
                return mid;
            
            //The left is ordered.
            if(nums[left]<=nums[mid]){
                if(target<nums[mid]  && target>=nums[left])
                    right=mid-1;
                else
                    left=mid+1;
            }
            else{
                //the right is ordered.
                if(target<=nums[right] && target>nums[mid])
                    left=mid+1;
                else
                    right=mid-1;
            }
        }
        return -1;
    }
}
// @lc code=end

