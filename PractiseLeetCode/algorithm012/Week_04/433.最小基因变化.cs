/*
 * @lc app=leetcode.cn id=433 lang=csharp
 *
 * [433] 最小基因变化
 */

// @lc code=start
public class Solution {
    public int MinMutation(string start, string end, string[] bank) {
        var bankSet=new HashSet<string>(bank);
        if(string.IsNullOrEmpty(start)|| string.IsNullOrEmpty(end)|| !bankSet.Contains(end))
            return -1;
        
        //BFS
        var queue=new Queue<string>();
        queue.Enqueue(start);
        int count=0;
        while(queue.Any()){
            count++;
            var size=queue.Count;
            while(size-->0){
                var str=queue.Dequeue();
                var neighbors=GetNeighbors(str, bankSet);
                foreach (var item in neighbors)
                {
                    if(item.Equals(end))
                        return count;
                    queue.Enqueue(item);
                }
            }
        }
        return -1;
    }

    private List<string> GetNeighbors(string str, HashSet<string> bankSet){
        var ans=new List<string>();
        var chars=str.ToCharArray();
        string cItem="ACGT";
        for(int i=0; i<str.Length; i++){
            var oldchar=chars[i];
            foreach (var ch in cItem)
            {
                if(ch==oldchar)
                    continue;
                chars[i]=ch;
                var newStr=new string(chars);
                if(bankSet.Remove(newStr))
                    ans.Add(newStr);
            }
            chars[i]=oldchar;
        }
        return ans;
    }
}
// @lc code=end

