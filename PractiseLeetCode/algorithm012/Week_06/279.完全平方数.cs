/*
 * @lc app=leetcode.cn id=279 lang=csharp
 *
 * [279] 完全平方数
 */

// @lc code=start
public class Solution {
    public int NumSquares(int n) {
        //dynamic programming. O(n*Sqrt(n)), O(n)
        // int[] dp=new int[n+1];
        // for(int i=1; i<=n; i++){
        //     dp[i]=i;
        //     for(int j=1; i-j*j>=0; j++)
        //         dp[i]=Math.Min(dp[i], dp[i-j*j]+1);
        // }
        // return dp[n];

        //M2:BFS
        var queue=new Queue<int>();
        queue.Enqueue(n);

        var visited=new HashSet<int>();
        int level=0;
        while(queue.Any()){
            var size=queue.Count;
            level++;
            while(size-->0){
                var cur=queue.Dequeue();
                for(int i=0; cur-i*i>=0; i++){
                    var next=cur-i*i;
                    if(next==0)
                        return level;
                    
                    if(!visited.Contains(next)){
                        visited.Add(next);
                        queue.Enqueue(next);
                    }
                }
            }
        }
        return -1;
    }
}
// @lc code=end

