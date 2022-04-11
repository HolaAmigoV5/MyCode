/*
 * @lc app=leetcode.cn id=680 lang=csharp
 *
 * [680] 验证回文字符串 Ⅱ
 */

// @lc code=start
public class Solution {
    public bool ValidPalindrome(string s) {
        int front=0, end=s.Length-1;
        while(front<end){
            if(s[front]!=s[end])
                return IsPalindrome(s,front+1,end)||IsPalindrome(s,front,end-1);
            front++;
            end--;
        }
        return true;
    }

    public bool IsPalindrome(string s, int front, int end){
        while(front<end){
            if(s[front++]!=s[end--])
                return false;
        }
        return true;
    }
}
// @lc code=end

