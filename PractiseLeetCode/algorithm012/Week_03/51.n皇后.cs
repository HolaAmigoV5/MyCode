/*
 * @lc app=leetcode.cn id=51 lang=csharp
 *
 * [51] N皇后
 */

// @lc code=start
public class Solution {
    List<IList<string>> output=new List<IList<string>>();
    public IList<IList<string>> SolveNQueens(int n) {
        //build chessboard
        char[][] chessboard=new char[n][];
        for(int i=0; i<n; i++){
            chessboard[i]=new char[n];
            for(int j=0; j<n;j++){
                chessboard[i][j]='.';
            }
        } 

        PlaceQueens(chessboard,0);
        return output;
    }

    private void PlaceQueens(char[][] chess, int row){
        //output
        if(row==chess.Length){
            output.Add(CharToStringList(chess));
            return;
        }

        for(int col=0; col<chess.Length; col++){
            if(IsValid(chess,col, row)){
                chess[row][col]='Q';
                PlaceQueens(chess,row+1);
                chess[row][col]='.';
            }
        }

    }

    private bool IsValid(char[][] chess, int col, int row){
        //check the up
        for(int i=0; i<row; i++){
            if(chess[i][col]=='Q')
                return false;
        }

        //check the left-up
        for(int i=row-1, j=col-1; i>=0 && j>=0; i--,j-- ){
            if(chess[i][j]=='Q')
                return false;
        }

        //check the right-up
        for(int i=row-1, j=col+1; i>=0 && j<chess.Length; i--,j++){
            if(chess[i][j]=='Q')
                return false;
        }

        return true;
    }

    private IList<string> CharToStringList(char[][] chess){
        var path=new List<string>();
        foreach(char[] chars in chess)
            path.Add(new string(chars));

        return path;
    }
}
// @lc code=end

