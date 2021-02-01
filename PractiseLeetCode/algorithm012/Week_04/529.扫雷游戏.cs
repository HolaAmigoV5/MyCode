/*
 * @lc app=leetcode.cn id=529 lang=csharp
 *
 * [529] 扫雷游戏
 */

// @lc code=start
public class Solution {
    //上下左右，左上，左下，右上，右下
    int[] dx = { 0, 0, -1, 1, -1, -1, 1, 1 };
    int[] dy = { 1, -1, 0, 0, 1, -1, 1, -1 };
    int row = 0, col = 0;
    public char[][] UpdateBoard(char[][] board, int[] click)
    {
        row = board.Length;
        col = board[0].Length;
        MinesDFS(board, click[0], click[1]);
        return board;
    }

    private void MinesDFS(char[][] board, int x, int y)
    {
        if (x < 0 || x >= row || y < 0 || y >= col)
            return;
        if (board[x][y] == 'E')
        {
            //E to B 
            board[x][y] = 'B';

             //count the number of mines in eight directions
            int count = Judge(board, x, y);
            if (count == 0)
            {
                //如果为0则进行递归
                for (int i = 0; i < 8; i++)
                    MinesDFS(board, x + dx[i], y + dy[i]);
            }
            else
                board[x][y] = (char)(count + '0');

        }
        else if (board[x][y] == 'M')
        {
            //this is a mine. Go die
            board[x][y] = 'X';
        }
    }

    private int Judge(char[][] board, int x, int y)
    {
        int count = 0;
        for (int i = 0; i < 8; i++)
        {
            int newX = x + dx[i];
            int newY = y + dy[i];
            if (newX < 0 || newX >= row || newY < 0 || newY >= col)
                continue;
            if (board[newX][newY] == 'M')
                count++;
        }
        return count;
    }
}
// @lc code=end

