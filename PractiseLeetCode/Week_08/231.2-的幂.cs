/*
 * @lc app=leetcode.cn id=231 lang=csharp
 *
 * [231] 2的幂
 */

// @lc code=start
public class Solution {
    public bool IsPowerOfTwo(int n) {
         //Iterative, O(logn)
        // if(n<=0)
        //     return false;
        // while((n&1)==0)
        //     n>>=1;
        // return n==1;

        //Recursive, O(logn)
        //return n>0 &&(n==1 || ((n&1)==0 && IsPowerOfTwo(n>>1)));

        //Bit Opertion. O(1)
        //return n>0 &&((n&(n-1))==0);

        //Look-up table. O(1)
        List<int> list=new List<int>(){1, 2, 4, 8, 16, 32, 64, 128, 256, 512, 
        1024, 2048, 4096, 8192, 16384, 32768, 65536, 131072, 262144, 524288, 
        1048576, 2097152, 4194304, 8388608,16777216, 33554432, 67108864, 
        134217728, 268435456, 536870912, 1073741824};
        
        return list.Contains(n);
    }
}
// @lc code=end

