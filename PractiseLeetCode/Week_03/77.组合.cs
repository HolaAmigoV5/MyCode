/*
 * @lc app=leetcode.cn id=77 lang=csharp
 *
 * [77] 组合
 */

// @lc code=start
public class Solution {
    List<IList<int>> res=new List<IList<int>>();
    public IList<IList<int>> Combine(int n, int k) {
        //backTracing. O(k*J). O(J). J is the count of Combine.
        if(n<=0 || k<=0 || n<k)
            return res;
        CombineHelper(n,k,1,new List<int>());
        return res;
    }

    private void CombineHelper(int n, int k, int start, List<int> list){
        //terminator
        if(list.Count==k){
            res.Add(new List<int>(list));
            return;
        }

        // for(int i=start; i<=n; i++){
        //     list.Add(i);
        //     CombineHelper(n,k,i+1,list);
        //     list.Remove(i);
        // }
        for(; start<=n-(k-list.Count)+1; start++){
            list.Add(start);
            CombineHelper(n,k,start+1,list);
            list.Remove(start);
        }
        // while(start<=n){
        //     list.Add(start++);
        //     CombineHelper(n,k,start,list);
        //     list.Remove(start-1);
        // }
    }

        private void CombineHelper2(int n, int k,int start, List<int> list){
        if(k==0){
            res.Add(new List<int>(list));
            return;
        }

        // for(;start<=n-k+1; start++){
        //     list.Add(start);
        //     CombineHelper(n,k-1,start+1, list);
        //     list.RemoveAt(list.Count-1);
        // }
        while(start<=n-k+1){
            list.Add(start++);
            CombineHelper(n,k-1,start,list);
            list.RemoveAt(list.Count-1);
        }
    }

}
// @lc code=end

