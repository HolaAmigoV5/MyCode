/*
 * @lc app=leetcode.cn id=264 lang=csharp
 *
 * [264] 丑数 II
 */

// @lc code=start
public class Solution {
    public int NthUglyNumber(int n) {
        //Dp
       if (n <= 5) return n;
            int[] dp = new int[n];
            dp[0] = 1;

            int value, index2 = 0, index3 = 0, index5 = 0;
            for (int i = 1; i < n; i++)
            {
                value = Math.Min(Math.Min(dp[index2] * 2, dp[index3] * 3), dp[index5] * 5);
                dp[i] = value;
                if (value == dp[index2] * 2) index2++;
                if (value == dp[index3] * 3) index3++;
                if (value == dp[index5] * 5) index5++;
            }

            return dp[n - 1];


        // if(n==1) return 1;
        // SortedSet<long> list=new SortedSet<long>();
        // list.Add(1);
        // int count=1;
        // while(count<n){
        //     var min=list.Min;
        //     list.Remove(min);
        //     list.Add(min*2);
        //     list.Add(min*3);
        //     list.Add(min*5);
        //     count++;
        // }
        // return (int)list.Min;
    }
}
// @lc code=end

