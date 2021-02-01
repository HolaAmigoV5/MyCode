/*
 * @lc app=leetcode.cn id=11 lang=csharp
 *
 * [11] 盛最多水的容器
 */

// @lc code=start
public class Solution {
    public int MaxArea(int[] height){
        //O(n), O(1)
         if(height==null || height.Length==0)
            return 0;
        
        int left=0, len=height.Length, right=len-1, res=0;
        while(left<right){
            var high=height[left]<height[right]?height[left++]:height[right--];
            res=Math.Max(res, high*(right-left+1));
        }
        return res;
    }
}
// @lc code=end

