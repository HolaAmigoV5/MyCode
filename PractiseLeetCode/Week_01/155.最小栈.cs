/*
 * @lc app=leetcode.cn id=155 lang=csharp
 *
 * [155] 最小栈
 */

// @lc code=start
public class MinStack {
    //Stack<int> stack;
    //Stack<int> minStack;
    //int min=0;

    Node head;
    /** initialize your data structure here. */
    public MinStack() {
        //M1:tow stacks
        //M2:one stack
        //M3:node
        //stack=new Stack<int>();
        //minStack=new Stack<int>();

        //min=int.MaxValue;
    }
    
    public void Push(int x) {
        //stack.Push(x);
        // if(minStack.Count==0 || minStack.Peek()>=x)
        //     minStack.Push(x);

        // if(minStack.Any()){
        //     minStack.Push(Math.Min(x, minStack.Peek()));
        // }
        // else
        //     minStack.Push(x);

        // if(x<=min){
        //     stack.Push(min);
        //     min=x;
        // }
        // stack.Push(x);

        if(head==null)
            head=new Node(x,x);
        else
            head=new Node(Math.Min(x, head.MinValue),x, head);
    }
    
    public void Pop() {
        //stack.Pop();
        // if(stack.Pop()==minStack.Peek())
        //     minStack.Pop();

        // stack.Pop();
        // minStack.Pop();

        // if(stack.Pop()==min){
        //     min=stack.Pop();
        // }
        if(head!=null)
            head=head.Next;
    }
    
    public int Top() {
        //return stack.FirstOrDefault();
        //return stack.Peek();
        return head.Value;
    }
    
    public int GetMin() {
        //return stack.Min();
        //return minStack.Peek();
        //return min;
        return head.MinValue;
    }

    public class Node{
        public int MinValue;
        public int Value;
        public Node Next;
        public Node(int minvalue, int value, Node next=null){
            MinValue=minvalue;
            Value=value;
            Next=next;
        }
    }
}

/**
 * Your MinStack object will be instantiated and called as such:
 * MinStack obj = new MinStack();
 * obj.Push(x);
 * obj.Pop();
 * int param_3 = obj.Top();
 * int param_4 = obj.GetMin();
 */
// @lc code=end

