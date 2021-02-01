/*
 * @lc app=leetcode.cn id=8 lang=csharp
 *
 * [8] 字符串转换整数 (atoi)
 */

// @lc code=start
using System.Text.RegularExpressions;
public class Solution {
    public int MyAtoi(string str) {
        if(string.IsNullOrEmpty(str.Trim()))
            return 0;

        int sign=1, ans=0, i=0;
        while(str[i]==' ') //忽略空格
            i++;
        if(str[i]=='-' || str[i]=='+') //处理正负号
            sign=str[i++]=='-'?-1:1;
        
        while(i<str.Length && str[i]>='0' && str[i]<='9'){
             //处理数字，注意数字越界时返回int.MaxValue或 int.MinValue
             //int.MaxValue=2147483647, 最后一位是7
             //int.MinValue=-2147483648，最后一位是8
             if(ans>int.MaxValue/10 || (ans==int.MaxValue/10 && str[i]>'7'))
                return (sign==1)?int.MaxValue:int.MinValue;
            ans=10*ans+(str[i++]-'0');
        }
        return ans*sign;


        //Regex
        /* 
            ^ 表示匹配字符串开头，我们匹配的就是 '+'  '-'  号
            [] 表示匹配包含的任一字符，比如[0-9]就是匹配数字字符 0 - 9 中的一个
            ? 表示前面一个字符出现零次或者一次，这里用 ? 是因为 '+' 号可以省略
            \d 表示一个数字 0 - 9 范围
            + 表示前面一个字符出现一次或者多次，\\d+ 合一起就能匹配一连串数字了
        */
        // int ans=0;
        // str=str.Trim();
        // Match match=Regex.Match(str, @"^[\+\-]?\d+");
        // if(match.Success){
        //     if(int.TryParse(match.Value, out ans))
        //         return ans;
        //     else
        //         ans=str[0]=='-'?int.MinValue:int.MaxValue;
        // }
        // return ans;
    }
}
// @lc code=end

