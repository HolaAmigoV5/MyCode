using System;

namespace ChainOfResponsibilityDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            PurchaseRequest requestTelphone = new PurchaseRequest(4000.0, "Telphone");
            PurchaseRequest requestSoftware = new PurchaseRequest(10000.0, "Visual Studio");
            PurchaseRequest requestComputers = new PurchaseRequest(40000.0, "Computers");

            Approver manager = new Manager("Learning Hard");
            Approver Vp = new VicePresident("Tony");
            Approver Pre = new President("BossTom");

            //设置责任链
            manager.NextApprover = Vp;
            Vp.NextApprover = Pre;

            //处理请求
            manager.ProcessRequest(requestTelphone);
            manager.ProcessRequest(requestSoftware);
            manager.ProcessRequest(requestComputers);

            Console.ReadLine();
        }
    }

    /// <summary>
    /// 采购请求
    /// </summary>
    public class PurchaseRequest
    {
        //金额
        public double Amount { get; set; }

        //产品名字
        public string ProductName { get; set; }

        public PurchaseRequest(double amount, string productName)
        {
            Amount = amount;
            ProductName = productName;
        }
    }

    /// <summary>
    /// 审批人，Handler
    /// </summary>
    public abstract class Approver
    {
        public Approver NextApprover { get; set; }
        public string Name { get; set; }
        public Approver(string name)
        {
            this.Name = name;
        }

        public abstract void ProcessRequest(PurchaseRequest request);
    }

    /// <summary>
    /// 经理  ConcreteHandler
    /// </summary>
    public class Manager : Approver
    {
        public Manager(string name) : base(name) { }

        public override void ProcessRequest(PurchaseRequest request)
        {
            if (request.Amount < 10000.0)
                Console.WriteLine("{0}-{1} approved the request of purshing {2}", this, Name, request.ProductName);
            else if (NextApprover != null)
                NextApprover.ProcessRequest(request);
        }
    }

    /// <summary>
    /// 副总  ConcreteHandler
    /// </summary>
    public class VicePresident : Approver
    {
        public VicePresident(string name) : base(name) { }

        public override void ProcessRequest(PurchaseRequest request)
        {
            if (request.Amount < 25000.0)
                Console.WriteLine("{0}-{1} approved the request of purshing {2}", this, Name, request.ProductName);
            else if (NextApprover != null)
                NextApprover.ProcessRequest(request);
        }
    }

    /// <summary>
    /// 总经理 ConcreteHandler
    /// </summary>
    public class President : Approver
    {
        public President(string name) : base(name) { }

        public override void ProcessRequest(PurchaseRequest request)
        {
            if (request.Amount < 100000.0)
                Console.WriteLine("{0}-{1} approved the request of purshing {2}", this, Name, request.ProductName);
            else
                Console.WriteLine("Request需要组织一个会议讨论");
        }
    }
}
