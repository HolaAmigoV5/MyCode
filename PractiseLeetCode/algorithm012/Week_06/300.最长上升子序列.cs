/*
 * @lc app=leetcode.cn id=300 lang=csharp
 *
 * [300] 最长上升子序列
 */

// @lc code=start
public class Solution {
    public int LengthOfLIS(int[] nums) {
        //M1: dp
        if(nums.Length<2) return nums.Length;
        var len=nums.Length;
        //dp[i]表示以nums[i]结尾的「上升子序列」的长度
        var dp=new int[len]; 
        
        Array.Fill(dp,1); //每个元素都至少可以单独成为子序列，此时长度都为 1
        int max=1;

        for(int i=1; i<len; i++){
            for(int j=0; j<i; j++){
                if(nums[j]<nums[i]){
                    dp[i]=Math.Max(dp[i], dp[j]+1);
                    max=Math.Max(max, dp[i]);
                }
                    
            }
        }
        return max;

        //patience sorting 耐心排序， “蜘蛛牌”算法
        // int[] top=new int[nums.Length];
        // int piles=0;   //牌堆数初始化为0
        // foreach(var num in nums){
        //     int left=0, right=piles;
        //     while(left<right){
        //         var mid=(left+right)>>1;
        //         if(top[mid]<num)
        //             left=mid+1;
        //         else
        //             right=mid;
        //     }

        //     //没有找到合适的牌堆，新建一堆
        //     if(left==piles)
        //         piles++;
        //     top[left]=num;  //把这张牌放到最左边
        // }
        // return piles;
    }
}
// @lc code=end

