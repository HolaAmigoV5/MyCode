# 《CLR via C#》
## CLR基础
### CLR的执行模型
[toc]
**名词解释**
* CLR（Common Language Runtime）：公共语言运行时，为应用程序提供运行环境（如32位或64位运行环境）。实际是进程的主线程调用mscoree.dll中的一个方法，初始化CLR。
* CTS(Common Type System)：通用类型系统。实际是一种规范，约定了一个类包含零个或多个成员（字段，方法，属性，事件）；类型的可见性规则（public, internal, protected, private）和类型继承、虚方法、对象生存期等定义了相应的规则。
* CLS(Common Language Specification)：公共语言规范。它是多种语言互相调用的基础。
* IL（Intermediate Language）：中间语言。微软提供了**ILAsm.exe**的IL汇编器和**ILDasm.exe**的IL反汇编器。IL代码有时称为托管代码(managed code)
* 托管模块：由PE32(或PE32+)，CLR头，元数据，IL代码组成。
* 程序集：可以是exe文件，也可以是dll(Dynamic-Link Library)文件。 C#源代码通过C#编译器(CSC.exe)编译成托管模块（中间语言和元数据），同时多个托管模块和资源文件合并就成为了程序集。
* JIT(just-in-time)编译器：即时编译器，用于将IL转换为CPU指令（即010101），一次编译可以多次运行而不用重复编译。IL编译成本机CPU指令时，CLR执行一个名为验证(verification)的过程，如核实方法参数，方法返回值等。微软提供了**PEVerify.exe**程序，检查一个程序集的所有方法，并报告其中含有不安全代码的方法。
* FCL(Framework Class Library)：比如Web服务，WCF，WPF，Windows控制台应用程序等均为FCL。
* PDB文件：Program Database文件，即调试文件，记录IL指令与源代码映射关系的文件。
* 使用/optimize-，在C#编译器生成的未优化IL代码中，包含许多NOP(no-operation 空操作)指令，还包含许多跳转到下一行的分支指令。如果生成优化的IL代码，C#编译器会删除多余的NOP和分支指令。
* 使用debug(+/full/pdbonly)，编译器会生成PDB文件，同时JIT编译器记录IL指令生成的机器码。
* 托管应用程序的性能超越了非托管应用程序。
* 微软提供NGen.exe工具，可以在应用程序安装到用户的计算机上时，将IL代码编译成本机代码。使用要慎重。
* 对于启动较慢的大型客户端应用程序，微软提供了MPGO.exe工具，该工具可以检查程序启动时需要哪些东西。

**CLR的执行模型**
![CLRModel](https://cdn.jsdelivr.net/gh/HolaAmigoV5/Images/DotNet/CLRModel.png)
**程序执行流程**
![ExeProcess](https://cdn.jsdelivr.net/gh/HolaAmigoV5/Images/DotNet/ExeProcess.png)

**CLR/CTS/CLS关系图**
![CLRandCTSandCLS](https://cdn.jsdelivr.net/gh/HolaAmigoV5/Images/DotNet/CLRandCTSandCLS.png)

### 程序集
**重点知识**
* 强命名程序集具有4个重要特性：文件名（不计扩展名）、版本号、语言文化和公钥
* GUID(Globally Unique Identifier)：全局唯一标识符
* URL(Uniform Resource Locator)：统一资源定位符
* URN(Uniform Resource Name)：统一资源名称
* 公钥标记（public key token）
* 使用Strong Name程序（SN.exe）创建公钥并对程序集进行签名
  1. 创建.snk文件：`SN -k MyCompany.snk` (该文件包含二进制形式的公钥和私钥)
  2. 创建只含公钥的文件：`SN -p MyCompany.snk MyCompany.PublicKey sha256`
  3. 传递只含公钥的文件：`SN -tp MyCompany.Publickey`
  4. 对程序集进行签名：`csc /keyfile:MyCompany.snk  Program.cs`
 * GAC(Global Assembly Cache)：全局程序集缓存
## 设计类型
### 类型基础
* 所有类型继承自System.Object。
* as类型转换比is类型转换效率高。
---
### 类型
**重点知识**
* 编译器直接支持的数据类型称为基元类型(primitive type)。
* C#基元类型与对应的FCL类型。

    ``` C#
     using string = System.String;
     using int = System.Int32;
     using long = System.Int64;
    ```
* dynamic关键字定义**动态类型**，动态类型在运行时才能确定实际类型，实质是包含了System.Dynamic.DynamicAttribute特性的System.Object类型。var是通过变量的初始化推断类型（var不是类型）。
* 声明变量的内部机制
   ![Variable](https://cdn.jsdelivr.net/gh/HolaAmigoV5/Images/DotNet/Variable.png)
* 托管堆(heap)和线程堆栈(stack)
   * 托管堆没有固定的容量限制，保存引用类型，由GC管理器进行托管，进行自动分配和释放。属于动态内存
   * 在CLR中的线程堆栈用来执行线程方法时，保存局部变量或值类型，在栈上的成员不受GC管理器控制，由操作系统分配内存，方法执行完后，该栈上成员采用后进先出的顺序由操作系统进行释放，执行效率高。属于静态内存。
   ![memory](https://cdn.jsdelivr.net/gh/HolaAmigoV5/Images/DotNet/memory.png)
   ![StackAndHeap](https://cdn.jsdelivr.net/gh/HolaAmigoV5/Images/DotNet/StackAndHeap.png)
* 值类型和引用类型
     ![ValueTypeAndReferenceTypes](https://cdn.jsdelivr.net/gh/HolaAmigoV5/Images/DotNet/ValueTypeAndReferenceTypes.png)
   
* 值类型：使用struct, enum关键字直接派生自System.ValueType定义的类型。存储在栈上（一个字节=8位，32位=4个字节，像Int32为代表的值类型本身就是固定的占用内存大小，所以值类型放在类型连续分配的栈上）。
   ![ValueTypes](https://cdn.jsdelivr.net/gh/HolaAmigoV5/Images/DotNet/ValueTypes.png)
* 引用类型：使用class, interface, delagate关键字派生自System.Object定义的类型。存储在堆上（对于类为代表的引用类型，一开始不知道需要多大内存，随着使用不断扩展，就需要动态的内存存储，所以引用类型存储在不连续的堆上）。
   ![Reference](https://cdn.jsdelivr.net/gh/HolaAmigoV5/Images/DotNet/Reference.png)
   
* 装箱：值类型转换成引用类型
  * 在托管堆中分配内存。内存量包括值类型字段所需的内存量，还要加上类型对象指针和同步索引块所需的内存量；
  * 值类型的字段复制到新分配的堆内存；
  * 返回对象地址。
* 拆箱：引用类型转换值类型。
    ![Boxing](https://cdn.jsdelivr.net/gh/HolaAmigoV5/Images/DotNet/Boxing.png)
***
### 类型和成员基础
**重点知识**
* **类型**的**可见性**：
    * public：所有代码可见
    * internal：对定义程序集中的所有代码可见
* 友元程序集(friend assembly)：TeamA把自己的类定义为internal，同时允许TeamB访问这些类型。主要用于单元测试。
* **成员**的**可访问性**：
    * private：成员只能由定义类型或任何嵌套类型中的方法访问；
    * protected：成员只能由定义类型、任何嵌套类型或者不管在什么程序集中的派生类型中的方法访问；
    * internal：成员只能由定义程序集中的方法访问；
    * protected internal：成员可由任何嵌套类型、任何派生类型（不管什么程序集）或者定义程序集中的任何方法访问；
    * public：成员可由任何程序集的任何方法访问。
* **特别注意：** 如果没有显示声明成员的可访问性，编译器默认选择private。CLR要求接口类型的所有成员都有public可访问性。
* 静态类：永远不需要实例化的类（如Console,Math等），用static关键字定义，该关键字只能应用于类，不能应用于结构（因为值类型总是会实例化）。C#编译器对静态类的限制：
    * 静态类必须直接从System.Object派生；
    * 静态类不能实现任何接口（因为只有实例类才能调用接口）；
    * 静态类只能定义静态成员（字段、方法、属性、事件）
    * 静态类不能作为字段、方法参数或局部变量使用（因为它们都代表引用了实例的变量）
***
### 常量和变量
**重点知识**
* 常量是值从不变化的符号。值在编译时确定。常量总是被视为类的一部分，因此常量总是静态成员。常量的值直接嵌入到IL代码，故运行时不需要为常量分配任何内存，不能获取常量地址，不能传引用方式传递常量。
* 字段是一种数据成员，其中容纳了一个值类型的实例或者一个引用类型的引用。字段修饰符有static，readonly，volatile。
    * 字段存储在动态类型中，所以在运行时才能获取，可以是任何类型数据。
    * readonly字段：在构造函数执行时，该对象创建。当此字段是引用类型，不可改变的是引用，而非字段引用的对象。
***
### 方法
**重点知识**
* 实例构造器和类(引用类型)
* 实例构造器和结构(值类型)
* 类型构造器
* Rational转换操作符
* 扩展方法（第一个参数前面有this的方法）
    * C#只支持扩展方法，不支持扩展属性、事件、操作符等；
    * 扩展方法必须在非泛型的静态类中声明。类名没有限制，至少有一个参数，而且只有第一个参数用this标记；
    * 扩展方法必须在顶级静态类中定义。
* 分部方法的规则和原则
    * 它们只能在分部类或结构中声明；
    * 分部方法的返回类型始终是void，任何参数不能用out标记；
    * 分部方法需要有一致的签名；
    * 分部方法总是private方法，但C#编译器禁止在方法前添加private关键字。
***
### 参数
**重点知识**
* 可选参数和命名参数
    1. 有默认值的参数必须放在没有默认值参数之后；
    2. 默认值必须是编译时能确定的常量值；
    3. C#编译器内部向该参数应用OptionalAttribute和DefaultParameterValueAttribut特性，在DefaultParameterValueAttribute的构造器传递你在源码中指定的常量值。之后，一旦编译器发现某个方法调用缺失了部分实参，就可以确定省略的是可选的实参，并从元数据中提取默认值，将值自动嵌入调用中；
    4. 如果参数用ref和out标识，不能设置默认值，如下：
       
        ```C#
        //方法声明：
        private static void M(ref int x){...}
        //方法的调用
        int a=5;
        M(x: ref a);
        ```
 * 隐式类型的局部变量（var关键字）：`var n=null; //错误，不能将null赋给var`。var的真正价值在于简化代码（如 `var col=new Dictionary<string, single>(){"Grant", 4.0f } `）。本质是一种简化语法。
* 关键字out和ref告诉C#编译器参数是传引用，而非传递参数本身。都会生成相同的IL代码。不同的是，out传值在方法调用时初始化，ref传值时要在方法调用前初始化。
* 对于引用类型使用out和ref，仅当方法“返回”对“方法知道的一个对象”的引用时使用才有意义。属性不能作为out或ref参数传给方法。
* 以传引用的方式传递变量，变量类型必须和方法参数类型一致。
* **可变参数**：`private static void M(params object[] objects) { }` 可以容纳任意数量，任意类型参数。调用可变参数的方法对性能有所影响，而且params只能应用在最后一个参数上。
* 声明方法的参数类型时，应尽量指定最弱的类型，宁愿要接口也不要基类。如下代码：
    ```C#
    //好：方法参数可以传递数组，List<T>，string等任意实现了IEnumerable<T>接口
    public void ManipulateItems<T>(IEnumerable<T> collection) { ... }
    //不好：方法只能传递List<T>对象
    public void ManipulateItems<T>(List<T> collection) { ... }
    ```
    
***
### 属性
**重点知识**
* 自动属性(Automatically Implemented Property AIP)：`public String Name{get; set;}`
* 自动属性可以初始化：`public String Name{get;set;}=''张三` 
* 对象和集合初始化器：通过在集合初始化器中嵌套大括号，可向Add方法传递多个实参。

``` C#
   public sealed class classroom{
        public List<String> Students { get; }=new List<String>();
     }
   public static void Main(){
        Classroom classroom=new Classroom{Students={"Jeff","Join","Jack"} };
     }
```
* 匿名类型（元组类型 tuple）含有一组属性的类型：
```C#
var o1=new  { Name="feff",Year=1966 };
var people =new [ ] {
        o1, new { Name="Jack",Year=1988 }, new { Name="John",Year=1999 }
    };
```
* 匿名类型经常与LINQ(Language Intergrated Query 语言集成查询)配合使用：
```C#
String myDocuments=Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
var query=
      from pathname in Directory.GetFiles(myDocuments)
      let LastWriteTime=File.GetLastWriteTime(pathname)
      where LastWriteTime>(DateTime.Now-TimeSpan.FromDays(7))
      orderby LastWriteTime
      select new {Path=pathname, LastWriteTime}; //匿名类型对象构成的集合
  
foreach (var file in query)
    Console.WriteLine($"LastWriteTime={file.LastWriteTime}, Path={file.Path}")
```
* System.Tuple类型包含一组静态Create方法，能根据实参推断泛型类型。
* System.Dynamic.ExpandoObject类：
```C#
dynamic  e= new System.Dynamic.ExpandoObject();
e.x=6;      //添加一个Int32 'x'属性，值为6
e.y="jeff"; //添加一个String 'y'属性，值为"Jeff"
e.z=null;   //添加一个Objcet 'z’属性，值为null
//查看所有属性和值
foreach (var v in (IDictionary<Sting,objcet>) e)
    Console.WriteLine("Key={0},V={1}", v.Key,v.Value);
 //删除'x'属性及值
 var d=(IDictionary<Sting,Objcet>)e;
 d.Remove("x");
```
 * 有参属性
***
 ### 事件
 **重点知识**
 * 委托是一个类，定义了方法的类型，使方法可以作为另一个方法的参数进行传递。此方式可以避免大量使用If-Else(Switch)语句，同时使程序更有扩展性。
 * 事件的本质是对委托进行封装（委托定义为private，event公布为public）。事件的声明实际转换为一个委托变量和Add_XX()和Remove_XX()方法。

```C#
 public event GreetingDelegate MakeGreet;
 
 //以上声明的事件实际转换为如下代码
 private GreetingDelegate MakeGreet;  //实际转换为私有委托
 
 //‘+=’时，使用如下方法
 public void Add_MakeGreet(GreetingDelegate value){
    this.MakeGreet=(GreetingDelegate)Delegate.Combine(this.MakeGreet, value);
 }
 
 //‘-=’时，使用如下方法
 public void Remove_MakeGreet(GreetingDelegate value){
    this.MakeGreet=(GreetingDelegate)Delegate.Remove(this.MakeGreet, value);
 }
```
***
### 泛型
**重点知识**
* 泛型(generic)：实质是另一种代码重用，即“算法重用”。常用的有泛型集合类，泛型接口，泛型委托，泛型方法。
* 泛型的优势：源代码保护，类型安全，更清晰的代码，更佳的性能。
* 具有泛型类型参数的类型称为开放类型，CLR禁止构造开放类型的任何实例。
如：`var dic=Dictionary<TKey,TValue>()`。
* 为所有类型参数都传递了实际的数据类型，类型就是封闭类型，CLR允许封闭类型的实例。如：`var dic=Dictionary<int,string>()`;
* 泛型类型参数可以是以下任何一种形式：
    * 不变量(invariant)，意味着泛型类型参数不能更改。
    * 逆变量(contravariant)，意味着泛型类型参数可以从一个类更改为它的某个派生类。参数用in关键字标记（父类转为子类）。
    * 协变量(convariant)，意味着泛型参数可以从一个类更改为它的某个基类。参数用out关键字标记（子类转为父类）。
    
```C#
  public delegate TResult Func<in T, out TResult>(T arg);
  Func<Object, ArgumentException> fn1=null;
  Func<String, Exception> fn2=fn1  //不需要显示转型
  Exception e=fn2("");
```
* 类型参数可以知道0或者一个主要约束。主要约束可以是代表非密封类的一个引用类型。如：
`internal sealed class M<T> where T:class{ ... } `。
* 类型参数可以知道0或多个次要约束，次要约束代表接口类型。如：
`private static List<TBase> ConvertIList<T,TBase>(IList<T> list) where T:TBase { ... }`
* 参数类型可指定0或1个构造器约束，它向编译器承诺类型实参是实现了公共无参构造器的非抽象类型。如：`internal sealed class M<T> where T:new(){ ... }`
* 泛型类型变量的转型：
```C#
    privat static void M<T>(T obj){
        String s= obj as String;
    }
    
    pri static void M2<T>(){
        T temp=default(T);  //如果T是引用类temp=null，如果是指类型，temp=0
    }
```
***
### 接口
**重点知识**
* 定义方法的那个接口的名称作为方法名前缀（如：IDisposable.Dispose），就会创建显示接口方法实现(Explicit Interface Method Implementation, EIMI)。避免使用EIMI.
* 泛型类型参数约束为接口的好处：
    1. 可将泛型类型参数约束为多个接口：实际表示向方法传递的实参必须实现多个接口；
    2. 传递值类型的实例时减少装箱。

## 基本类型
### 字符串
**重点知识**
* String对象是**不可变字符串**。`s.ToUpper().Substring(10,21).EndsWith("EXE")`中执行ToUpper()方法和Substring(10,21)方法都返回新的字符串，没有修改字符串s的字符。如果执行大量字符串的操作，会在堆上创建大量String对象，造成更频繁的垃圾回收，从而影响应用程序性能。要高效执行大量字符串操作，建议使用StringBuilder类。
* 如果应用程序经常对字符串进行区分大小写的序号比较，或者事先知道许多字符串对象都有相同的值，就可以利用CLR的**字符串留用**（string interning）机制来显著提高性能。字符串留用：`String s1="Hello"  s1=String.Intern(s1);` 。字符串留用虽然有用，但使用需谨慎。
* StringBuilder代表**可变字符串**，该对象的大多数成员都能更改字符串数组的内容，不造成托管堆上分配新对象。
* **定制格式化器**：格式化Html文本，希望所有Int值都加粗显示

``` C#
  class Program
    {
        static void Main(string[] args)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(new BoldInt32s(), "{0} {1} {2:M}", "Jeff", 123, DateTime.Now);
            Console.WriteLine(sb);  //输出“Jeff  <B>123</B> 4月9日 ”
            Console.ReadLine();
        }
    }

    internal sealed class BoldInt32s : IFormatProvider, ICustomFormatter
    {
        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            string s;
            if (!(arg is IFormattable formattable))
                s = arg.ToString();
            else
                s = formattable.ToString(format, formatProvider);
            if (arg.GetType() == typeof(Int32))
                return "<B>" + s + "</B>";
            return s;
        }

        public object GetFormat(Type formatType)
        {
            if (formatType == typeof(ICustomFormatter))
                return this;
            return Thread.CurrentThread.CurrentCulture.GetFormat(formatType);
        }
    }
```
* 安全字符串类SecureString，构造该对象时，会在内部分配一个非托管内存块，其中包含字符数组。

### 枚举
**重点知识**
* 每个枚举类型都直接从System.Enum派生。枚举类型是值类型。
```C#
internal enum Color{
    White, Red, Green, Blue, Orange
}
```
* 可以通过扩展方法功能模拟向枚举类型添加方法。

### 数组
**重点知识**
* 所有数组类型都隐式地从System.Array抽象类派生。所有数组都隐式实现IEnumerable，ICollection和IList。
```C#
string [] names=new string [] {"Aidan", "Grant"}
 
 //利用C#隐式类型的局部变量功能，简化初始化
 var names = new string [] {"Aidan", "Grant"}
 
 //继续简化
 string [] names= {"Aidan", "Grant"}
```
* 二维数组不能转型为一维数组。不能将值类型的数组转型为其他任何类型。
* Array.Copy执行的是对原始数组的浅拷贝。
* 使用Array的CreateInstance（）静态方法创建数组：
```C#
//创建二维数组[2004...2009][1...4]
 int[] lowerBounds = { 2005, 1 };
 int[] lengths = { 5, 4 };
 decimal[,] quarterlyRevenue = 
 (decimal[,])Array.CreateInstance(typeof(decimal), lengths, lowerBounds);
Console.WriteLine("{0,4} {1,9} {2,9} {3,9} {4,9}", "Year", "Q1", "Q2", "Q3", "Q4");

 int firstYear = quarterlyRevenue.GetLowerBound(0);
 int lastYear = quarterlyRevenue.GetUpperBound(0);
 int firstQuarter = quarterlyRevenue.GetLowerBound(1);
 int lastQuarter = quarterlyRevenue.GetUpperBound(1);
 for (int year = firstYear; year <= lastYear; year++)
    {
       Console.Write(year + " ");
        for (int quarter = firstQuarter; quarter <= lastQuarter; quarter++)
        {
            Console.Write("{0,9:C}", quarterlyRevenue[year, quarter]);
        }
       Console.WriteLine();
    }
    Console.ReadLine();
```
### 委托
**重点知识**
* 委托对象是方法的包装器（wrapper），使方法能通过包装器间接回调。其本质是一个类，该类有4个方法：构造器，Invoke，BeginInvoke和EndInvoke。所有委托继承自FCL定义的System.MulticastDelegate类，该类又派生自System.Delegate。
```C#
using System;
using System.Windows.Forms;
using System.IO;

//声明一个委托类型，它的实例引用一个方法，该方法获取一个int参数，返回void
internal delegate void Feedback(int value);

public sealed class Program{
    public static void Main(){
      StaticDelegateDemo();
      InstanceDelegateDemo();
      ChainDelegateDemo1(new Program());
      ChainDelegateDemo2(new Program());
    }
    
    private static void StaticDelegateDemo(){
       Console.WriteLine("---------Static Delegate Demo-------");
       Counter(1,3,null);
       Counter(1,3, new Feedback(Program.FeedbackToConsole));
       Counter(1,3, new Feedback(FeedbackToMsgBox));
       Console.WriteLine();
    }

    private static void InstanceDelegateDemo(){
         Console.WriteLine("---------Instance Delegate Demo-------");
         Program p=new Program();
         Counter(1,3, new Feedback(p.FeedbackToFile));
         Console.WriteLine();
    }

    private static void ChainDelegateDemo1(Program p){
         Console.WriteLine("---------Chain Delegate Demo 1-------");
         Feedback fb1=new Feedback(FeedbackToConsole);
         Feedback fb2=new Feedback(FeedbackToMsgBox);
         Feedback fb3=new Feedback(p.FeedbackToFile);

         Feedback fbChain=null;
         fbChain=(Feedback)Delegate.Combine(fbChain,fb1);         
         fbChain=(Feedback)Delegate.Combine(fbChain,fb2);
         fbChain=(Feedback)Delegate.Combine(fbChain,fb3);
         Counter(1,2,fbChain);
         Console.WriteLine();

         fbChain=(Feedback)Delegate.Remove(fbChain,new Feedback(FeedbackToMsgBox));
         Counter(1,2,fbChain);
    }

    private static void ChainDelegateDemo2(Program p){
         Console.WriteLine("---------Chain Delegate Demo 2-------");
         Feedback fb1=new Feedback(FeedbackToConsole);
         Feedback fb2=new Feedback(FeedbackToMsgBox);
         Feedback fb3=new Feedback(p.FeedbackToFile);

         Feedback fbChain=null;
         fbChain+=fb1;
         fbChain+=fb2;
         fbChain+=fb3;
         Counter(1,2, fbChain);

         Console.WriteLine();
         fbChain-=new Feedback(FeedbackToMsgBox);
         Counter(1,2,fbChain);
    }

    private static void Counter(int from, int to, Feedback fb){
        for(int val=from; val<=to; val++){
              if(fb!=null)
                 fb(val);
         }
    }

    private static void FeedbackToConsole(int value){
         Console.WriteLine("Item="+value);
    }

    private static void FeedbackToMsgBox(int value){
         MessageBox.Show("Item="+value);
    }

    private void FeedbackToFile(int value){
         using(StreamWriter sw=new StreamWriter("Status",true)){
              sw.WriteLine("Item="+value);
          }
    }
}
```
* 将方法绑定到委托时，C#和CLR都允许引用类型的协变性(covariance)和逆变性(contravariance)。只有引用类型才支持协变和逆变，值类型或者void不支持。
```C#
delegate object MyCallBack(FileStream s);

//SomeMethod的返回类型(string)派生自委托类型的返回类型(object)，这是协变
//SomeMethod的参数类型(Stream)是委托的参数类型(FileStream)的基类，这是逆变
string SomeMethod(Stream s);
```
* MulticastDelegate中三个重要的非公共字段：_traget(回调方法的实例对象)，_methodPtr（回调方法），_invocationList(委托数组组成的委托链)。
* MulticastDelegate类中提供了GetInvocationList实例方法获取委托链中的委托数组，然后可以遍历委托数组，执行委托。该方法可以解决委托链中默认返回最后一个委托的返回值问题以及委托链中有任意一回调方法执行错误，委托链执行中断问题。
* 泛型委托：无返回值的Action委托，有返回值的Func委托
    * Action委托：`public delegate void Action<int T1, int T2>(T1 arg1, T2 arg2);` 共17个；泛型委托Action的入参(in)只有逆变。。
    * Func委托：`public delegate TResult Func<T1,T2, out TResult>(T1 arg1, T2 arg2)`，共17个；Func泛型委托的出参(out)只能协变。
 * C#为委托提供的简化语法（语法糖）
    1. 不需要构造委托对象：在需要委托对象时只提供方法，C#编译器可自行推断委托，IL代码自动生成委托对象；
    2. 不需要定义回调方法（用lambda表达式，也叫匿名函数）；
    3. 局部变量不需要手动包装到类中即可传给回调方法；
```C#
        //创建并初始化一个string数组
        string [] names={"Jeff", "KK", "Tom", "Aidan", "Grant"};

        //只获取含有小写字母'a'的名字
        char charToFind='a';
        names=Array.FindAll(names, name=>name.indexOf(charToFind)>=0);

        //将每个字符串的字符转换为大写
        names=Array.ConvertAll(names, name=>name.ToUpper());

        //显示结果
        Array.ForEach(names, Console.WriteLine);
```
* System.Delegate.MethodInfo提供了一个CreateDelegate方法，创建委托。System.Delegate提供DynamicInvoke方法调用回调方法。委托和反射代码：
```C#
    Delegate d;
    //将Arg1实参转换为方法
    MethodInfo mi = typeof(DelegateReflection).GetTypeInfo().GetDeclaredMethod(args[1]);
    //创建包装了静态方法的委托对象
    d = mi.CreateDelegate(delType);
    
    //调用委托并显示结果。callbackArgs为委托对象传给方法的参数
   object result = d.DynamicInvoke(callbackArgs);
    Console.WriteLine("Result = " + result);
```

### 定制特性
**重点知识**
* 定制特性其实是一个类型的实例。定制特性必须直接或者间接从公共抽象类System.Attribute派生。
* 检测定制特性`this.GetType().IsDefined(typeof(FlagsAttribute),false)`
* System.Reflection.CustomAttributeExtensions定的三个静态方法：
    * IsDefined：至少有一个指定的Attribute派生类的实例与目标关联，就返回true。方法高效，因为它不构造特性类的任何实例。
    * GetCustomAttributes：返回应用于目标的指定特性对象的集合。
    * GetCustomAttribute：返回应用于目标的指定特性类的实例。
* System.Reflection命名空间定义了几个类允许检查模块的元数据。这些类包括Assembly，Module，ParameterInfo，MemberInfo，Type，MethodInfo，ConstructorInfo，FieldInfo，EventInfo，PropertyInfo及各自的\*Bulder类。所有类提供了IsDefined和GetCustomAttributes方法。
* 检测定制属性时不创建从Attribute派生的对象：System.Reflection.CustomAttributeData。
* 应用了System.Dignostics.ConditionalAttribute的特性类称为条件特性类。
* 特性是应用：实体类成员属性的约束（比如长度约束，显示名称约束）。实现方式：① 定义继承Attribute的自定义特性类，② 扩展Validate方法，调用验证

### 可空值类型
**重点知识**
* 可空值类型：System.Nullable<T>。由于数据库中的列或者Java中类型可能允许值为空，映射到FCL中，没有办法把值类型表示成null。于是可空值类型出现了。
```C#
Nullable<int> x=5;
Nullable<int> y=null;

//以上代码可以简写为：
int? x=5;
int? y=null;
```
* 空接合操作符“??"。“??”左边操作数不为null，就返回该操作数，否则返回后边操作数。
```C#
  int? b=null;
  int x=b ?? 123;  // 等价于x=(b.HasValue)?b.Value:123;
  
  //符合情形下的简洁
  string s = SomeMethod1() ?? SomeMethod2() ?? "Untitled";
```


## 核心机制
### 异常和状态管理
**重点知识**
* C#捕捉类型必须是System.Exception或者它的派生类型。首先出现的是派生程度最大的异常类型，接着是基类，最后是System.Exception。不按照这个顺序会报错。
* 设计规范和最佳实践
    * 善用finally块：用finally块清理已成功启动的操作或释放对象，避免资源泄露。lock,using,foreach，析构方法。
    * 不要什么都捕捉：不要悄悄吞噬异常，要重新抛出异常；
    * 得体的从异常中恢复：能预料的异常捕捉后，使代码能得体的恢复并继续；
    * 发送不可恢复的异常要回滚状态：回滚原来状态后，继续Throw异常；
    * 隐藏实现细节来维系协定：捕捉异常处理后，抛出具体的异常。
* 没有任何catch块匹配，就发生一个未处理的异常，发生未处理异常时进程终止。应用程序发生未处理异常时，Windows会向事件日志写一条记录。
* 代码协定（没搞懂）

### 托管堆和垃圾回收
**重点知识**
* C#的new操作符导致CLR执行的步骤如下：
    1. 计算类型的字段所需要的字节数；
    2. 加上对象的开销所需要的字节数。主要是类型对象指针和同步块索引。
    3. CLR检查是否有分配对象所需的字节数。如果有，就在NextObjPtr指针指向的地址放入对象，为对象分配的字节会被清零。NextObjPtr指针的值加上对象占用字节数得到一个新值，即下一个对象放入托管堆时的地址。
* 垃圾回收：应用程序调用new操作符创建对象时，可能没有足够的地址空间来分配该对象，CLR就执行垃圾回收。GC回收的是托管资源。
*  **垃圾回收机制：标记回收，压缩偏移** 。
    1. 标记阶段：CLR开始GC时，首先暂停进程中所有线程。然后CLR遍历堆中的所有对象，将**同步块索引**字段中的一位设为0。接着检查所有活动根，如果根上引用了堆中对象，就讲该对象的同步块索引中的位设为1。一个对象被标记后，CLR会检查那个对象中是否引用了另外一个对象，标记引用的另一个对象；
    2. 清除阶段：GC对未标记的对象进行垃圾回收；
    3. 压缩阶段：CLR对堆中已标记的对象进行“乾坤大挪移”，压缩所有幸存下来的对象，使它们占用连续的内存空间。压缩的过程中，CLR从每一个根中减去所引用的对象在内存中偏移的字节数（这样保证每个根还是引用和之前一样的对象）。该阶段完成后，CLR恢复应用程序的所有线程。
![GC](https://cdn.jsdelivr.net/gh/HolaAmigoV5/Images/DotNet/GC.png)
![Mark-Compact](https://cdn.jsdelivr.net/gh/HolaAmigoV5/Images/DotNet/Mark-Compact.png)

 * OutOfMemoryException：GC进行垃圾回收后，进程还是没有足够的内存进行分配，进行new操作时，就出现该错误（内存泄露）。内存泄露一个常见原因是静态字段应用了某个集合对象，然后不停的向集合添加数据项。静态字段会一直存在，直到AppDomain卸载为止。
 * 以下代码中，TimerCallback不会如预期一样每隔2000毫秒调用一次，而只会调用一次。因为初始化之后，Main方法再没有用过变量t，就会被GC回收。

```C#
 public static class Program {
     public static void Main() {
       //创建每隔2000毫秒调用一次TimerCallback方法的Timer对象
       Timer t=new Timer(TimerCallback, null, 0, 2000);
       Console.ReadLine();
     }
     
     private static void TimerCallback(Object o) {
         //当调用该方法时，显示日期和时间
         Console.WriteLine("In TimerCallback:" + DateTime.Now);
         
         //出于演示，强制执行一次垃圾回收
         GC.Collect();
     }
 }
```
* 代：提升性能。CLR的GC是基于代的垃圾回收器。它对你的代码做出来以下几点假设：

    1. 对象越新，生存期越短；
    2. 对象越老，生存期越长；
    3. 回收堆的一部分，速度快于回收整个堆。
* 代的工作原理：
    * 托管堆初始化的对象为第0代对象，经历一次垃圾回收后存活的对象升级为第1代对象，此时第0代对象为空，再有新对象会分配到第0代中。经历第二次垃圾回收0代后，原来的第0代提升至第1代（第1代的大小变大），此时第0代又空出来了。
    * 假定第1代的增长导致占用了全部预算，此时GC需要检查第1和第0代中的所有对象。再次垃圾回收后，第1代幸存者提升至第2代，第0代提升至第1代，第0代空余出来。
    * 托管堆只支持三代（第0代，第1代，第2代）。CLR初始化时会为每一代选择内存预算。如果没有足够的内存，GC会执行一次完整的回收。如果还不够，就抛出OutOfMemoryException异常。
    ![GCGen012](https://cdn.jsdelivr.net/gh/HolaAmigoV5/Images/DotNet/GCGen012.png)
* 垃圾回收触发条件：
    * 代码显示调用System.GC的静态Collect方法；
    * Windows报告低内存情况；
    * CLR正在卸载APPDomain：执行所有代的垃圾回收
    * CLR正在关闭：由Windows回收全部内存
* 大对象：目前认为85000字节或更大的对象为大对象。
    * 大对象不是在小对象的地址空间分配，而是在进程地址空间的其他地方分配；
    * 目前版本的GC不压缩大对象（因为移动代价太高）。
    * 大对象总是第2代，绝不可能是第0代和第1代。
* 只有GC完成后才运行Finalize。Finalize使用需谨慎。
* 使用using时，编译器会自动加上try块和finally块。使用using的前提是该类型实现了IDisposable接口。

### CLR寄宿和APPDomain
**重点知识**
* AppDomain是一组程序集的逻辑容器。隔离是AppDomain的全部目的。一个进程可以运行多个AppDomain，线程和AppDomain没有一对一关系。
    * 一个AppDomain的代码不能直接访问另外一个AppDomain代码创建的对象；
    * AppDomain可以卸载；
    * AppDomain可以单独保护
    * AppDomain可以单独配置
* 按引用封送（Marshal-by-Reference）进行跨AppDomain边界的对象访问，会产生性能开销，少用。实际就是在新建的AppDomain中创建代理调用原AppDomain对象。按引用封送的对象要继承自System.MarshalByRefObject。
* 按值封送（Marshal-by-Value）进行跨AppDomain通信：实质是在新的AppDomain中复制了源AppDomain中对象。这是个真实的对象，而不是代理。按值封送对象要用[Serializable]特性标记。
* 卸载AppDomain：`AppDomain.Unload(ad2)`
    * CLR挂起进程中执行过托管代码的所有线程。
    * CLR检查所有线程栈；
    * 发现所有线程离开AppDomain后，CLR遍历堆，为代理对象设置flag；
    * CLR强制垃圾回收：
    * CLR恢复剩余所有线程的执行。

### 程序集加载和反射
**重点知识**
* 程序集加载：
```C#
    public class Assembly {
      public static Assembly Load(AssemblyName assemblyRef);
      public static Assembly Load(string assemblyString);
      
      //LoadFrom方法本质是在内部调用Load(AssemblyName assemblyRef)方法
      public static Assembly LoadFrom(string path);
      
      //该方法只加载程序集，不执行程序集中方法
      public static Assembly ReflectionOnlyLoadFrom(string assemblyFile);
      public static Assembly RefectionOnlyLoad(string assemblyString);
    }
```
* 反射：利用System.Reflection中包含的类型反射（解析）出这些元数据表（类型定义表，方法定义表，字段定义表等）反射是相当强大的机制，允许在运行时发现并使用编译时还不了解的类型及成员。
* 反射的缺点：
    1. 反射造成编译时无法保证类型安全。例如`Type.GetType("int")`，编译通过，但是运行时返回null。因为CLR只知道“System.Int32”，不知道“int”。
    2. 反射速度慢。反射严重依赖字符串，反射机制就是不停地执行字符串搜索。使用反射调用成员也影响性能。例如反射调用方法时，先将实参打包成数组，然后在内部解包到线程栈上。最后，调用前还有检查实参正确的数据类型和安全权限
* 避免使用反射来访问字段或调用方法。建议让类型从编译时已知的类型派生或实现已知的接口。
* 发现程序集中定义的类型：遍历Assembly.ExportedTypes属性。
``` C#
    Type typeReference = typeof(string);
    TypeInfo typeDefinition = typeReference.GetTypeInfo();
    Type typeRef=typeDefinition.AsType(); 
```
 * 泛型构造函数实例，利用以下机制，可为除数组（System.Array派生类型）和委托(System.MulticastDelegate派生类型)之外的所有类型创建对象。
     1. System.Activator的CreateInstance方法；
     2. System.Activator的CreateInstanceFrom方法；
     3. System.AppDomain里的CreateInstance，CreateInstanceAndUnwrap，CreateInstanceFrom和CreateInstanceFromAndUnwrap方法
     4. System.Reflection.Constructorinfo的invoke实例方法
* 数组实例的创建：System.Array.CreateInstance();
* 委托实例创建：MethodInfo的静态CreateDelegate方法。

``` C
    public static class ConstructingGenericType
    {
        private sealed class Dictionary<TKey, TValue> { }
        public static void Go()
        {
            //获取泛型的类型对象的引用
            Type openType = typeof(Dictionary<,>);

            //使用TKey=String， TValue=int 封闭泛型类型
            Type closedType = openType.MakeGenericType(typeof(string), typeof(int));

            //构造封闭类型实例
            object o = Activator.CreateInstance(closedType);

            //验证构造的类型可以工作
            Console.WriteLine(o.GetType());
        }
    }
```
* 使用反射发现类型的成员
![Reflection](https://cdn.jsdelivr.net/gh/HolaAmigoV5/Images/DotNet/Reflection.png)
```C#
 public static class MemberDiscover
    {
        public static void Go()
        {
            //遍历当前AppDomain中加载的所有程序集
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly a in assemblies)
            {
                Show(0, "Assembly : {0}", a);
                //查找程序集中的类型
                foreach (Type t in a.ExportedTypes)
                {
                    Show(1, "Type : {0}", t);
                    //发现类型成员
                    foreach (MemberInfo mi in t.GetTypeInfo().DeclaredMembers)
                    {
                        string typeName = string.Empty;
                        if (mi is Type) typeName = "(Nested) Type"; //嵌套类型
                        if (mi is FieldInfo) typeName = "FieldInfo";
                        if (mi is MethodInfo) typeName = "MethodInfo";
                        if (mi is ConstructorInfo) typeName = "ConstrucorInfo";
                        if (mi is PropertyInfo) typeName = "PropertyInfo";
                        if (mi is EventInfo) typeName = "EventInfo";
                        Show(2, "{0}: {1}", typeName, mi);
                    }
                }
            }
        }
        private static void Show(int indent, string format, params object[] args)
        {
            Console.WriteLine(new string(' ', 3 * indent) + format, args);
        }
    }
```
* 反射的使用
```C#
            Type ctorArgument = Type.GetType("System.Int32&"); //相当于以下代码
            //Type ctoArgument2 = typeof(Int32).MakeByRefType();

            //构造实例
            ConstructorInfo ctor = t.GetTypeInfo().DeclaredConstructors
                .First(c => c.GetParameters()[0].ParameterType == ctorArgument);
            object[] args = new object[] { 12 }; //构造器的实参
            object obj = ctor.Invoke(args);  //构造器调用

            //读写字段
            FieldInfo fi = obj.GetType().GetTypeInfo().GetDeclaredField("m_someField");
            fi.SetValue(obj, 33);  //字段写值

            //调用方法
            MethodInfo mi = obj.GetType().GetTypeInfo().GetDeclaredMethod("ToString");
            string s = (string)mi.Invoke(obj, null);

            //调用属性
            PropertyInfo pi = obj.GetType().GetTypeInfo().GetDeclaredProperty("SomeProp");
            try
            {
                pi.SetValue(obj, 0, null);
            }
            catch (TargetInvocationException e)
            {
                Console.WriteLine("Property set catch " + e.Message);
            }
            pi.SetValue(obj, 2, null);

            //为事件添加和删除委托
            EventInfo ei = obj.GetType().GetTypeInfo().GetDeclaredEvent("SomeEvent");
            EventHandler eh = new EventHandler(EventCallback); //包装了回调方法的委托

            ei.AddEventHandler(obj, eh);
            ei.RemoveEventHandler(obj, eh);
            
             //委托调用
            MethodInfo mi = obj.GetType().GetTypeInfo().GetDeclaredMethod("ToString");
            var toString = mi.CreateDelegate<Func<string>>(obj);
            string s = toString();

             //读写属性
            PropertyInfo pi = obj.GetType().GetTypeInfo().GetDeclaredProperty("SomeProp");
            var setSomeProp = pi.SetMethod.CreateDelegate<Action<int>>(obj);
            try
            {
                setSomeProp(0);
            }
            catch (ArgumentOutOfRangeException e)
            {
                Console.WriteLine("Property set catch " + e.Message);
            }
            setSomeProp(2);
            var getSomeProp = pi.GetMethod.CreateDelegate<Func<int>>(obj);
            Console.WriteLine("SomeProp: " + getSomeProp());

            //向事件增删委托
            EventInfo ei = obj.GetType().GetTypeInfo().GetDeclaredEvent("SomeEvent");
            var addEvent = ei.AddMethod.CreateDelegate<Action<EventHandler>>(obj);
            addEvent(EventCallback);
            var reEvent = ei.RemoveMethod.CreateDelegate<Action<EventHandler>>(obj);
            reEvent(EventCallback);
            
        private static void UseDynamicToBindAndInvokeTheMember(Type t)
        {
            //构造实例
            object[] args = new object[] { 12 };//构造器的实参
            dynamic obj = Activator.CreateInstance(t, args);

            //读写字段
            try
            {
                obj.m_someField = 5;  //无法读写私有字段m_someField
                int v = (int)obj.m_someField;
            }
            catch (RuntimeBinderException e)
            {
                Console.WriteLine("Failed to access field: " + e.Message);
            }

            //调用方法
            string s = (string)obj.ToString();
            Console.WriteLine("ToString: " + s);

            //读写属性
            try
            {
                obj.SomeProp = 0;
            }
            catch (ArgumentOutOfRangeException e)
            {
                Console.WriteLine("Property set catch " + e.Message);
            }

            obj.SomeProp = 2;
            int val = (int)obj.SomeProp;
            Console.WriteLine("SomeProp: " + val);

            //从事件增删委托
            obj.SomeEvent += new EventHandler(EventCallback);
            obj.SomeEvent -= new EventHandler(EventCallback);
        }


    //CreateDelegate方法的扩展
    internal static class ReflectionExtensions
    {
        public static TDelegate CreateDelegate<TDelegate>
        (this MethodInfo mi, object target = null)
        {
            return (TDelegate)(object)mi.CreateDelegate(typeof(TDelegate), target);
        }
    }
```

### 运行时序列化
**重点知识**
*  **序列化**是将对象或对象图转换成字节流的过程，`formatter.Serialize(stream, objectGraph);`。**反序列化**是将字节流转换回对象或对象图的过程，`formatter.Deserialize(stream, objectGraph);`。序列化对象只需调用格式化器的Serialize方法，并向它提供流对象的引用(stream)和要序列化对象的引用(objectGraph)。流对象标识了序列化好的字节应该放在哪里，从System.IO.Stream抽象基类派生的任何类型对象。一个流对象可以序列化多个对象。
* FCL提供两个格式化器：BinaryFormatter和SoapFormatter(.net Framework 3.5后废弃使用)。
* SerializableAttribute只能应用于引用类型(class)，值类型(struct)，枚举类型(enum)和委托类型(delegate)。枚举和委托总是可序列化的，所以不必显示应用SerializableAttribute特性，同时该特性不会被继承。
```C#
        //序列化
        private static MemoryStream SerializeToMemory(Object objectGraph)
        {
            //构造流来容纳序列化对象
            MemoryStream stream = new MemoryStream();
            //构造序列化格式化器来执行所有真正的工作
            BinaryFormatter formatter = new BinaryFormatter();
            //告诉格式化器将对象序列化到流中
            formatter.Serialize(stream, objectGraph);
            return stream;
        }
        
        //反序列化
        private static object DeserializeFromMemory(Stream stream)
        {
            //构造序列化格式化器来做所有真正的工作
            BinaryFormatter formatter = new BinaryFormatter();
            return formatter.Deserialize(stream);
        }
```
* System.NonSerializedAttribute特性指出类型中不应序列化的字段。
* System.Runtime.Serialization.OnDeserializedAttribute、OnDeserializingAttribute、OnSerializedAttibute、OnSerializingAttribute四个特性，分别用于序列化和反序列化前后的操作。这4个特性定义的方法必须获取一个StreamingContext参数并返回void。方法名是你希望的任何名称，并声明为private，以免其他代码调用。
```C#
    [OnSerialized]
   private void OnSerialized(StreamingContext context) 
   {
         //在序列化后，恢复任何需要恢复的状态
   }
```
* 如果序列化类型的实例，在类型中添加新字段，然后试图反序列化不包含新字段的对象，格式化器会抛出SerializationException异常。解决此问题可以使用OptionalFieldAttribute特性。
*  **格式化器如何序列化**：
    1. 格式化器调用**FormatterServices**的GetSerializableMembers方法。
    `public static MemberInfo[] GetSerializableMembers(Type type, StreamingContext context);` 这个方法利用反射获取类型的未标记NonSerializedAttribute特性的public和private字段，方法返回MemberInfo对象构成的数组，其中每个元素对应一个可序列化的实例字段。
    2. 将上一步返回的MemberInfo[]数组传给FormatterServices的静态方法GetObjectData：
    `public static Object[] GetObjectData(object obj, MemberInfo[] members);` 该方法返回Object数组，其中保存的是MemberInfo[]对应对象的值。
    3. 格式化器将程序集标识和类型的完整名称写入流中。
    4. 格式化器然后遍历两个数组中的元素，将每个成员的名称和值写入流中。
* **格式化器如何反序列化**：
    1. 格式化器从流中读取程序集标识和完整类型名称。然后调用FormatterServices的静态方法GetTypeFromAssembly返回System.Type对象，它代表要反序列化的那个对象的类型
    `public static Type GetTypeFromAssembly(Assembly assem, string name);`
    2. 格式化器调用FormatterServices的静态方法GetUninitializedObjec创建新对象：
    `public static object GetUninitializedObject(Type type);`
    3. 格式化器现在构造并初始化一个MemberInfo数组。同序列化一样调用GetSerializableMembers方法。
    4. 格式化器根据流中包含的数据创建并初始化一个Object数组；
    5. 将新分配对象、MemberInfo数组及并行的Object数组(值对象)传给FormatterServices的静态方法PopulateObjectMembers构建反序列化后的对象`
    public static Object PopulateObjectMembers(Object obj, MemberInfo[] members Object[] data)`

## 线程处理
### 线程基础
**重点知识**
*  **进程**实际是应用程序的实例要使用资源的集合。每个进程都被赋予一个虚拟地址空间，确保在一个进程中使用的代码和数据无法由另一个进程访问。此外，进程访问不了OS的内核代码和数据；所以，应用程序代码破坏不了操作系统代码和数据。
*  **线程**的职责是对CPU进行虚拟化。是CPU调度的最小单元。线程的开销主要来源自身的内存占用和上下文切换开销。创建进程十分“昂贵”，创建线程比较“廉价”
* 每个线程都分配了从0（最低）到31（最高）的优先级。线程有7个优先级：Idle，Lowest，Below Normal, Normal, Above Normal, Highest和Time-Critical。
* 进程有6个优先级：Idle, Below Normal, Normal, Above Normal, High和Realtime。进程优先级和线程优先级组合成1-31优先级表。
![Thread](https://cdn.jsdelivr.net/gh/HolaAmigoV5/Images/DotNet/Thread.png)
* **上表说明**
    * 系统启动时会创建一个零页线程（Zero page thread），该线程的优先级是0。
    * 17,18,19,20,21,27,28,29或者30：以内核模式运行的设备驱动程序才能获得这些优先级；
    * Realtime进程优先级下，线程优先级不能低于16，非Realtime不能高于16。
* 线程实现`Thread t=new Thread(()=>Console.WriteLine("Hello Thread")); t.Start();`
### 计算限制的异步操作
**重点知识**
* CLR初始化时，线程池中没有线程。内部维护一个请求队列，执行异步操作时，将记录项(entry)追加到线程池队列，创建新线程，用完后不销毁线程，而是返回线程池进入空闲状态等待调用。`System.Threading.ThreadPool`
` ThreadPool.QueueUserWorkItem(()=>Console.WriteLine("Hello"));`
* 默认情况下，CLR自动造成初始线程的执行上下文“流向”任何辅助线程。`CallContext.LogicalSetData("Name","Jeff");  CallContext.LogicalGetData("Name"); ExecutionContext.SuppresFlow(); ExecutionContext.RestoreFlow()`
* 协作式取消操作：先创建CancellationTokenSource对象，然后调用Token属性的Register注册取消时的回调方法。
```C#
    public static void RegisterGo()
    {
            var cts = new CancellationTokenSource();
            //注册取消回调方法
            cts.Token.Register(() => Console.WriteLine("Canceled 1"));
            cts.Token.Register(() => Console.WriteLine("Canceled 2"));
            
            //出于测试目的，让我们取消它
            cts.Cancel();
     }
```
* 任务的引入解决了Thread.QueueUserWorkItem调用无返回值和操作完成状态的跟踪问题。`System.Threading.Tasks`
``` C#
    ThreadPool.QueueUserWorkItem(ComputeBoundOp, 5);  
    new Task(ComputeBoundOp, 6).Start();  //用Task实现
    Task.Run(()=>ComputeBoundOp(5));    //等价写法
```
* 任务的取消
```C#
    var cts=new CancellationTokenSource();
    Task<int> t= Task.Run(()=>Sum(cts.Token, 1000), cts.Token);
    cts.Cancel();
    
    private static int Sum(CancellationToken ct, int n) {
        int sum=0;
        for(; n>0; n--)
        {
            //如果取消抛出OperationCancelledException异常
            ct.ThrowIfCancellationRequested(); 
            checked {sum+=n; }  //如果n太大，抛出System.OverflowException
        }
        return sum;
    }
```
* 任务完成时自动启动新任务和启动子任务
```C#
   private static void ParentChild()
        {
            Task<int[]> parent = new Task<int[]>(() =>
            {
                var results = new int[3];
                new Task(() => results[0] = Sum(1000), TaskCreationOptions.AttachedToParent).Start();
                new Task(() => results[1] = Sum(2000), TaskCreationOptions.AttachedToParent).Start();
                new Task(() => results[2] = Sum(3000), TaskCreationOptions.AttachedToParent).Start();
                return results;
            });
            var cwt = parent.ContinueWith(parentTask => Array.ForEach(parentTask.Result, Console.WriteLine));
            parent.Start();
            cwt.Wait();
        }
```
* 任务状态：`if(task.status==TaskStatus.RanToCompletion/Created/WaitingForActiovation/Canceled/Faulted..)`
* 可以通过创建TaskFactory对象包装多个有共同配置的Task进行操作
```C#
    Task parent = new Task(() =>
    {
        var cts = new CancellationTokenSource();
        var tf = new TaskFactory<int>(cts.Token, TaskCreationOptions.AttachedToParent,
            TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);

        //这个任务创建并启动3个子任务
        var childTasks = new[] {
            tf.StartNew(()=>Sum(cts.Token,1000)),
            tf.StartNew(()=>Sum(cts.Token,2000)),
            tf.StartNew(()=>Sum(cts.Token,int.MaxValue))  //Too big, throws OverflowException
        };

        //任何子任务抛出异常，就取消其余子任务
        for (int task = 0; task < childTasks.Length; task++)
        {
            childTasks[task].ContinueWith(t => cts.Cancel(), TaskContinuationOptions.OnlyOnFaulted);
        }

        //所有子任务完成后，从未出错的任务获取返回的最大值，然后将最大值传给另一个任务来显示最大结果。
        tf.ContinueWhenAll(childTasks,completedTasks => completedTasks.
            Where(t => t.Status == TaskStatus.RanToCompletion).Max(t =>
                      t.Result), CancellationToken.None)
        .ContinueWith(t => Console.WriteLine("The maximum is: " + t.Result), 
                      TaskContinuationOptions.ExecuteSynchronously)
        .Wait();
       });

        //捕获并记录异常
        parent.ContinueWith(p =>
        {
            StringBuilder sb = new StringBuilder("The following exeception(s) occurred:" +  Environment.NewLine);
            foreach (var e in p.Exception.Flatten().InnerExceptions)
            {
                sb.AppendLine("  " + e.GetType().ToString());
            }
            Console.WriteLine(sb.ToString());
        }, TaskContinuationOptions.OnlyOnFaulted);

        //启动父任务，使它能启动子任务。
        parent.Start();
        try
        {
            parent.Wait();
        }
        catch (AggregateException e)
        {
            Console.WriteLine(e.InnerException.Message);
        }
```
* Parallel的静态For, ForEach和Invoke的方法。调用Parallel前提是工作项必须能并行执行！要避免修改任何共享数据项，否则可能损坏数据。如果只是区区几个工作项使用Parallel的方法，有点得不偿失，反而降低了性能。
``` C#
    //The thread pool's threads process the work in parallel
   Parallel.For(0, 1000, i => Console.WriteLine("DoWork {0}", i));
   Parallel.ForEach(collection, item => DoWork(item));
   Parallel.Invoke(
                () => Console.WriteLine("Method1"), 
                () => Console.WriteLine("Method2"), 
                () => Console.WriteLine("Method3"));
```
* 语言集成查询（Language Integrated Query, LINQ）提供简洁的语法查询数据集合。
* 只有一个线程顺序处理数据集合中的所有项，我们称为顺序查询（sequential query）。要提高处理性能，可以使用并行LINQ（Parallel LINQ）。
* System.Linq.ParallelEnumerable类实现了LINQ的所有功能。AsParallel(), AsSequential()
```C#
    public static void Go()
    {
        ObsoleteMethods(typeof(object).Assembly);
    }

    //查找一个程序集中定义的所有过时(obsolete)方法
    private static void ObsoleteMethods(Assembly assembly)
    {
        var query = from type in assembly.GetExportedTypes().AsParallel()
            from method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
                    let obsoleteAttrType = typeof(ObsoleteAttribute)
                    where Attribute.IsDefined(method, obsoleteAttrType)
                    orderby type.FullName
            let obsoleteAttrObj =(ObsoleteAttribute)Attribute.GetCustomAttribute(method, obsoleteAttrType)
            select string.Format("Type={0}\nMethod={1}\nMessage={2}\n", 
            type.FullName, method.ToString(), obsoleteAttrObj.Message);

        //foreach (var result in query)
        //{
        //    Console.WriteLine(result);
        //}

        query.ForAll(result => Console.WriteLine(result));
        //query.ForAll(Console.WriteLine);
    }
```

### I/O限制的异步操作
**重点知识**
* async和await的使用，异步函数
* 异步函数（asynic/await）的工作原理：创建状态机，状态机维护工作进度，最终输出结果。调用awaiter对象的IsCompleted属性，来更新进度m_state，接着调用OnCompleted方法并传递委托（包装了MoveNext方法）。
* 异步编程模型(APM）：BeginXXX/EndXxx方法和IAsyncResult接口
* 基于事件的编程模型。现在这两个编程模型已经过时，使用Task的新模型才是首要选择。
* SynchronizationContext类。ConfigureAwait(false).
```C#
    private async Task<string> GetHTTP（）{
        HttpResponseMessage msg=await new HttpClient().GetAsync("http://www.baidu.com")
        .configureAwait(false);
            return await msg.Content.ReadAsStringAsync().ConfigureAwait(false);
        });
    }
    
    private Task<string> GetHTTP（）{
        return Task.Run(async ()=>{
            HttpResponseMessage msg=await new HttpClient().GetAsync("http://www.baidu.com");
            return await msg.Content.ReadAsStringAsync();
        });
    }
```

### 基元线程同步构造
**重点知识**
* 线程同步会造成性能下降（① 锁的创建和释放 ② 新线程的创建 ③ 上下文切换），应该避免使用线程同步。
* 由于JIT编译器和CPU会对代码进行优化，时常会出现调试没问题（调试不优化），发布后出现问题。当线程通过共享内存互相通信时，调用System.Threading.Volatile.Write(易变构造)来写入最后一个值，调用Volatile.Read来读取第一个值。
* volatile关键字作用：在多线程环境下，被volatile修饰的变量，其值被多线程读写时永远操作的是最新值。多个线程同时访问一个变量时，CLR为了效率会进行相应优化，比如“允许线程进行本地缓存”，这样就可能导致变量访问的不一致。volatile修饰的变量，不允许线程进行本地缓存，每个线程的读写都直接操作在共享内存上，这就保证了变量始终具有一致性。
```C#
    internal sealed class ThreadSharingData {
        private int m_flag=0;
        private int m_value=0;
        
        public void Thread1() {
            m_value =5;  //1写入m_flag之前，必须先将5写入m_value
            Volatile.Write(ref m_flag,1);
        }
        
        public void Thread2() {
            if(Volatile.Read(ref m_flag)==1)
                Console.WriteLine (m_value);
        }
    }
```
* 互锁构造：System.Threading.Interlocked类中的每个方法都执行一次原子读取和写入操作。主要用于int上。
* 自旋锁：只应该用于保护那些会执行非常快的代码区域。
* 线程同步能避免就尽量避免。如果一定要进行线程同步，就尽量使用用户模式的构造（慢），内核模式的构造要尽量避免（超慢）。
* 用户模式构造，内核模式构造，易变构造，互锁构造，自旋锁，Event构造，Semaphore构造，Mutex构造

### 混合线程同步构造
**重点知识**

* 合并了用户模式和内核模式的构造，我们称为混合线程同步构造。