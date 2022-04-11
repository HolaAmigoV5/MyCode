/*
 * @lc app=leetcode.cn id=145 lang=csharp
 *
 * [145] 二叉树的后序遍历
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
    public IList<int> PostorderTraversal(TreeNode root) {
        //M1:Recursion. left-right-root
        List<int> res=new List<int>();
        // Postorder(root,res);
        // return res;

        //M2:Iteration
        if (root == null) return res;
        Stack<TreeNode> stackIn = new Stack<TreeNode>();
        Stack<TreeNode> stackOut = new Stack<TreeNode>();

        stackIn.Push(root);
        while (stackIn.Any())
        {
            var curNode = stackIn.Pop();
            stackOut.Push(curNode);

            if (curNode.left != null)
                stackIn.Push(curNode.left);

            if (curNode.right != null)
                stackIn.Push(curNode.right);
        }

        while (stackOut.Any())
        {
            var outNode = stackOut.Pop();
            res.Add(outNode.val);
        }

        return res;
    }

    private void Postorder(TreeNode node, List<int> res){
        if(node==null) return;
        Postorder(node.left, res);
        Postorder(node.right, res);
        res.Add(node.val);
    }
}
// @lc code=end

