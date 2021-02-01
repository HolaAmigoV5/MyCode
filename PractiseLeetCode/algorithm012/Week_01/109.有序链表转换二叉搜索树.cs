/*
 * @lc app=leetcode.cn id=109 lang=csharp
 *
 * [109] 有序链表转换二叉搜索树
 */

// @lc code=start
/**
 * Definition for singly-linked list.
 * public class ListNode {
 *     public int val;
 *     public ListNode next;
 *     public ListNode(int val=0, ListNode next=null) {
 *         this.val = val;
 *         this.next = next;
 *     }
 * }
 */
/**
 * Definition for a binary tree node.
 * public class TreeNode {
 *     public int val;
 *     public TreeNode left;
 *     public TreeNode right;
 *     public TreeNode(int val=0, TreeNode left=null, TreeNode right=null) {
 *         this.val = val;
 *         this.left = left;
 *         this.right = right;
 *     }
 * }
 */
public class Solution {
    public TreeNode SortedListToBST(ListNode head) {
        if(head==null)
            return null;

        // var list=new List<int>();
        // while(head!=null){
        //     list.Add(head.val);
        //     head=head.next;
        // }

        // return ToBST(list, 0, list.Count-1);

        //fast and slow pointers
        return BuildTree(head, null);
    }

    private TreeNode BuildTree(ListNode head, ListNode tail){
        if(head==tail)
            return null;
        ListNode slow=head;
        ListNode fast=head;
        while(fast!=tail && fast.next!=tail){
            slow=slow.next;
            fast=fast.next.next;
        }

        TreeNode root=new TreeNode(slow.val);
        root.left=BuildTree(head, slow);
        root.right=BuildTree(slow.next, tail);
        return root;
    }

    private TreeNode ToBST(List<int> list, int start, int end){
        if(start>end)
            return null;
        var mid=start+((end-start)>>1);
        TreeNode root=new TreeNode(list[mid]);
        root.left=ToBST(list, start, mid-1);
        root.right=ToBST(list, mid+1, end);
        return root;
    }
}
// @lc code=end

