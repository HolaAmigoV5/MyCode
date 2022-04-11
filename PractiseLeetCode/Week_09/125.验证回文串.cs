/*
 * @lc app=leetcode.cn id=125 lang=csharp
 *
 * [125] 验证回文串
 */

// @lc code=start
public class Solution {
    public bool IsPalindrome(string s) {
        if(s==string.Empty) return true;
        s=s.ToLower();
        int left=0, right=s.Length-1;
        while(left<right){
            // while(left<right && !char.IsLetterOrDigit(s[left])) left++;
            // while(left<right && !char.IsLetterOrDigit(s[right])) right--;

            while(left<right && !IsCharOrNumber(s[left])) left++;
            while(left<right && !IsCharOrNumber(s[right])) right--;

            if(s[left++]!=s[right--])
                return false;
        }
        return true;
    }

    private bool IsCharOrNumber(char s)
    {
        if((s>='a'&& s<='z')||(s>='0'&& s<='9'))
        {
            return true;
        }
        return false;
    }
}
// @lc code=end

