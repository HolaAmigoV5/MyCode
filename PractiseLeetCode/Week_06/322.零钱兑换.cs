/*
 * @lc app=leetcode.cn id=322 lang=csharp
 *
 * [322] 零钱兑换
 */

// @lc code=start
public class Solution {
    public int CoinChange(int[] coins, int amount) {
        //Iteration. from bottom to top
        // var dp=new int[amount+1];
        // Array.Fill(dp, amount+1);
        // dp[0]=0;
        // for(int i=0; i<dp.Length; i++){
        //     foreach (int coin in coins)
        //     {
        //         if(i-coin<0)
        //             continue;
        //         dp[i]=Math.Min(dp[i], dp[i-coin]+1);
        //     }
        // }
        // return (dp[amount]==amount+1)?-1:dp[amount];

        //bfs
        if(amount==0) return 0;
        
        int count=0;
        var visited=new HashSet<int>();
        var queue=new Queue<int>();
        queue.Enqueue(amount);

        while(queue.Any()){
            var size=queue.Count;
            count++;
            while(size-->0){
                var num=queue.Dequeue();
                for(int i=0; i<coins.Length; i++){
                    if(coins[i]<=num){
                        var next=num-coins[i];
                        if(next==0)
                            return count;
                        if(!visited.Contains(next)){
                            visited.Add(next);
                            queue.Enqueue(next);
                        }
                    }
                   
                }
            }
        }
        return -1;


        //Greedy
        // Array.Sort(coins);
        // int res=amount+1;
        // CoinChangeHelper(coins.Length-1, coins, 0, amount, ref res);
        // return res==amount+1?-1:res;
    }

    private void CoinChangeHelper(int index, int[] coins, int count, int needAmount, ref int ans){
        if(needAmount==0){
            ans=Math.Min(count,ans);
            return;
        }
        if(index<0)
            return;
        int i=needAmount/coins[index];
        for(int k=i; k>=0 && count+k<ans;k--)
            CoinChangeHelper(index-1, coins, count+k, needAmount-k*coins[index], ref ans);
    }
}
// @lc code=end

