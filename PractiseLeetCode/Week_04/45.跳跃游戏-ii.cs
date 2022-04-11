/*
 * @lc app=leetcode.cn id=45 lang=csharp
 *
 * [45] 跳跃游戏 II
 */

// @lc code=start
public class Solution {
    public int Jump(int[] nums) {
        int end=0;
        int maxPosition=0;
        int steps=0;
        for(int i=0; i<nums.Length-1; i++){
            maxPosition=Math.Max(maxPosition,nums[i]+i);
            if(i==end){
                end=maxPosition;
                steps++;
            }
        }
        return steps;
    }
}
// @lc code=end

