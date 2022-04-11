/*
 * @lc app=leetcode.cn id=141 lang=csharp
 *
 * [141] 环形链表
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
    public bool HasCycle(ListNode head) {
        if(head==null || head.next==null)
            return false;
        //use list.O(n), O(n)
        // var list=new List<ListNode>();
        // while(head!=null){
        //     if(list.Contains(head))
        //         return true;
        //     else
        //         list.Add(head);
        //     head=head.next;
        // }
        // return false;

        //use double pointers.O(n), O(1)
        ListNode slow=head;
        ListNode fast=head.next;
        while(slow!=fast){
            if(fast==null || fast.next==null)
                return false;
            slow=slow.next;
            fast=fast.next.next;
        }
        return true;
    }
}
// @lc code=end

