/*
 * @lc app=leetcode.cn id=215 lang=csharp
 *
 * [215] 数组中的第K个最大元素
 */

// @lc code=start
public class Solution {
    public int FindKthLargest(int[] nums, int k) {
        // Array.Sort(nums);
        // return nums[^k];

        var len=nums.Length;
        //QuickSort
        int left=0, right=len-1, target=len-k;
        while(left<right){
            int index=Partition(nums, left, right);
            if(index>target)
                right=index-1;
            else if(index<target)
                left=index+1;
            else
                break;
        }
        return nums[target];
    }

    private int Partition(int[] arr, int begin, int end){
        int pivot=end, counter=begin;
        for(int i=begin; i<end; i++){
            if(arr[i]<arr[pivot]){
                Swap(arr, i, counter);
                counter++;
            }
        }
        Swap(arr, pivot, counter);
        return counter;
    }
    private void Swap(int[] arr, int i, int j){
        if(i==j) return;
        var tmp=arr[i];
        arr[i]=arr[j];
        arr[j]=tmp;
    }
}
// @lc code=end

