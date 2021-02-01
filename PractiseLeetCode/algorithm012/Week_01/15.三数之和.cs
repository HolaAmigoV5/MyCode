/*
 * @lc app=leetcode.cn id=15 lang=csharp
 *
 * [15] 三数之和
 */

// @lc code=start
public class Solution {
    public IList<IList<int>> ThreeSum(int[] nums) {
        //O(n*n). O(1)
        
        var res=new List<IList<int>>();
        if(nums==null || nums.Length<3)
            return res;
        
        Array.Sort(nums);
        for(int i=0; i<nums.Length; i++){
            if(nums[i]>0)
                return res;
            if(i>0 && nums[i]==nums[i-1])
                continue;
            int left=i+1;
            int right=nums.Length-1;
            while(left<right){
                int sum=nums[i]+nums[left]+nums[right];
                if(sum==0){
                    res.Add(new List<int>(){nums[i], nums[left],nums[right]});
                    while(left<right && nums[left]==nums[left+1])
                        left++;
                    while(left<right && nums[right]==nums[right-1])
                        right--;
                    left++;
                    right--;
                }
                else if(sum<0)
                    left++;
                else if(sum>0)
                    right--;
            }
        }
        return res;
    }
}
// @lc code=end

