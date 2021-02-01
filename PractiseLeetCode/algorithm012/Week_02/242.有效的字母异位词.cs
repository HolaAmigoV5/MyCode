/*
 * @lc app=leetcode.cn id=242 lang=csharp
 *
 * [242] 有效的字母异位词
 */

// @lc code=start
public class Solution {
    public bool IsAnagram(string s, string t) {
        //M1 call sort
        if(s==null || t==null) return false;
        if(s.Length!=t.Length) return false;
        // char[] schar=s.ToCharArray();
        // char[] tchar=t.ToCharArray();
        // Array.Sort(schar);
        // Array.Sort(tchar);
        // return new string(schar)==new string(tchar);

        //M2 
        // int [] arr=new int[26];
        // for(int i=0; i<s.Length; i++){
        //     arr[s[i]-'a']++;
        //     arr[t[i]-'a']--;
        // }
        // return !arr.Any(item=>item!=0);

        //M3: use dictionary. the best
        var dic=new Dictionary<char,int>();
        foreach(char sc in s)
        {
            if(dic.ContainsKey(sc))
                dic[sc]++;
            else
                dic[sc]=1;
        }
        foreach(char tc in t)
        {
            if(dic.ContainsKey(tc)){
                if(dic[tc]>1)
                    dic[tc]--;
                else
                    dic.Remove(tc);
            }
            else
                return false;
        }
        return true;
    }
}
// @lc code=end

