/*
 * @lc app=leetcode.cn id=433 lang=csharp
 *
 * [433] 最小基因变化
 */

// @lc code=start
public class Solution {
    public int MinMutation(string start, string end, string[] bank) {
        //two-ended BFS
        var bankSet=bank.ToHashSet<string>();
        if(bankSet.Count==0 || !bankSet.Contains(end))
            return -1;
        
        var startVisited=new HashSet<string>(){start};
        var endVisited=new HashSet<string>(){end};
        var visited=new HashSet<string>();

        int count=0;
        while(startVisited.Any()){
            count++;
            //switch
            if(startVisited.Count<endVisited.Count){
                var tmp=startVisited;
                startVisited=endVisited;
                endVisited=tmp;
            }

            var nextVisited=new HashSet<string>();
            foreach(var str in startVisited){
                var neighbors=GetNeighbors(str, bankSet);
                foreach(var n in neighbors){
                    if(endVisited.Contains(n))
                        return count;
                    if(!visited.Contains(n)){
                        nextVisited.Add(n);
                    }
                }
            }
            visited.UnionWith(nextVisited);
            startVisited=nextVisited;
        }
        return -1;
    }

    private List<string> GetNeighbors(string str, HashSet<string> bankSet){
        var list=new List<string>();
        if(string.IsNullOrEmpty(str))
            return list;
        var chars=str.ToCharArray();
        var gen="ACGT";
        for(int i=0; i<str.Length; i++){
            var old=chars[i];
            foreach(var ch in gen){
                if(ch==old)
                    continue;
                chars[i]=ch;
                var newstr=new string(chars);
                if(bankSet.Contains(newstr))
                    list.Add(newstr);
            }

            chars[i]=old;
        }
        return list;
    }
}
// @lc code=end

