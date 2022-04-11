/*
 * @lc app=leetcode.cn id=206 lang=csharp
 *
 * [206] 反转链表
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
    public ListNode ReverseList(ListNode head) {
       //verification
        if(head==null || head.next==null)
            return head;
        
        //M1: use Stack
        // ListNode theHead=head;
        // Stack<int> stack=new Stack<int>();
        // while(head!=null){
        //     stack.Push(head.val);
        //     head=head.next;
        // }
        // head=theHead;
        // while(stack.Any() && head!=null){
        //     head.val=stack.Pop();
        //     head=head.next;
        // }
        // return theHead;

        //M2: use Stack
        // var stack=new Stack<int>();
        // while(head!=null){
        //     stack.Push(head.val);
        //     head=head.next;
        // }

        // ListNode pre=new ListNode(0);
        // ListNode theHead=pre;
        // while(stack.Any()){
        //     pre.next=new ListNode(stack.Pop());
        //     pre=pre.next;
        // }
        // return theHead.next;

        //M3:double pointers
        // ListNode pre=null;
        // ListNode cur=head;
        // while(head!=null){
        //     cur=head.next;
        //     head.next=pre;
        //     pre=head;
        //     head=cur;
        // }
        // return pre;

        //M3-2:
        // ListNode newhead=null;
        // while(head!=null){
        //     var next=head.next;
        //     head.next=newhead;
        //     newhead=head;
        //     head=next;
        // }
        // return newhead;

        //M4:Recursion
        // var newhead=ReverseList(head.next);
        // head.next.next=head;
        // head.next=null;
        // return newhead;

        //M4:Recursion-2
        return ReverseLinkedList(head,null);
    }

    private ListNode ReverseLinkedList(ListNode cur, ListNode pre){
        if(cur==null)
            return pre;
        var node=cur.next;
        cur.next=pre;
        return ReverseLinkedList(node, cur);
    }
}
// @lc code=end

