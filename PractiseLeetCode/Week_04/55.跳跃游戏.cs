/*
 * @lc app=leetcode.cn id=55 lang=csharp
 *
 * [55] 跳跃游戏
 */

// @lc code=start
public class Solution {
    public bool CanJump(int[] nums) {
        if(nums==null||nums.Length==0)
            return false;
        
        // int k=0;
        // for(int i=0; i<nums.Length; i++){
        //     if(i>k)
        //         return false;
        //     k=Math.Max(k, i+nums[i]);
        // }
        // return true;

        int dis=0;
        for(int i=0; i<=dis; i++){
            dis=Math.Max(dis, i+nums[i]);
            if(dis>nums.Length-1)
                return true;
        }
        return false;
    }
}
// @lc code=end

