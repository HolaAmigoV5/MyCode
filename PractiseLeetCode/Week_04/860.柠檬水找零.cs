/*
 * @lc app=leetcode.cn id=860 lang=csharp
 *
 * [860] 柠檬水找零
 */

// @lc code=start
public class Solution {
    public bool LemonadeChange(int[] bills) {
        //O(n), O(1)
        int five=0, ten=0;
        foreach(int bill in bills){
            if(bill==5)
                five++;
            else if(bill==10){
                if(five==0)
                    return false;
                five--;
                ten++;
            }
            else{
                if(five>0 && ten>0){
                    five--;
                    ten--;
                }
                else if(five>=3)
                    five-=3;
                else
                    return false;
            }
        }
        return true;
    }
}
// @lc code=end

