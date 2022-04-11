/*
 * @lc app=leetcode.cn id=105 lang=csharp
 *
 * [105] 从前序与中序遍历序列构造二叉树
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
    Dictionary<int, int> dic=null;
    int[] the_preorder=null;
    int[] the_inorder=null;
    public TreeNode BuildTree(int[] preorder, int[] inorder) {
        
        //Recursion
        //prepare the data
        the_inorder=inorder;
        the_preorder=preorder;
        dic=new Dictionary<int, int>();
        for(int i=0; i<inorder.Length; i++){
            dic[inorder[i]]=i;
        }

        return BuildTreeHelper(0,0,inorder.Length-1);
    }

    private TreeNode BuildTreeHelper(int p_start,int i_start, int i_end){
        if(p_start>the_preorder.Length-1 || i_start>i_end)
            return null;
        var root=new TreeNode(the_preorder[p_start]);
        var root_index=dic[root.val];
        var len=root_index-i_start;
        root.left=BuildTreeHelper(p_start+1,i_start, root_index-1);
        root.right=BuildTreeHelper(p_start+len+1, root_index+1, i_end);
        return root;
    }

    
}
// @lc code=end

