/*
 * @lc app=leetcode.cn id=621 lang=csharp
 *
 * [621] 任务调度器
 */

// @lc code=start
public class Solution {
    public int LeastInterval(char[] tasks, int n) {
        //贪心
        if(tasks.Length<=0 || n<1)
            return tasks.Length;
        int[] counts=new int[26];
        for(int i=0; i<tasks.Length; i++)
            counts[tasks[i]-'A']++;
        
        Array.Sort(counts);
        int maxCount=counts[25];
        int retCount=(maxCount-1)*(n+1)+1;
        int k=24;
        while(k>=0 && counts[k]==maxCount){
            retCount++;
            k--;
        }

        return Math.Max(retCount, tasks.Length);
    }
}
// @lc code=end

