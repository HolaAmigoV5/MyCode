/*
 * @lc app=leetcode.cn id=1091 lang=csharp
 *
 * [1091] 二进制矩阵中的最短路径
 */

// @lc code=start
public class Solution {
    public int ShortestPathBinaryMatrix(int[][] grid)
    {
        if (grid == null || grid.Length == 0 || grid[0][0] == 1 || grid[^1][^1] == 1)
            return -1;

        int[,] dir = { { 0, 1 }, { 1, 0 }, { 0, -1 }, { -1, 0 }, { 1, 1 }, { 1, -1 }, { -1, -1 }, { -1, 1 } };
        int count = 0, m = grid.Length;

        var queue = new Queue<int[]>();
        queue.Enqueue(new int[] { 0, 0 });
        while (queue.Count > 0)
        {
            count++;
            var size = queue.Count;
            while (size-- > 0)
            {
                var p = queue.Dequeue();
                if (p[0] == m - 1 && p[1] == m - 1)
                    return count;

                for (int i = 0; i < 8; i++)
                {
                    int x = p[0] + dir[i, 0];
                    int y = p[1] + dir[i, 1];

                    if (x >= 0 && x < m && y >= 0 && y < m && grid[x][y] == 0)
                    {
                        grid[x][y] = 1;
                        queue.Enqueue(new int[] { x, y });
                    }
                }
            }
        }
        return -1;
    }
}
// @lc code=end

