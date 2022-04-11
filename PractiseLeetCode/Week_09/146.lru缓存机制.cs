/*
 * @lc app=leetcode.cn id=146 lang=csharp
 *
 * [146] LRU缓存机制
 */

// @lc code=start
public class LRUCache {
    private Dictionary<int, Node> map;
    private DoubleList cache;
    private int cap;

    public LRUCache(int capacity) {
        this.cap=capacity;
        map=new Dictionary<int, Node>();
        cache=new DoubleList();
    }
    
    public int Get(int key) {
        if(!map.ContainsKey(key))
            return -1;
        int val=map[key].val;
        Put(key,val);
        return val;
    }
    
    public void Put(int key, int value) {
        if(map.ContainsKey(key)){
            cache.Remove(map[key]);
        }
        else{
            if(cap==cache.Size()){
                Node last=cache.RemoveLast();
                map.Remove(last.key);
            }
        }
        Node node=new Node(key, value);
        cache.AddFirst(node);
        map[key]=node;
    }

    class Node{
        public int key, val;
        public Node next, prev;
        public Node(int k, int v){
            this.key=k;
            this.val=v;
        }
    }

    class DoubleList{
        private Node head, tail;
        private int size;

        public void AddFirst(Node node){
            if(head==null)
                head=tail=node;
            else{
                Node n=head;
                n.prev=node;
                node.next=n;
                head=node;
            }
            size++;
        }

        public Node RemoveLast(){
            Node node=tail;
            Remove(tail);
            return node;
        }

        public void Remove(Node node){
            if(head==node && tail==node){
                head=null;
                tail=null;
            }
            else if(tail==node){
                node.prev.next=null;
                tail=node.prev;
            }
            else if(head==node){
                node.next.prev=null;
                head=node.next;
            }
            else{
                node.prev.next=node.next;
                node.next.prev=node.prev;
            }
            size--;
        }

        public int Size(){
            return size;
        }
    }
}

/**
 * Your LRUCache object will be instantiated and called as such:
 * LRUCache obj = new LRUCache(capacity);
 * int param_1 = obj.Get(key);
 * obj.Put(key,value);
 */
// @lc code=end

