/*
 * @lc app=leetcode.cn id=344 lang=csharp
 *
 * [344] 反转字符串
 */

// @lc code=start
public class Solution {
    public void ReverseString(char[] s) {
        if(s==null || s.Length<2)
            return;
        int left=0, right=s.Length-1;

        while(left<right){
            var tmp=s[left];
            s[left++]=s[right];
            s[right--]=tmp;
        }
    }
}
// @lc code=end

