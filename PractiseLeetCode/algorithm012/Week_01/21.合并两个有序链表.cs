/*
 * @lc app=leetcode.cn id=21 lang=csharp
 *
 * [21] 合并两个有序链表
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
public class Solution {
    public ListNode MergeTwoLists(ListNode l1, ListNode l2) {
        // if(l1==null)
        //     return l2;
        // if(l2==null)
        //     return l1;
        // if(l1.val<l2.val){
        //     l1.next=MergeTwoLists(l1.next,l2);
        //     return l1;
        // }
        // else{
        //     l2.next=MergeTwoLists(l1,l2.next);
        //     return l2;
        // }

        //M2:O(m+n), O(1)
        ListNode prehead = new ListNode(-1);
        ListNode prev = prehead;
        while (l1 != null && l2 != null)
        {
            if (l1.val <= l2.val)
            {
                prev.next = l1;
                l1 = l1.next;
            }
            else
            {
                prev.next = l2;
                l2 = l2.next;
            }
            prev = prev.next;
        }
        prev.next = l1 == null ? l2 : l1;
        return prehead.next;
    }
}
// @lc code=end

