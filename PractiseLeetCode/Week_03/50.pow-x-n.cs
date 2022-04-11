/*
 * @lc app=leetcode.cn id=50 lang=csharp
 *
 * [50] Pow(x, n)
 */

// @lc code=start
public class Solution {
    public double MyPow(double x, int n) {

        //not good.
        // double res=1;
        // for(int i=0; i<(n>0?n:-n); i++)
        //     res*=x;

        // return n>0?res:1/res;
        
        //quick Pow. Recursion. O(logn), O(logn)
        // int N=n;
        // return N>0?QuickMul(x, N):QuickMul(1/x, -N);

        //Iteration. O(logn), O(1)
        // long N=n;
        // return N>=0?QuickMul2(x,N):1.0/QuickMul2(x,-N);

        if(x==0)
            return 0;
        long b=n;
        double res=1.0;
        if(b<0){
            x=1/x;
            b=-b;
        }
        while(b>0){
            if((b&1)==1)
                res*=x;
            x*=x;
            b>>=1;
        }
        return res;
    }

    private double QuickMul(double x, int N){
        if(N==0)
            return 1;
        var y=QuickMul(x, N/2);
        return N%2==0?y*y:y*y*x;
    }

   private double QuickMul2(double x, long N){
       double ans=1.0;
       double x_contribute=x;
       while(N>0){
            if(N%2==1){
                 ans*=x_contribute;
            }
               
            x_contribute*=x_contribute;
            N/=2;
       }
       return ans;
   }
}
// @lc code=end

