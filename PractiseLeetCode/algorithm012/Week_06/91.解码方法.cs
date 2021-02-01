/*
 * @lc app=leetcode.cn id=91 lang=csharp
 *
 * [91] 解码方法
 */

// @lc code=start
public class Solution {
    public int NumDecodings(string s) {
        if(string.IsNullOrEmpty(s))
            return 0;

        // if(s[0]=='0')
        //     return 0;

        // int pre=1, cur=1;
        // for(int i=1;i<s.Length; i++){
        //     int temp=cur;
        //     if(s[i]=='0'){
        //         if(s[i-1]=='1' || s[i-1]=='2')
        //             temp=pre;
        //         else
        //             return 0;
        //     }
        //     else if(s[i-1]=='1' || (s[i-1] == '2' && s[i]>='1' && s[i]<='6'))
        //         temp+=pre;
            
        //     pre=cur;
        //     cur=temp;
        // }
        // return cur;

        //like climb stairs, we can decode one char or two chars, then add them.
        int len=s.Length;
        int[] dp=new int[len+1];
        //base case
        dp[0]=1; // to get the right dp[2], let's dp[0]=1
        dp[1]=s[0]=='0'?0:1;
        for(int i=2; i<=len; i++){
            int first=int.Parse(s.Substring(i-1,1));
            int second=int.Parse(s.Substring(i-2,2));
            if(first!=0)
                dp[i]+=dp[i-1];
            if(second>=10 && second <=26)
                dp[i]+=dp[i-2];
        }
        return dp[len];
    }
}
// @lc code=end

