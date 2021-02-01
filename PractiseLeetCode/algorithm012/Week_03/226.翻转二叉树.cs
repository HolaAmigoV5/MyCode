/*
 * @lc app=leetcode.cn id=226 lang=csharp
 *
 * [226] 翻转二叉树
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
    public TreeNode InvertTree(TreeNode root) {
        if(root==null)
            return null;

        //DFS
        // var tmp=root.right;
        // root.right=root.left;
        // root.left=tmp;

        // InvertTree(root.left);
        // InvertTree(root.right);

        // return root;

        //BFS
        var queue=new Queue<TreeNode>();
        queue.Enqueue(root);
        while(queue.Any()){
            var tmp=queue.Dequeue();
            var left=tmp.left;
            tmp.left=tmp.right;
            tmp.right=left;

            if(tmp.left!=null)
                queue.Enqueue(tmp.left);
            if(tmp.right!=null)
                queue.Enqueue(tmp.right);
        }
        return root;
    }
}
// @lc code=end

