/*
 * @lc app=leetcode.cn id=941 lang=csharp
 *
 * [941] 有效的山脉数组
 */

// @lc code=start
public class Solution {
    public bool ValidMountainArray(int[] A) {
        if(A.Length<3)
            return false;

        int left=0, right=A.Length-1;
        while (left < A.Length - 2 && A[left] < A[left + 1])
        {
            left++;
        }
        while (right>1 && A[right]<A[right-1])
        {
            right--;
        }

        return left==right;
    }
}
// @lc code=end

