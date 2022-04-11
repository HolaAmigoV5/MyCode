/*
 * @lc app=leetcode.cn id=169 lang=csharp
 *
 * [169] 多数元素
 */

// @lc code=start
public class Solution {
    public int MajorityElement(int[] nums) {
        //find the majority element.

        //M1: Sort O(nlogn) O(logn)
        // Array.Sort(nums);
        // return nums[nums.Length/2];
        //return nums.GroupBy(i=>i).OrderBy(i=>i.Count()).Select(i=>i.Key).Last();

        //M2:Dictionary. O(n), O(n)
        // var dic=new Dictionary<int, int>();
        // var n=nums.Length/2;
        // foreach(var num in nums){
        //     if(dic.ContainsKey(num)){
        //         dic[num]++;
        //     }
        //     else{
        //         dic[num]=1;
        //     }
        //     if(dic[num]>n)
        //         return num;
        // }
        // return -1;

        //M3:Moore Vote. O(n), O(1)
        // int candidate=0, count=0;
        // for(int i=0; i<nums.Length; i++){
        //     if(count==0)
        //         candidate=nums[i];
        //     count+=nums[i]==candidate?1:-1;
        // }
        // return candidate;

        //M4:divide and Conquer
        return MajorityElementRec(nums,0, nums.Length-1);
    }

    private int MajorityElementRec(int[] nums, int lo, int hi){
        if(lo==hi)
            return nums[lo];
        int mid=lo+(hi-lo)/2;
        int left=MajorityElementRec(nums, lo, mid);
        int right=MajorityElementRec(nums, mid+1, hi);

        if(left==right)
            return left;
        
        int leftCount=CountInRange(nums, left, lo, hi);
        int rightCount=CountInRange(nums, right,lo, hi);

        return leftCount>rightCount?left:right;
    }

    private int CountInRange(int[] nums, int num, int lo, int hi){
        int count=0;
        for(int i=lo; i<=hi; i++){
            if(nums[i]==num)
                count++;
        }
        return count;
    }
}
// @lc code=end

