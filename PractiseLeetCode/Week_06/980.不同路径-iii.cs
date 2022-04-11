/*
 * @lc app=leetcode.cn id=980 lang=csharp
 *
 * [980] 不同路径 III
 */

// @lc code=start
public class Solution {
    int res=0, empty=1, sx,sy;
    public int UniquePathsIII(int[][] grid) {
        int m=grid.Length;
        int n=grid[0].Length;
        for(int i=0; i<m; i++){
            for(int j=0; j<n; j++){
                if(grid[i][j]==0)
                    empty++;
                else if(grid[i][j]==1){
                    sx=i; sy=j;
                }
            }
        }
        DFS(grid, sx, sy);
        return res;
    }
   
   private void DFS(int[][] grid, int x, int y){
        if(x<0 || x>=grid.Length || y<0 || y>=grid[0].Length || grid[x][y]<0)
            return;
        if(grid[x][y]==2){
            if(empty==0)
                res++;
            return;
        }

        grid[x][y]=-2;
        empty--;
        for(int i=0; i<4; i++)
            DFS(grid, x+(i==1?1:i==3?-1:0), y+(i==0?1:i==2?-1:0));
        grid[x][y]=0;
        empty++;
   }
}
// @lc code=end

