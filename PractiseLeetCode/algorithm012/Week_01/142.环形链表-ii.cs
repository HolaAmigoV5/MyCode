/*
 * @lc app=leetcode.cn id=142 lang=csharp
 *
 * [142] 环形链表 II
 */

// @lc code=start
/**
 * Definition for singly-linked list.
 * public class ListNode {
 *     public int val;
 *     public ListNode next;
 *     public ListNode(int x) {
 *         val = x;
 *         next = null;
 *     }
 * }
 */
public class Solution {
    public ListNode DetectCycle(ListNode head) {
        //verification
        if(head==null || head.next==null)
            return null;

        //M1:use List<T>. O(n), O(n)
        // List<ListNode> list=new List<ListNode>();
        // while(head!=null){
        //     if(list.Contains(head))
        //         return head;
        //     else
        //         list.Add(head);
        //     head=head.next;
        // }

        // return null;

        //use double pointers. O(n), O(1)
        ListNode slow=head;
        ListNode fast=head;
        //first meeting
        while (true)
        {
            if(fast==null || fast.next==null)
                return null;
            slow=slow.next;
            fast=fast.next.next;
            if(slow==fast)
                break;
        }

        //second meeting
        fast=head;
        while(fast!=slow){
            fast=fast.next;
            slow=slow.next;
        }
        return fast;
    }
}
// @lc code=end

