/*
 * @lc app=leetcode.cn id=88 lang=csharp
 *
 * [88] 合并两个有序数组
 */

// @lc code=start
public class Solution {
    public void Merge(int[] nums1, int m, int[] nums2, int n) {
        //M1:
        /*  Array.Copy(nums2,0,nums1,m,n);
         Array.Sort(nums1); */

        //M2:
        /* for (int i = 0; i < nums2.Length; i++)
        {
            nums1[m+i]=nums2[i];
        }
        Array.Sort(nums1); */

        //M3:double pointers
        int p1 = m - 1, p2 = n - 1, p = m + n - 1;
        while (p2 >= 0)
        {
            // if (p1 >= 0 && nums1[p1] > nums2[p2])
            //     nums1[p--] = nums1[p1--];
            // else
            //     nums1[p--] = nums2[p2--];
            nums1[p--]=p1>=0 &&nums1[p1]>nums2[p2]?nums1[p1--]:nums2[p2--];
        }
    }
}
// @lc code=end

