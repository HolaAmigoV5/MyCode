/*
 * @lc app=leetcode.cn id=74 lang=csharp
 *
 * [74] 搜索二维矩阵
 */

// @lc code=start
public class Solution {
    public bool SearchMatrix(int[][] matrix, int target) {
        //Binary Search. O(log m*n), O(1)
        int m=matrix.Length;
        if(m==0)
            return false;
        int n=matrix[0].Length;

        int left=0, right=m*n-1,pivotIdex=0, pivotElement=0;
        while (left<=right)
        {
            pivotIdex=(left+right)/2;
            pivotElement=matrix[pivotIdex/n][pivotIdex%n];
            if(pivotElement==target)
                return true;
            if(pivotElement>target){
                right=pivotIdex-1;
            }
            else
                left=pivotIdex+1;
        }
        return false;
    }
}
// @lc code=end

