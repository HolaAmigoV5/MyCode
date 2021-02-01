/*
 * @lc app=leetcode.cn id=338 lang=csharp
 *
 * [338] 比特位计数
 */

// @lc code=start
public class Solution {
    public int[] CountBits(int num) {
        //M1:O(nk), O(n)
        //int[] res=new int[num+1];
        // for(int i=1; i<=num; i++){
        //     int count=0;
        //     int tmp=i;
        //     while(tmp!=0){
        //         count++;
        //         tmp&=(tmp-1);
        //     }
        //     res[i]=count;
        // }
        // return res;

        //M2:dp. O(n), O(n)
        int[] dp=new int[num+1];
        for(int i=1; i<=num; i++){
            // if((i&1)==1)
            //     dp[i]=dp[i-1]+1;
            // else
            //     dp[i]=dp[i>>1];

            //dp[i]=dp[i>>1]+(i&1);

            dp[i]=dp[i&(i-1)]+1;
        }
        return dp;
    }
}
// @lc code=end

