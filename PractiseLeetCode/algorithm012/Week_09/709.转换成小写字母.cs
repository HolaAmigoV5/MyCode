/*
 * @lc app=leetcode.cn id=709 lang=csharp
 *
 * [709] 转换成小写字母
 */

// @lc code=start
public class Solution {
    public string ToLowerCase(string str) {
        if(string.IsNullOrEmpty(str))
            return str;
        int len=str.Length;
        var sb=new StringBuilder(str);

        for(int i=0; i<len; i++){
            if(sb[i]>='A' && sb[i]<='Z'){
                sb[i]=(char)(sb[i]+('a'-'A'));
            }
                
        }
        return sb.ToString();
    }
}
// @lc code=end

