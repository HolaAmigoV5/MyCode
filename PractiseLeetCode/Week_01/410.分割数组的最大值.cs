/*
 * @lc app=leetcode.cn id=410 lang=csharp
 *
 * [410] 分割数组的最大值
 */

// @lc code=start
public class Solution {
    public int SplitArray(int[] nums, int m) {
        if(nums==null || nums.Length==0 || m==0)
            return 0;
        
        //M1:Binary Search
        //计算「子数组各自的和的最大值」的上下界
        int left=0, right=0;
        foreach(var num in nums){
            left=Math.Max(left, num);
            right+=num;
        }

        //使用「二分查找」确定一个恰当的「子数组各自的和的最大值」，
        //使得它对应的「子数组的分割数」恰好等于m
        while(left<right){
            var mid=left+((right-left)>>1);
            int splits=Split(nums, mid);
            if(splits>m){
                //如果分割数太多，说明「子数组各自的和的最大值」太小，此时需要将「子数组各自的和的最大值」调大
                //下一轮搜索的区间是 [mid + 1, right]
                left=mid+1;
            }
            else
                right=mid;
        }
        return left;
    }

    public int Split(int[] nums, int maxSum){
        int splits=1; //至少是一个分割
        int curSum=0; //当前区间的和
        foreach(var num in nums){
            //加上去超过了「子数组各自的和的最大值」，就不加这个数，另起炉灶
            if(curSum+num>maxSum){
                curSum=0;
                splits++;
            }
            curSum+=num;
        }
        return splits;
    }
}
// @lc code=end

