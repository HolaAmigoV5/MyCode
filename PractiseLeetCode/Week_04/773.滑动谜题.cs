/*
 * @lc app=leetcode.cn id=773 lang=csharp
 *
 * [773] 滑动谜题
 */

// @lc code=start
public class Solution {
    public int SlidingPuzzle(int[][] board) {
        //define the start and target
        string target="123450";
        string start=string.Empty;
        foreach(var arr in board){
            foreach(var item in arr)
                start+=item;
        }

        if(start.Equals(target))
            return 0;
        
        //begin to BFS
        int count=0;
        var visited=new HashSet<string>(){start};
        //all the positions 0 can be swapped to 
        int[][] dirs=new int[][]{
            new int[]{1,3}, 
            new int[]{0,2,4}, 
            new int[]{1,5}, 
            new int[]{0,4}, 
            new int[]{1,3,5}, 
            new int[]{2,4}
        };

        var queue=new Queue<string>();
        queue.Enqueue(start);
        while(queue.Count>0){
            var size=queue.Count;
            count++;
            while(size-->0){
                var cur=queue.Dequeue();
                if(cur==target)
                    return count-1; //最后一步不算，多算了一步，故-1
                int zero=cur.IndexOf('0');
                foreach(var dir in dirs[zero]){
                    var next=Swap(cur.ToCharArray(), dir, zero);
                    if(!visited.Contains(next)){
                        visited.Add(next);
                        queue.Enqueue(next);
                    }
                }
            }
        }
        return -1;
    }

    private string Swap(char[] chars, int newPos, int zero){
        if(zero!=newPos){
            var tmp=chars[zero];
            chars[zero]=chars[newPos];
            chars[newPos]=tmp;
        }
        return new string(chars);
    }
}
// @lc code=end

