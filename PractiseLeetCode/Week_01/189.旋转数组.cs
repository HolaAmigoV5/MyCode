/*
 * @lc app=leetcode.cn id=189 lang=csharp
 * [189] 旋转数组
 */

// @lc code=start
public class Solution {
    public void Rotate(int[] nums, int k) {
        //M1:move 1 step, k times
        // int tem=0, pre=0;
        // k%=nums.Length;
        // for(int i=0;i<k;i++){
        //     pre=nums[nums.Length-1];
        //     for(int j=0;j<nums.Length;j++){
        //         tem=nums[j];
        //         nums[j]=pre;
        //         pre=tem;
        //     }
        // }

        //M2：new Array
        /* int len=nums.Length;
        int [] newNums=new int[len];
        for(int i=0;i<len;i++){
            newNums[(i+k)%len]=nums[i];
        }
        for(int i=0;i<len;i++){
            nums[i]=newNums[i];
        } */

        //M3:loop Replace
        int len=nums.Length;
        k=k%len;
        int count=0;
        for(int start=0; count<len;start++){
            int current=start;
            int prev=nums[start];
            do
            {
                int next=(current+k)%len;
                int temp=nums[next];
                nums[next]=prev;
                prev=temp;
                current=next;
                count++;
            } while (start!=current);
        } 

        //M4: Reverse
        // int len=nums.Length;
        // k%=len;
        // reverse(nums,0,len-1);
        // reverse(nums,0,k-1);
        // reverse(nums,k,len-1);
    }

    public void reverse(int[] nums, int start, int end){
        while (start < end)
        {
            int temp = nums[start];
            nums[start] = nums[end];
            nums[end] = temp;
            start++;
            end--;
        }
    }
}
// @lc code=end

