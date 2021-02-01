/*
 * @lc app=leetcode.cn id=541 lang=csharp
 *
 * [541] 反转字符串 II
 */

// @lc code=start
public class Solution {
    public string ReverseStr(string s, int k) {
        //brute force. O(n), O(n)
        char[] arr=s.ToCharArray();
        int len=arr.Length, i=0;
        while(i<len){
            int j=Math.Min(i+k-1, len-1);
            Reverse(arr,i,j);
            i+=2*k;
        }
        return new string(arr);
    }

    private void Reverse(char[] arr, int left, int right){
        while(left<right){
            var tmp=arr[left];
            arr[left++]=arr[right];
            arr[right--]=tmp;
        }
    }
}
// @lc code=end

