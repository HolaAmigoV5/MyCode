/*
 * @lc app=leetcode.cn id=190 lang=csharp
 *
 * [190] 颠倒二进制位
 */

// @lc code=start
public class Solution {
    public uint reverseBits(uint n) {
        // uint res=0;
        // for(uint i=0; i<32; i++){
        //     res=(res<<1)+(n&1);
        //     n>>=1;
        // }
        // return res;

        //(n>>(31-i))&1 取出n的从右往左边的第31-i位的数(结果为0或者1)
        //以上部分<<i 将以上31-i的数升i位=将n的31-i位颠倒(低位晋升到高位)
        //res=res|以上部分, 设置res的第n位二进制数(填补最高位)
        uint res=0;
        for(int i=31; i>=0; i--){
            res=res|(((n>>(31-i))&1)<<i);
        }
        return res;
    }
}
// @lc code=end

