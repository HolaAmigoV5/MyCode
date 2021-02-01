/*
 * @lc app=leetcode.cn id=20 lang=csharp
 *
 * [20] 有效的括号
 */

// @lc code=start
public class Solution {
    public bool IsValid(string s) {
        Dictionary<char, char> map=new Dictionary<char, char>{
            {']','['},{')','('},{'}','{'}
        };
        var stack=new Stack<char>();
        foreach(var item in s){
            if(map.ContainsKey(item)){
                if(stack.Count==0)
                    return false;
                else{
                    if(stack.Pop()!=map[item])
                        return false;
                }
            }
            else
                stack.Push(item);
        }
        return stack.Count==0;

        // if(s==null) return false;
        // var stack=new Stack<char>();
        // foreach (var item in s)
        // {
        //     switch (item)
        //     {
        //         case '(':
        //             stack.Push(')');
        //             break;
        //         case '{':
        //             stack.Push('}');
        //             break;
        //         case '[':
        //             stack.Push(']');
        //             break;
        //         default:
        //             if(stack.Count==0 || item!=stack.Pop())
        //                 return false;
        //             break; 
        //     }
        // }
        // return stack.Count==0;
    }
}
// @lc code=end

