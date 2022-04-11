/*
 * @lc app=leetcode.cn id=66 lang=csharp
 *
 * [66] 加一
 */

// @lc code=start
public class Solution {
    public int[] PlusOne(int[] digits) {
        if(digits==null || digits.Length==0) return null;
        for(int i= digits.Length-1; i>=0; i--){
            digits[i]++;
            digits[i]%=10;
            if(digits[i]!=0) return digits;
        }

        digits=new int[digits.Length+1];
        digits[0]=1;
        return digits;
    }
}
// @lc code=end

