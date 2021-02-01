/*
 * @lc app=leetcode.cn id=403 lang=csharp
 *
 * [403] 青蛙过河
 */

// @lc code=start
public class Solution {
    public bool CanCross(int[] stones) {
        int len=stones.Length;
        if(stones==null || len==0 || stones[1]!=1 ||
            stones[len-1]>(len*(len-1))/2)
                return false;
            
        // var map=new Dictionary<int, HashSet<int>>(len);
        // for(int i=0; i<len; i++)
        //     map.Add(stones[i], new HashSet<int>());
        
        // map[0].Add(1);

        // for(int i=0; i<len-1; i++){
        //     int stone=stones[i];
        //     foreach(var step in map[stone]){
        //         int reach=step+stone;
        //         if(reach==stones[len-1])
        //             return true;
        //         if(map.ContainsKey(reach)){
        //             map[reach].Add(step);
        //             if(step-1>0)
        //                 map[reach].Add(step-1);
        //             map[reach].Add(step+1);
        //         }
        //     }
        // }
        // return false;

        //dp
        //dp[i][k] 表示能否由前面的某一个石头 j 通过跳 k 步到达当前这个石头
        bool [,] dp=new bool[len, len+1];
        dp[0,0]=true;
        for(int i=1; i<len; i++){
            for(int j=0; j<i; j++){
                int k=stones[i]-stones[j];
                if(k<=i){
                    dp[i,k]=dp[j,k-1]|| dp[j,k] || dp[j,k+1];

                    //提前结束循环直接返回结果
                    if(i==len-1 && dp[i,k])
                        return true;
                }
            }
        }
        return false;
    }
}
// @lc code=end

