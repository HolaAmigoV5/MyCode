/*
 * @lc app=leetcode.cn id=144 lang=csharp
 *
 * [144] 二叉树的前序遍历
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
    public IList<int> PreorderTraversal(TreeNode root) {
        //Recursion. root-left-right
        if(root==null) return new List<int>();
        // var nes=new List<int>();
        // Preorder(root,nes);
        // return nes;

        //M2:Iteration
        Stack<TreeNode> stack=new Stack<TreeNode> ();
        List<int> res=new List<int>();

        stack.Push(root);
        while(stack.Any()){
            var node=stack.Pop();
            res.Add(node.val);
            if(node.right!=null)
                stack.Push(node.right);
            if(node.left!=null)
                stack.Push(node.left);
        }
        return res;
    }

    // private void Preorder(TreeNode node, List<int> res){
    //     if(node==null) return;
    //     res.Add(node.val);
    //     Preorder(node.left,res);
    //     Preorder(node.right,res);
    // }
}
// @lc code=end

