/*
 * @lc app=leetcode.cn id=46 lang=csharp
 *
 * [46] 全排列
 */

// @lc code=start
public class Solution {
    List<IList<int>> res = new List<IList<int>>();
    int[] the_nums=null;
    public IList<IList<int>> Permute(int[] nums) {
        //O(N*N!), O(N*N!)
        int len=nums.Length;
        if(len==0)
            return res;

        the_nums=nums;
        PermuteNums(new List<int>());
        return res;
    }

    private void PermuteNums(List<int> list){
        if(list.Count==the_nums.Length){
            res.Add(new List<int>(list));
            return;
        }
        for(int i=0; i<the_nums.Length; i++){
            if(list.Contains(the_nums[i]))
                continue;
            list.Add(the_nums[i]);
            PermuteNums(list);
            list.Remove(the_nums[i]);
        }
    }
}
// @lc code=end

