/*
 * @lc app=leetcode.cn id=94 lang=csharp
 *
 * [94] 二叉树的中序遍历
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
     IList<int> list=new List<int>();
    public IList<int> InorderTraversal(TreeNode root) {
        //M1: Recursion. left-root-right
        if(root==null) return list;

        // InorderTraversal(root.left);
        // list.Add(root.val);
        // InorderTraversal(root.right);

        // return list;

        //M2:Iteration
        Stack<TreeNode> stack=new Stack<TreeNode>();
        while (root != null || stack.Any())
        {
            //All the left nodes push into the stack.
            while (root != null)
            {
                stack.Push(root);
                root = root.left;
            }
            var temp = stack.Pop();
            list.Add(temp.val);
            root = temp.right;
        }
        return list;

        //M3:Iteartion, right-root-left
        // var stack=new Stack<TreeNode>();
        // stack.Push(root);
        // while(stack.Any()){
        //     var node=stack.Pop();
        //     if(node!=null){
        //         if(node.right!=null)
        //             stack.Push(node.right);
        //         stack.Push(node);
        //         stack.Push(null);
        //         if(node.left!=null)
        //             stack.Push(node.left);
        //     }
        //     else
        //         list.Add(stack.Pop().val);
        // }
        // return list;
    }
}
// @lc code=end

