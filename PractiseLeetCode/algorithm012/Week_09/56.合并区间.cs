/*
 * @lc app=leetcode.cn id=56 lang=csharp
 *
 * [56] 合并区间
 */

// @lc code=start
public class Solution {
    public int[][] Merge(int[][] intervals) {
        if(intervals.Length<=1)
            return intervals;

        //先按照区间起始位置排序
        Array.Sort(intervals, (a, b) =>(a[0] - b[0]));
        var res=new List<int[]>(intervals.Length);

        foreach(var item in intervals){
            //如果结果集为空或者当前区间起始位置>结果集最后区间的终止位置
            //则不合并，直接将当前区间加入结果集中
            //这里res[^1]=res[res.Count-1]
            if(res.Count==0 || item[0]>res[^1][1])
                res.Add(item);
            else
                //反之，则合并区间(更新结果集最后区间的终止位置)
                res[^1][1]=Math.Max(res[^1][1], item[1]);
        }
        return res.ToArray();
    }
}
// @lc code=end

