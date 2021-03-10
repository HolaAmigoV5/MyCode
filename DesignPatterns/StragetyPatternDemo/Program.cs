using System;

namespace StrategyPatternDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            //个人所得税方式
            InterestOperation operation = new InterestOperation(new PersonalTaxStrategy());
            Console.WriteLine("个人支付的税为：{0}", operation.GetTax(5000.00));

            //企业所得税
            operation = new InterestOperation(new EnterpriseTaxStrategy());
            Console.WriteLine("企业支付的税为：{0}", operation.GetTax(50000.00));

            Console.ReadLine();
        }
    }


    /// <summary>
    /// 所得税计算策略
    /// </summary>
    public interface ITaxStragety
    {
        double CalculateTax(double income);
    }


    /// <summary>
    /// 个人所得税
    /// </summary>
    public class PersonalTaxStrategy : ITaxStragety
    {
        public double CalculateTax(double income)
        {
            return income * 0.12;
        }
    }

    /// <summary>
    /// 企业所得税
    /// </summary>
    public class EnterpriseTaxStrategy : ITaxStragety
    {
        public double CalculateTax(double income)
        {
            return (income - 3500) > 0 ? (income - 3500) * 0.045 : 0.0;
        }
    }

    public class InterestOperation
    {
        private ITaxStragety m_strategy;
        public InterestOperation(ITaxStragety strategy)
        {
            m_strategy = strategy;
        }

        public double GetTax(double income)
        {
            return m_strategy.CalculateTax(income);
        }
    }
}
