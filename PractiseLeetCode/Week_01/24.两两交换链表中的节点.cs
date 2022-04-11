/*
 * @lc app=leetcode.cn id=24 lang=csharp
 *
 * [24] 两两交换链表中的节点
 */

// @lc code=start
/**
 * Definition for singly-linked list.
 * public class ListNode {
 *     public int val;
 *     public ListNode next;
 *     public ListNode(int x) { val = x; }
 * }
 */
public class Solution {
    public ListNode SwapPairs(ListNode head) {
        //Recursion
        // if(head==null || head.next==null)
        //     return head;
        // var next=head.next;
        // head.next=SwapPairs(next.next);
        // next.next=head;
        // return next;

        //Iteration
        ListNode dummy=new ListNode(0);
        dummy.next=head;
        ListNode pre=dummy;
        while(head!=null && head.next!=null){
            var sec=head.next;
            head.next=sec.next;
            sec.next=head;
            pre.next=sec;
            
            pre=head;
            head=head.next;
        }
        return dummy.next;
    }
}
// @lc code=end

