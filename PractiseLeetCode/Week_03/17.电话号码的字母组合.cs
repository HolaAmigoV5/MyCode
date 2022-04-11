/*
 * @lc app=leetcode.cn id=17 lang=csharp
 *
 * [17] 电话号码的字母组合
 */

// @lc code=start
public class Solution {
    string[] letter_map={"", "*", "abc", "def", "ghi", "jkl", "mno", "pqrs", "tuv", "wxyz"};
    List<string> res=new List<string>();
    public IList<string> LetterCombinations(string digits) {
        //take care the Boundary
        if(digits==null || digits.Length==0)
            return new List<string>();
        // IterStr(digits, "",0);
        // return res;

        //use queue
        Queue<string> ans=new Queue<string>();
        ans.Enqueue("");
        while(ans.Peek().Length!=digits.Length){
            string tmp=ans.Dequeue();
            string str_map=letter_map[digits[tmp.Length]-'0'];
            foreach (char c in str_map)
            {
                ans.Enqueue(tmp+c);
            }
        }
        return ans.ToList();
    }

    private void IterStr(string str, string letter, int index){
        if(index==str.Length){
            res.Add(letter);
            return;
        }

        char c=str[index];
        int pos=c-'0';
        string map_string=letter_map[pos];
        for(int i=0; i<map_string.Length; i++)
            IterStr(str,letter+map_string[i],index+1);
    }
}
// @lc code=end

