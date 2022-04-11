/*
 * @lc app=leetcode.cn id=3 lang=csharp
 *
 * [3] 无重复字符的最长子串
 */

// @lc code=start
public class Solution {
    public int LengthOfLongestSubstring(string s) {
        if(string.IsNullOrEmpty(s))
            return 0;
        
        //M1
        var window=new Dictionary<char, int>();
        int left=0, right=0, res=0;
        while(right<s.Length){
            char c=s[right];
            right++;
            
            //进行窗口内数据的一系列更新
            if(window.ContainsKey(c))
                window[c]++;
            else
                window[c]=1;
            
            //判断右窗口是否要收缩
            while(window[c]>1){
                char d=s[left];
                left++;

                //进行一系列窗口更新
                window[d]--;
            }
            res=Math.Max(res, right-left);
        }
        return res;

        //M2
        // var dic=new Dictionary<char, int>();
        // int max=0, left=0;
        // for(int i=0; i<s.Length; i++){
        //     if(dic.ContainsKey(s[i]))
        //         left=Math.Max(left, dic[s[i]]);
        //     dic[s[i]]=i+1;
        //     max=Math.Max(max, i-left+1);
        // }
        // return max;
    }
}
// @lc code=end

