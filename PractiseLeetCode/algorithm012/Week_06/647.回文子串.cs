/*
 * @lc app=leetcode.cn id=647 lang=csharp
 *
 * [647] 回文子串
 */

// @lc code=start
public class Solution {
    public int CountSubstrings(string s) {
        if(string.IsNullOrEmpty(s) || s.Length<2)
            return s.Length;
        
        int len=s.Length;
        

        //M1:Brute force
        // int count=len;
        // for(int i=0; i<len; i++){
        //     for(int j=i+1; j<len; j++){
        //         if(IsPalindrome(s,i,j))
        //             count++;
        //     }
        // }
        // return count;

        //M2: dp, dp[i,j] means whether the substring s[i,j] is palindrome
        int count=0;
        bool[,] dp=new bool[len, len];
        for(int i=len-1; i>=0; i--){
            for(int j=i; j<len; j++){
                dp[i,j]=s[i]==s[j] && (j-i<3 || dp[i+1, j-1]);
                if(dp[i,j])
                    count++;
            }
        }
        return count;
       
       //M3: Extend
       for(int i=0; i<len; i++){
           Extend(s, i,i);
           Extend(s, i, i+1);
       }
       return count;
    }

    int count=0;
    private void Extend(string s, int left, int right){
        while(left>=0 && right<s.Length && s[left]==s[right]){
            count++;
            left--;
            right++;
        }
    }

    private bool IsPalindrome(string str,int start, int end){
        while(start<end){
            if(str[start++]!=str[end--])
                return false;
        }
        return true;
    }
}
// @lc code=end

