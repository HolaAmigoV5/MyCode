/*
 * @lc app=leetcode.cn id=455 lang=csharp
 *
 * [455] 分发饼干
 */

// @lc code=start
public class Solution {
    public int FindContentChildren(int[] g, int[] s) {
        if(g==null || s==null)
            return 0;
        Array.Sort(g);
        Array.Sort(s);
        int gi=0, si=0;
        while(gi<g.Length && si<s.Length){
            if(g[gi]<=s[si])
                gi++;
            si++;
        }
        return gi;
    }
}
// @lc code=end

