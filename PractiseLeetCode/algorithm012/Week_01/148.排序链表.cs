/*
 * @lc app=leetcode.cn id=148 lang=csharp
 *
 * [148] 排序链表
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
    public ListNode SortList(ListNode head) {
        if(head==null || head.next==null)
            return head;
        
        //MergeSort1:Recursion-fast and slow
        //step1: cut
        //注意：fast必须从head.next开始，防止偶数个数链表时，找中点不准确，出现栈溢出
        // ListNode fast=head.next, slow=head; 
        // while(fast!=null && fast.next!=null){
        //     slow=slow.next;
        //     fast=fast.next.next;
        // }
        // var tmp=slow.next;
        // slow.next=null;

        // var left=SortList(head);
        // var right=SortList(tmp);

        // //step 2:Merge
        // ListNode h=new ListNode(0);
        // ListNode res=h;
        // while(left!=null && right!=null){
        //     if(left.val<right.val){
        //         h.next=left;
        //         left=left.next;
        //     }
        //     else{
        //         h.next=right;
        //         right=right.next;
        //     }
        //     h=h.next;
        // } 
        // h.next=left??right;
        // return res.next;

        //Botom-to-up(no recurring)
        ListNode dummy=new ListNode(0);
        dummy.next=head;

        //先统计长度
        int n=0;
        while(head!=null){
            head=head.next;
            n++;
        }

        //循环切割成1,2,4,8..长度后，两两合并
        for(int size=1; size<n; size<<=1){
            ListNode prev=dummy;
            ListNode cur=dummy.next;
            while(cur!=null){
                var left=cur;
                var right=Cut(left, size);  //链表切掉size大小，剩下返还给right
                cur=Cut(right, size);  //链表切掉size, 剩下返还给cur
                prev=Merge(left, right, prev); //left和right合并后再与prev接上
            }
        }
        return dummy.next;
    }

    private ListNode Cut(ListNode head, int size){
        for(int i=1; head!=null && i<size; i++){
            head=head.next;
        }
        if(head==null)
            return null;
        
        ListNode second=head.next;
        head.next=null;
        return second;
    }

    private ListNode Merge(ListNode left, ListNode right, ListNode prev){
        ListNode cur=prev;
        while(left!=null && right!=null){
            if(left.val<right.val){
                cur.next=left;
                left=left.next;
            }
            else{
                cur.next=right;
                right=right.next;
            }
            cur=cur.next;
        }
        cur.next=left??right;
        while(cur.next!=null)
            cur=cur.next;  //保持返回最尾端元素
        
        return cur;
    }
}
// @lc code=end

