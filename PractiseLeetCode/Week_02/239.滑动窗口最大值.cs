/*
 * @lc app=leetcode.cn id=239 lang=csharp
 *
 * [239] 滑动窗口最大值
 */

// @lc code=start
public class Solution {
    public int[] MaxSlidingWindow(int[] nums, int k) {
        //Verification
        if(nums==null || nums.Length*k==0) 
            return new int[0];
        if(k==1) return nums;

        //brute force O(nk), O(n)
        // int len=nums.Length;
        // int[] res=new int[len-k+1];
        // for(int i=0; i<res.Length; i++){
        //     int max=int.MinValue;
        //     for(int j=0; j<k; j++){
        //         max=Math.Max(max, nums[i+j]);
        //         res[i]=max;
        //     }
        // }
        // return res;

        //MonotonicQueue. O(n), O(k)
        // MonotonicQueue window=new MonotonicQueue();
        // int[] res=new int[nums.Length-k+1];
        // int index=0;
        // for(int i=0; i<nums.Length; i++){
        //     if(i<k-1){
        //         window.PushToEnd(nums[i]);
        //     }
        //     else{
        //         window.PushToEnd(nums[i]);
        //         res[index++]=window.Max;
        //         window.PopFromStart(nums[i-k+1]);
        //     }
        // }
        // return res;

        //O(n),O(n)
        int count=nums.Length-k+1;
        int[] res=new int[count];
        LinkedList<int> queue=new LinkedList<int>();

        for(int i=0; i<nums.Length; i++){
            //Remove numbers out of range k
            if(queue.Any() && queue.First.Value<i-k+1)
                queue.RemoveFirst();
            
            //remove smaller numbers in k range as they are useless
            while(queue.Any() && nums[queue.Last.Value]<nums[i])
                queue.RemoveLast();

            //q contains index ... ans contains content
            queue.AddLast(i);
            if(i-k+1>=0)
                res[i-k+1]=nums[queue.First()];
        }
        return res;
    }

        private class MonotonicQueue{
        LinkedList<int> queue;
        public MonotonicQueue()=>queue=new LinkedList<int>();
        public int Max=>queue.Max();

        //Pop a item from the start of the queue
        public void PopFromStart(int item){
            if(queue.Any() && queue.First()==item)
                queue.RemoveFirst();
        }

        //Push a item to the end of the queue
        public void PushToEnd(int item){
            while(queue.Any() && queue.Last()<item)
                queue.RemoveLast();
            queue.AddLast(item);
        }
    }
}
// @lc code=end

