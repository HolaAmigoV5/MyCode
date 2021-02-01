/*
 * @lc app=leetcode.cn id=26 lang=csharp
 *
 * [26] 删除排序数组中的重复项
 */

// @lc code=start
public class Solution
{
    public int RemoveDuplicates(int[] nums)
    {
        if (nums == null || nums.Length == 0) return 0;
        // int p = 0, q = 1;
        // while (q < nums.Length)
        // {
        //     if (nums[p] != nums[q])
        //     {
        //         if (q - p > 1)
        //             nums[p + 1] = nums[q];
        //         p++;
        //     }
        //     q++;
        // }
        // return p + 1;

        // int count=0, len=nums.Length;
        // for(int i=1; i<len; i++){
        //     if(nums[i]==nums[i-1])
        //         count++;
        //     else
        //         nums[i-count]=nums[i];
        // }
        // return len-count;

        int len=nums.Length, id=1;
        if(len<2) return len;
        for(int i=1; i<len;i++){
            if(nums[i]!=nums[i-1]) nums[id++]=nums[i];
        }
        return id;
    }
}
// @lc code=end

