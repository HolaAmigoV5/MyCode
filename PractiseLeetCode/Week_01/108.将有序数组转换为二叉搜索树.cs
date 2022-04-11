/*
 * @lc app=leetcode.cn id=108 lang=csharp
 *
 * [108] 将有序数组转换为二叉搜索树
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
    public TreeNode SortedArrayToBST(int[] nums) {
        if(nums==null || nums.Length==0)
            return null;
        
        return ToBST(nums,0,nums.Length-1);
    }

    private TreeNode ToBST(int[] nums, int start, int end){
        if(start>end)
            return null;
        var mid=(start+end)>>1;
        TreeNode root=new TreeNode(nums[mid]);
        root.left=ToBST(nums, start, mid-1);
        root.right=ToBST(nums, mid+1, end);
        return root;
    }
}
// @lc code=end

