/*
 * @lc app=leetcode.cn id=347 lang=csharp
 *
 * [347] 前 K 个高频元素
 */

// @lc code=start
public class Solution {
    public int[] TopKFrequent(int[] nums, int k) {
        //use Dic O(n), O(n)
        // var dic=new Dictionary<int, int>();
        // foreach (int num in nums)
        // {
        //     if(dic.ContainsKey(num))
        //         dic[num]++;
        //     else
        //         dic[num]=1;
        // }

        // var temp=new List<int>();
        // dic.OrderByDescending(x=>x.Value).Take(k).ToList().ForEach(y=>temp.Add(y.Key));
        // return temp.ToArray();

        //return nums.GroupBy(r=>r).OrderByDescending(r=>r.Count()).Take(k).Select(r=>r.Key).ToArray();

        //Bucket Sort
         var dic = new Dictionary<int, int>();
            var list = new List<int>[nums.Length + 1];
            List<int> res = new List<int>();
            foreach (int num in nums)
            {
                if (dic.ContainsKey(num))
                    dic[num]++;
                else
                    dic[num] = 1;
            }
            
            foreach (int key in dic.Keys)
            {
                int i = dic[key];
                if (list[i] == null)
                    list[i] = new List<int>();
                list[i].Add(key);
            }

            for (int i = list.Length - 1; i >= 0 && res.Count < k; i--)
            {
                if (list[i] == null) continue;
                res.AddRange(list[i]);
            }

            return res.ToArray();
    }
}
// @lc code=end

