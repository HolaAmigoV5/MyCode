/*
 * @lc app=leetcode.cn id=58 lang=csharp
 *
 * [58] 最后一个单词的长度
 */

// @lc code=start
public class Solution {
    public int LengthOfLastWord(string s) {
        if(string.IsNullOrEmpty(s))
            return 0;
        //return s.Trim().Split(' ').Last().Length;

        // int end=s.Length-1;
        // while(end>=0 && s[end]==' ')
        //     end--;
        // if(end<0) return 0;

        // int start=end;
        // while(start>=0 && s[start]!=' ')
        //     start--;
        
        // return end-start;

        //M2
        s=s.Trim();
        int count=0, len=s.Length;
        if(len>0){
            while(len-->0){
                if(s[len]!=' ')
                    count++;
                else
                    return count;
            }
        }
        return count;
    }
}
// @lc code=end

