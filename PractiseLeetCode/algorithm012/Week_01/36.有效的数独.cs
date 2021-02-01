/*
 * @lc app=leetcode.cn id=36 lang=csharp
 *
 * [36] 有效的数独
 */

// @lc code=start
public class Solution {
    public bool IsValidSudoku(char[][] board) {
        //M1:
        //row[i,x]:i行数字x的个数， col[j,x]:j列数字x的个数，box[k,x]:宫格中x的个数
        // int[,] row=new int[9,10], col=new int[9,10], box=new int[9,10];
        // for(int i=0; i<9; i++){
        //     for(int j=0; j<9; j++){
        //         int x=board[i][j]-'0';
        //         if(x>=1 && x<=9){
        //             row[i,x]++;
        //             col[j,x]++;
        //             box[i/3*3+j/3,x]++;
        //             if(row[i,x]>1 || col[j,x]>1 || box[i/3*3+j/3,x]>1)
        //                 return false;
        //         }
        //     }
        // }
        // return true;

        //M2
        var seen=new HashSet<string>();
        for(int i=0; i<9; i++){
            for(int j=0; j<9; j++){
                char number=board[i][j];
                if(number!='.'){
                    if(!seen.Add(number+"in row"+i) ||
                    !seen.Add(number+"in col"+j)||
                    !seen.Add(number+"in box"+i/3+"-"+j/3))
                        return false;
                }
            }
        }
        return true;
    }
}
// @lc code=end

