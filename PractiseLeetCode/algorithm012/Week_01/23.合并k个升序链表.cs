/*
 * @lc app=leetcode.cn id=23 lang=csharp
 *
 * [23] 合并K个升序链表
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
    public ListNode MergeKLists(ListNode[] lists) {
        if(lists==null || lists.Length==0)
            return null;
        
        //M1:逐个合并.O(NK)
        // ListNode res=null;
        // foreach(var list in lists){
        //     res=Merge2Lists(res, list);
        // }
        // return res;

        //M2:MergeSort. O(NlogK)
        return Merge(lists, 0, lists.Length-1);
    }

    private ListNode Merge(ListNode[] lists, int left, int right){
        if(left==right)
            return lists[left];
        
        var mid=left+((right-left)>>1);
        var l1=Merge(lists, left, mid);
        var l2=Merge(lists, mid+1, right);
        return Merge2Lists(l1,l2);
    }

    private ListNode Merge2Lists(ListNode l1, ListNode l2){
        //M1:Recursion. the same as 21
        // if(l1==null)
        //     return l2;
        // if(l2==null)
        //     return l1;
        // if(l1.val<l2.val){
        //     l1.next=Merge2Lists(l1.next, l2);
        //     return l1;
        // }
        // else{
        //     l2.next=Merge2Lists(l2.next,l1);
        //     return l2;
        // }

        //M2:Iterator
        ListNode dummy=new ListNode(0);
        ListNode head=dummy;
        while(l1!=null && l2!=null){
            if(l1.val<l2.val){
                head.next=l1;
                l1=l1.next;
            }
            else{
                head.next=l2;
                l2=l2.next;
            }
            head=head.next;
        }
        head.next=l1??l2;
        return dummy.next;
    }
}
// @lc code=end

