/*
 * @lc app=leetcode.cn id=557 lang=csharp
 *
 * [557] 反转字符串中的单词 III
 */

// @lc code=start
public class Solution {
    public string ReverseWords(string s) {
         if(string.IsNullOrEmpty(s))
            return s;

        //M1:Split() then reverse
        // string[] st=s.Split(' ');
        // StringBuilder sb=new StringBuilder();
        // for(int i=0; i<st.Length; i++){
        //     char[] arr=st[i].ToCharArray();
        //     Array.Reverse(arr);
        //     sb.Append(new string(arr)+" ");
        // }
        // return sb.ToString().Trim();

        
        //split then reverse
        //string[] strs=s.Split(' ');
        // var sb=new StringBuilder();
        // foreach(string str in strs)
        //     sb.Append(Reverse(str)+" ");
        // return sb.ToString().Trim();

        //M3:
        char[] arr=s.ToCharArray();
        for(int i=0; i<arr.Length; i++){
            if(arr[i]!=' '){  //when is is non-space
                int j=i;
                while(j+1<arr.Length && arr[j+1]!=' ')  //move j to the end of the word.
                    j++;
                Reverse2(arr, i,j);
                i=j;
            }
        }
        return new string(arr);
    }

    private string Reverse(string str){
        if(string.IsNullOrEmpty(str) || str.Length==1)
            return str;
        
        int left=0, right=str.Length-1;
        char[] arr=str.ToCharArray();

        while(left<right){
            var tmp=arr[left];
            arr[left++]=arr[right];
            arr[right--]=tmp;
        }

        return new string(arr);
    }

    private void Reverse2(char[] arr, int start, int end){
        while(start<end){
            var tmp=arr[start];
            arr[start++]=arr[end];
            arr[end--]=tmp;
        }
    }
}
// @lc code=end

