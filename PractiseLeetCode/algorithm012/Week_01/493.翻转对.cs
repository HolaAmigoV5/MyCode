/*
 * @lc app=leetcode.cn id=493 lang=csharp
 *
 * [493] 翻转对
 */

// @lc code=start
public class Solution {
    public int ReversePairs(int[] nums) {
         if(nums==null || nums.Length==0)
            return 0;

        //burte force. timeout
        //int count=0, len=nums.Length;
        // for(int i=0; i<len; i++){
        //     for(int j=i+1; j<len; j++){
        //         if(nums[i]/2.0>nums[j])
        //             count++;
        //     }
        // }
        // return count;

        //MergeSort
        return MergeSort(nums, 0, nums.Length-1);
    }

    private int MergeSort(int[] nums, int left, int right){
        if(left>=right)
            return 0;
        
        int mid=(left+right)>>1;
        //左右子序列翻转对个数和
        int cnt=MergeSort(nums, left, mid)+MergeSort(nums, mid+1, right);

        for(int i=left, j=mid+1; i<=mid; i++){
            while(j<=right && nums[i]/2.0>nums[j])
                j++;
            
            //左序列对应右序列翻转对个数
            cnt+=j-(mid+1);
        }
         //合并排序
        //Array.Sort(nums, left, right-left+1);
        Merge(nums, left, mid, right);
        return cnt;
    }


    private void Merge(int[] arr, int left, int mid, int right){
        int[] tmp=new int[right-left+1];
        int i=left, j=mid+1, k=0;
        
        while(i<=mid && j<=right)
            tmp[k++]=arr[i]<=arr[j]?arr[i++]:arr[j++];
        while(i<=mid)
            tmp[k++]=arr[i++];
        while(j<=right)
            tmp[k++]=arr[j++];
        
        Array.Copy(tmp,0,arr,left, tmp.Length);
    }
}
// @lc code=end

