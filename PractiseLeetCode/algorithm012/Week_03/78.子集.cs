/*
 * @lc app=leetcode.cn id=78 lang=csharp
 *
 * [78] 子集
 */

// @lc code=start
public class Solution {
    List<IList<int>> res=new List<IList<int>>();
    public IList<IList<int>> Subsets(int[] nums) {
        if(nums==null || nums.Length==0)
            return res;

        BackTrack(nums,0,new List<int>());
        return res;
    }

    private void BackTrack(int[] nums, int start, List<int> list){
        res.Add(new List<int>(list));
        for(int i=start; i<nums.Length; i++){
            list.Add(nums[i]);
            BackTrack(nums,i+1,list);
            list.RemoveAt(list.Count-1);
        }
    }
}
// @lc code=end

