/*
 * @lc app=leetcode.cn id=367 lang=csharp
 *
 * [367] 有效的完全平方数
 */

// @lc code=start
public class Solution {
    public bool IsPerfectSquare(int num) {
        if(num<2)
            return true;
        
        //M1:brute force
        // long i=2;
        // while(i*i<num)
        //     i++;
        // return i*i==num;

        //M2:use Sqrt and Pow
        //return Math.Pow((int)Math.Sqrt(num),2)==num; 

        //M3:minus odd
        // int temp=1;
        // while(num>0){
        //     num-=temp;
        //     temp+=2;
        // }
        // return num==0;

        //m4:binary search
        long left=0, right=num/2,mid=0;
        while(left<=right){
            mid=(left+right)/2;
            var temp=mid*mid;
            if(temp==num)
                return true;
            if(temp<num)
                left=mid+1;
            else
                right=mid-1;
        }
        return false;
    }
}
// @lc code=end

