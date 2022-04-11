/*
 * @lc app=leetcode.cn id=552 lang=csharp
 *
 * [552] 学生出勤记录 II
 */

// @lc code=start
public class Solution {
    public int CheckRecord(int n) {
        int _mod=1000000007;
        if(n==1) return 3;
        if(n==2) return 8;
        long[] dp=new long[n+1];

        dp[0]=1; dp[1]=2; dp[2]=4;
        for(int i=3; i<=n; i++)
            dp[i]=(dp[i-3]+dp[i-2]+dp[i-1])%_mod;
        
        //插入A
        long anum=0;
        for(int i=1; i<=n; i++){
            anum+=(dp[i-1]*dp[n-i])%_mod;
        }
        return (int)((anum+dp[n])%_mod);
    }
}
// @lc code=end

