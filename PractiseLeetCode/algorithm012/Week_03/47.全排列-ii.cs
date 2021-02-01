/*
 * @lc app=leetcode.cn id=47 lang=csharp
 *
 * [47] 全排列 II
 */

// @lc code=start
public class Solution {
    List<IList<int>> res=new List<IList<int>>();
    int[] the_nums;
    public IList<IList<int>> PermuteUnique(int[] nums) {
        if(nums==null || nums.Length==0)
            return res;

        //prepare data
        the_nums=nums;
        Array.Sort(the_nums);
        PermuteUniqueHelper(new List<int>(), new bool[nums.Length]);
        return res;
    }

    private void PermuteUniqueHelper(List<int> list, bool[] used){
        if(list.Count==the_nums.Length){
            res.Add(new List<int>(list));
            return;
        }

        for(int i=0; i<the_nums.Length; i++){
            if(used[i])
                continue;
            if(i>0 && the_nums[i]==the_nums[i-1] && !used[i-1])
                continue;
            list.Add(the_nums[i]);
            used[i]=true;
            PermuteUniqueHelper(list,used);
            list.RemoveAt(list.Count-1);
            used[i]=false;
        }
    }
}
// @lc code=end

