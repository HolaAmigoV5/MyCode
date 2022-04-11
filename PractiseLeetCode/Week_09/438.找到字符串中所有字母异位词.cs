/*
 * @lc app=leetcode.cn id=438 lang=csharp
 *
 * [438] 找到字符串中所有字母异位词
 */

// @lc code=start
public class Solution {
    public IList<int> FindAnagrams(string s, string p) {
        var res = new List<int>();
        if (p.Length > s.Length) return res;

        var need = new Dictionary<char, int>();
        var window = new Dictionary<char, int>();

        foreach (var ch in p)
        {
            if (need.ContainsKey(ch))
                need[ch]++;
            else
                need[ch] = 1;
        }

        int left = 0, right = 0, count = 0;
        while (right < s.Length)
        {
            char c = s[right];
            right++;
            //进行窗口内数据一系列更新
            if (need.ContainsKey(c))
            {
                if (window.ContainsKey(c))
                    window[c]++;
                else
                    window[c] = 1;
                if (window[c] == need[c])
                    count++;
            }

            //判断右侧窗口是否需要收缩
            while (right-left>= p.Length)
            {
                //当窗口符合条件时，把起始索引加入res
                if (count == need.Count)
                    res.Add(left);
                char d = s[left];
                left++;

                //进行窗口内数据一系列更新
                if (need.ContainsKey(d))
                {
                    if (window[d] == need[d])
                        count--;
                    window[d]--;
                }
            }
        }
        return res;
    }
}
// @lc code=end

