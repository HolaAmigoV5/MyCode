/*
 * @lc app=leetcode.cn id=30 lang=csharp
 *
 * [30] 串联所有单词的子串
 */

// @lc code=start
public class Solution {
    public IList<int> FindSubstring(string s, string[] words) {
        var res=new List<int>();
        if(words.Length==0 || s.Length<words.Length*words[0].Length)
            return res;
        
        int one_word=words[0].Length, word_num=words.Length, all_len=one_word*word_num;

        var map=new Dictionary<string,int>();
        foreach(var str in words){
            if(map.ContainsKey(str))
                map[str]++;
            else
                map[str]=1;
        }

       for(int i=0; i<one_word; i++){
           int left=i, right=i, count=0;
           var tmp=new Dictionary<string,int>();
           while(right+one_word<=s.Length){
                string w=s.Substring(right, one_word);
                right+=one_word;
                if(!map.ContainsKey(w)){
                    count=0;
                    left=right;
                    tmp.Clear();
                }
                else{
                    if(tmp.ContainsKey(w))
                        tmp[w]++;
                    else
                        tmp[w]=1;

                    count++;
                    while(tmp[w]>map[w]){
                        string t_w=s.Substring(left, one_word);
                        count--;
                        if(tmp.ContainsKey(t_w))
                            tmp[t_w]--;
                        left+=one_word;
                    }
                    if(count==word_num)
                        res.Add(left);
                }
           }
       }
       return res;
    }
}
// @lc code=end

