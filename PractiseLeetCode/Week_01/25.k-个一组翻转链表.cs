/*
 * @lc app=leetcode.cn id=25 lang=csharp
 *
 * [25] K 个一组翻转链表
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
    public ListNode ReverseKGroup(ListNode head, int k) {
        if(head==null || head.next==null)
            return head;
        
        //M1:Stack
        // var stack=new Stack<ListNode>();
        // ListNode dummy=new ListNode(0);
        // ListNode pre=dummy;
        // while(true){
        //     int count=0;
        //     ListNode tmp=head;
        //     while(tmp!=null && count<k){
        //         stack.Push(tmp);
        //         tmp=tmp.next;
        //         count++;
        //     }
        //     if(count!=k){
        //         pre.next=head;
        //         break;
        //     }
        //     while(stack.Count>0){
        //         pre.next=stack.Pop();
        //         pre=pre.next;
        //     }
        //     pre.next=tmp;
        //     head=tmp;
        // }
        // return dummy.next;

        //M2:Recursion
        ListNode tail=head;
        int count=k;
        while(count-->0){
            if(tail==null)  //剩余数量小于k的话，则不需要反转
                return head;
            tail=tail.next;
        }
        //反转前k个元素，头变尾，尾变头
        var newHead=Reverse(head, tail);

         //下一轮开始的地方就是tail，转变后的头(尾巴）链接到下一轮循环
        head.next=ReverseKGroup(tail,k);
        return newHead;

        // int n=0;
        // for(ListNode i=head; i!=null; n++, i=i.next);
        // ListNode dmy=new ListNode(0);
        // dmy.next=head;
        // for(ListNode prev=dmy, tail=head; n>=k; n-=k){
        //     for(int i=1; i<k; i++){
        //         ListNode next=tail.next.next;
        //         tail.next.next=prev.next;
        //         prev.next=tail.next;
        //         tail.next=next;
        //     }

        //     prev=tail;
        //     tail=tail.next;
        // }
        // return dmy.next;

        // //1. test weather we have more then k node left, 
        // //if less then k node left we just return head
        // ListNode cur=head;
        // int count=0;
        // while(count!=k){ //find the k+1 node
        //     if(cur==null)
        //         return head;
        //     cur=cur.next;
        //     count++;
        // }
        
        // // 2.reverse k node at current level 
        // //pre node point to the the answer of sub-problem 
        // ListNode pre=ReverseKGroup(cur, k); 
        // while(count-->0){
        //     ListNode next=head.next;
        //     head.next=pre;
        //     pre=head;
        //     head=next;
        // }
        // return pre;
    }

    private ListNode Reverse(ListNode head, ListNode tail){
        ListNode pre=null;
        while(head!=tail){
            var next=head.next;
            head.next=pre;
            pre=head;
            head=next;
        }
        return pre;
    }
}
// @lc code=end

