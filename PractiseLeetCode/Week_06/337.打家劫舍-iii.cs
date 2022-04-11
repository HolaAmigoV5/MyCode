/*
 * @lc app=leetcode.cn id=337 lang=csharp
 *
 * [337] 打家劫舍 III
 */

// @lc code=start
/**
 * Definition for a binary tree node.
 * public class TreeNode {
 *     public int val;
 *     public TreeNode left;
 *     public TreeNode right;
 *     public TreeNode(int x) { val = x; }
 * }
 */
public class Solution {
    public int Rob(TreeNode root) {
        if(root==null)
            return 0;

        /*任何一个节点能偷到的最大钱的状态可以定义为：
        当前节点不偷：MaxMoney = 左孩子能偷到的钱 + 右孩子能偷到的钱
        当前节点偷：MaxMoney = 左孩子自己不偷时能得到的钱 + 右孩子不偷时能得到的钱 + 当前节点的钱数*/

        int[] result=RobInternal(root);
        return Math.Max(result[0], result[1]);  //0 means no robbery, 1 means robbery
    }

    private int[] RobInternal(TreeNode root){
        int[] result=new int[2];
        if(root==null)
            return result;
        
        int[] left=RobInternal(root.left);
        int[] right=RobInternal(root.right);

        result[0]=Math.Max(left[0], left[1])+Math.Max(right[0], right[1]);
        result[1]=left[0]+right[0]+root.val;

        return result;
    }
}
// @lc code=end

