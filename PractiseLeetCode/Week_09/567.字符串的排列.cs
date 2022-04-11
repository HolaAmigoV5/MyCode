/*
 * @lc app=leetcode.cn id=567 lang=csharp
 *
 * [567] 字符串的排列
 */

// @lc code=start
public class Solution {
    public bool CheckInclusion(string s1, string s2) {
        var need=new Dictionary<char, int>();
        var window=new Dictionary<char, int>();

        foreach(var ch in s1){
            if(need.ContainsKey(ch))
                need[ch]++;
            else
                need[ch]=1;
        }

        int left=0, right=0, count=0;
        while(right<s2.Length){
            char c=s2[right];
            right++;
            
            //进行窗口数据的一系列更新
            if(need.ContainsKey(c)){
                if(window.ContainsKey(c)){
                    window[c]++;
                }
                else
                    window[c]=1;
                if(window[c]==need[c])
                    count++;
            }

            //判断左侧窗口是否需要收缩
            while(right-left>=s1.Length){
                if(count==need.Count)
                    return true;
                char d=s2[left];
                left++;

                //进行窗口内数据一系列更新
                if(need.ContainsKey(d)){
                    if(window[d]==need[d])
                        count--;
                    window[d]--;
                }
            }
        }
        return false;
    }
}
// @lc code=end

