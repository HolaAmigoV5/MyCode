/*
 * @lc app=leetcode.cn id=98 lang=csharp
 *
 * [98] 验证二叉搜索树
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
    TreeNode pre;
    public bool IsValidBST(TreeNode root) {
        if(root==null) return true;

        //InOrder:left-root-right
        // if(!IsValidBST(root.left))
        //     return false;
        // if(pre!=null && pre.val>=root.val)
        //     return false;
        // pre=root;
        // return IsValidBST(root.right);

        //M2:Iterator
        // var stack=new Stack<TreeNode>();
        // while(root!=null || stack.Count>0){
        //     //push all the left to stack
        //     while(root!=null){
        //         stack.Push(root);
        //         root=root.left;
        //     }
        //     var node=stack.Pop();
        //     if(pre!=null && pre.val>=node.val)
        //         return false;
        //     pre=node;
        //     root=node.right;
        // }
        // return true;

        //M3:Iterator2
        var stack=new Stack<TreeNode>();
        stack.Push(root);
        while(stack.Count>0){
            var node=stack.Pop();
            if(node!=null){
                //right-root-left
                if(node.right!=null)
                    stack.Push(node.right);
                stack.Push(node);
                stack.Push(null);
                if(node.left!=null)
                    stack.Push(node.left);
            }
            else{
                var tmp=stack.Pop();
                if(pre!=null && pre.val>=tmp.val)
                    return false;
                pre=tmp;
            }
        }
        return true;
    }
}
// @lc code=end

