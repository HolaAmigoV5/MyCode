/*
 * @lc app=leetcode.cn id=5 lang=csharp
 *
 * [5] 最长回文子串
 */

// @lc code=start
public class Solution {
    public string LongestPalindrome(string s) {
        int len=s.Length;
        if(len<2) return s;

        //int begin=0, maxLen=1;
        //M1:brute force
        // for(int i=0; i<len; i++){
        //     for(int j=i+1; j<len; j++){
        //         if(IsPalindrome(s, i,j) && j-i+1>maxLen){
        //             maxLen=j-i+1;
        //             begin=i;
        //         }
        //     }
        // }
        // return s.Substring(begin,maxLen);


        //M2: dp
        // bool[,] dp=new bool[len,len];
        // for(int i=len-1; i>=0; i--){
        //     for(int j=0; j<len; j++){
        //         dp[i,j]=s[i]==s[j] && (j-i<3 || dp[i+1, j-1]);
        //         if(dp[i,j]){
        //             if(j-i+1>maxLen){
        //                 maxLen=j-i+1;
        //                 begin=i;
        //             }
        //         }
        //     }
        // }
        // return s.Substring(begin,maxLen);

        //M3: extend
        for(int i=0; i<len-1; i++){
            Extend(s,i,i); //assume odd length, try to extend Palindrome as possible
            Extend(s,i,i+1); //assum even length
        }
        return s.Substring(begin,maxLen);
    }

    int begin=0, maxLen=1;
    private void Extend(string s, int i, int j){
        while(i>=0 && j<s.Length && s[i]==s[j]){
            i--;
            j++;
        }
        if(j-i-1>maxLen){
            begin=i+1;
            maxLen=j-i-1;
        }
    }
    private bool IsPalindrome(string s, int start, int end){
        while(start<end){
            if(s[start++]!=s[end--])
                return false;
        }
        return true;
    }
}
// @lc code=end

