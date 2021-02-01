/*
 * @lc app=leetcode.cn id=773 lang=csharp
 *
 * [773] 滑动谜题
 */

// @lc code=start
public class Solution {
    public int SlidingPuzzle(int[][] board) {
        string target="123450";
        string start=string.Empty;

        int m=board.Length, n=board[0].Length;
        for(int i=0; i<m; i++){
            for(int j=0; j<n; j++){
                start+=board[i][j];
            }
        }

        if(target==start)
            return 0;
        
        //BFS
        int count=0;
        int[][] map={
            new int[]{1,3}, new int[]{0,2,4}, new int[]{1,5},
            new int[]{0,4}, new int[]{1,3,5}, new int[]{2,4}
        };

        var queue=new Queue<string>();
        queue.Enqueue(start);
        var visited=new HashSet<string>(){start};

        while(queue.Count>0){
            count++;
            var size=queue.Count;
            while(size-->0){
                var cur=queue.Dequeue();
                if(cur==target)
                    return count-1;
                
                var zero=cur.IndexOf('0');
                foreach(var pos in map[zero]){
                    var next=Swap(cur.ToCharArray(), pos, zero);
                    if(!visited.Contains(next)){
                        visited.Add(next);
                        queue.Enqueue(next);
                    }
                }
            }
        }
        return -1;
    }

    private string Swap(char[] chars, int nextPos, int zeroPos){
        var tmp=chars[zeroPos];
        chars[zeroPos]=chars[nextPos];
        chars[nextPos]=tmp;

        return new string(chars);
    }
}
// @lc code=end

