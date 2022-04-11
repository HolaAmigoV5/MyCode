/*
 * @lc app=leetcode.cn id=42 lang=csharp
 *
 * [42] 接雨水
 */

// @lc code=start
public class Solution {
    public int Trap(int[] height) {
        //M1: Time limit exceeded
        /* int sum=0;
        //最两端的列不用考虑，因为一定不会有雨水。所以下标从1到Length-2
        for (int i = 1; i < height.Length - 1; i++)
        {
            int max_left = 0;
            //找出左边最高
            for (int j = i - 1; j >= 0; j--)
            {
                if (height[j] > max_left) 
                {
                    max_left = height[j];
                }
            }

            int max_right = 0;
            //找出右边最高
            for (int j = i + 1; j < height.Length; j++)
            {
                if (height[j] > max_right)
                    max_right = height[j];
            }

            //找出两端较小的
            int min = Math.Min(max_left, max_right);
            //只有较小的一段大于当前列的高度才会有水，其他情况不会有水
            if (min > height[i])
                sum = sum + (min - height[i]);
        }*/

        //M2:
        /* int sum = 0;
        int[] max_left = new int[height.Length];
        int[] max_right = new int[height.Length];

        for (int i = 1; i < height.Length - 1; i++)
            max_left[i] = Math.Max(max_left[i - 1], height[i - 1]);

        for (int i = height.Length - 2; i >= 0; i--)
            max_right[i] = Math.Max(max_right[i + 1], height[i + 1]);

        for (int i = 1; i < height.Length - 1; i++)
        {
            int min = Math.Min(max_left[i], max_right[i]);
            sum += Math.Max(0, min - height[i]);
        } */

        //M3:double pointer
        int len=height.Length, left=0, right=len-1, max_left=0, max_right=0, res=0;
        while(left<right){
            if(height[left]<height[right]){
                max_left=Math.Max(max_left, height[left]);
                res+=max_left-height[left++];
            }
            else{
                max_right=Math.Max(max_right, height[right]);
                res+=max_right-height[right--];
            }
        }
        return res;
    }
}
// @lc code=end

