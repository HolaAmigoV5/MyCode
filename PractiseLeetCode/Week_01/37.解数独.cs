/*
 * @lc app=leetcode.cn id=37 lang=csharp
 *
 * [37] 解数独
 */

// @lc code=start
public class Solution {
    public void SolveSudoku(char[][] board) {
        if(board==null || board.Length==0)
            return;
        DoSolve(board,0,0);
    }

    private bool DoSolve(char[][] board, int row, int col){
        int m=9, n=9;
        if(col==n) //reach the last column, next row
            return DoSolve(board, row+1, 0);
        if(row==m) //reach the last row, finish
            return true;
        if(board[row][col]!='.') //skip
            return DoSolve(board, row, col+1);
        
        for(char ch='1'; ch<='9'; ch++){
            if(IsValid(board,row, col, ch)){
                board[row][col]=ch;
                if(DoSolve(board, row, col+1))
                    return true;
                board[row][col]='.';
            }
        }
        return false;
    }

    private bool IsValid(char[][] board, int row, int col, char c){
        //row/3)*3，(col/3)*3分别为box第一个格子坐标
        for(int i=0; i<9; i++){
            if(board[i][col]==c || board[row][i]==c ||
                board[(row/3)*3+i/3][(col/3)*3+i%3]==c)
                    return false;
        }
        return true;
    }
}
// @lc code=end

