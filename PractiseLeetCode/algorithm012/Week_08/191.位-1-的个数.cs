/*
 * @lc app=leetcode.cn id=191 lang=csharp
 *
 * [191] 位1的个数
 */

// @lc code=start
public class Solution {
    public int HammingWeight(uint n) {
        //O(1), O(1)
        // int count=0;
        // int mask=1;
        // for(int i=0; i<32; i++){
        //     if((mask&n)!=0){
        //         count++;
        //     }
        //     mask<<=1;
        // }
        // return count;

        //n&(n-1) 表示清零最低位1
        int count=0;
        while(n!=0){
            count++;
            n&=(n-1);
        }
        return count;
    }
}
// @lc code=end

