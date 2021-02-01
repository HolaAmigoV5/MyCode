/*
 * @lc app=leetcode.cn id=76 lang=csharp
 *
 * [76] 最小覆盖子串
 */

// @lc code=start
public class Solution {
    public string MinWindow(string s, string t) {
        if(string.IsNullOrEmpty(s) || string.IsNullOrEmpty(t))
            return "";
        
        // int[] need=new int[128];
        //记录需要的字符个数
        // foreach(var ch in t)
        //     need[ch]++;
        // int counter=t.Length, begin =0, end=0, d=int.MaxValue, head=0;
        // while(end<s.Length){
        //     if(need[s[end++]]-->0) counter--;
        //     while(counter==0){
        //         if(end-begin<d) d=end-(head=begin);
        //         if(need[s[begin++]]++==0) counter++;
        //     }
        // }
        // return d==int.MaxValue?"":s.Substring(head,d);

        var need=new Dictionary<char, int>();
        var window=new Dictionary<char, int>();

        //记录需要的字符个数
        foreach(var ch in t){
            if(need.ContainsKey(ch))
                need[ch]++;
            else
                need[ch]=1;
        }

        int start=0, len=s.Length+1;
        int left=0, right=0, count=0;
        while(right<s.Length){
            char ch =s[right];
            right++; //右移动窗口

            //进行窗口数据更新
            if(need.ContainsKey(ch)){
                if(window.ContainsKey(ch))
                    window[ch]++;
                else
                    window[ch]=1;

                if(window[ch]==need[ch])
                    count++;
            }
            //判断左侧窗口是否需要收缩
            while(count==need.Count){
                //这里更新最小覆盖子串
                if(right-left<len){
                    start=left;
                    len=right-left;
                }
                //d是移出窗口的字符
                char d=s[left];
                left++; //左移窗口

                //窗口数据更新
                if(need.ContainsKey(d)){
                    if(window[d]==need[d])
                        count--;
                    window[d]--;
                }
            }
        }

        return len==s.Length+1?"":s.Substring(start, len);
    }
}
// @lc code=end

