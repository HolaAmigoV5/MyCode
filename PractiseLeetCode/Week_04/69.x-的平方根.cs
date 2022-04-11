/*
 * @lc app=leetcode.cn id=69 lang=csharp
 *
 * [69] x 的平方根
 */

// @lc code=start
public class Solution {
    public int MySqrt(int x) {
        if(x<2)
            return x;
        //M1:Brute force
        // long a=1;
        // while(a*a<x)
        //     a++;
        // return a*a==x?(int)a:(int)a-1;

        //M2: use Sqrt
        //return (int)Math.Sqrt(x);

        //M3:Binary Serach
        // int left=0, right=x/2, mid=0;
        // while(left<=right){
        //     mid=left+(right-left)/2;
        //     var res=(double)mid*mid;
        //     if(res==x)
        //         return mid;
        //     if(res>x)
        //         right=mid-1;
        //     else
        //         left=mid+1;
        // }
        // return right;

        //M4:Newton Iteration
        long i=x;
        while(i*i>x)
            i=(i+x/i)/2;
        return (int)i;
    }
}
// @lc code=end

