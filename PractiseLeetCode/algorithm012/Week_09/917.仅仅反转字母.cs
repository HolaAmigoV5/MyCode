/*
 * @lc app=leetcode.cn id=917 lang=csharp
 *
 * [917] 仅仅反转字母
 */

// @lc code=start
public class Solution {
    public string ReverseOnlyLetters(string S) {
         //M1: stack.O(n), O(n)
        // var stack=new Stack<char>();
        // var chars=S.ToCharArray();
        // foreach(var ch in chars)
        //     if(char.IsLetter(ch))
        //         stack.Push(ch);

        // var sb=new StringBuilder();
        // foreach(var ch in chars){
        //     if(char.IsLetter(ch))
        //         sb.Append(stack.Pop());
        //     else
        //         sb.Append(ch);
        // }

        // return sb.ToString();

        //M2:double pointers
        var chars=S.ToCharArray();
        int left=0, right=S.Length-1;

        while(left<right){
            while(!IsLetter(chars[left]) && left<right) left++;
            while(!IsLetter(chars[right]) && left<right) right--;

            //swap
            if(left<right){
                var tmp=chars[left];
                chars[left++]=chars[right];
                chars[right--]=tmp;
            }
        }
        return new string(chars);
    }

    private bool IsLetter(char c){
        return ((c>=65 && c<=90) || (c>=97 && c<=122));
    }
}
// @lc code=end

