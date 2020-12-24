using Castle.DynamicProxy;
using My.Repository;
using My.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;

namespace My.Business
{
    /// <summary>
    /// 描述：数据重复性校验
    /// 作者：wby 2019/11/18 10:58:33
    /// </summary>
    public class DataRepeatValidateAttribute : BaseFilterAttribute
    {
        private readonly bool _allData;
        private Dictionary<string, string> _validateFields = new Dictionary<string, string>();
        private readonly string[] _whereEqualFields;

        public DataRepeatValidateAttribute(string[] validateFields, string[] validateFieldNames, bool allData = false, string[] whereEqualFields = null)
        {
            if (validateFields.Length != validateFieldNames.Length)
                throw new Exception("校验列与描述信息不对应!");
            _allData = allData;
            for (int i = 0; i < validateFields.Length; i++)
            {
                _validateFields.Add(validateFields[i], validateFieldNames[i]);
            }
            _whereEqualFields = whereEqualFields;
        }

        public override void OnActionExecuted(IInvocation invocation)
        {
            throw new NotImplementedException();
        }

        public override void OnActionExecuting(IInvocation invocation)
        {
            Type entityType = invocation.Arguments[0].GetType();
            var data = invocation.Arguments[0];
            List<string> wherelist = new List<string>();
            var propertities = _validateFields.Where(x => !data.GetPropertyValue(x.Key).IsNullOrEmpty()).ToList();
            propertities.ForEach((aProperty, index) =>
            {
                wherelist.Add($"{aProperty.Key}=@{index}");
            });

            IQueryable q = null;
            if (_allData)
            {
                var repository = invocation.InvocationTarget.GetPropertyValue("Service") as IRepository;
                q = repository.GetIQueryable(entityType);
            }
            else
                q = invocation.InvocationTarget.GetType().GetMethod("GetIQueryable").
                    Invoke(invocation.InvocationTarget, new object[] { }) as IQueryable;
            q = q.Where(string.Join("||", wherelist), propertities.Select(x => data.GetPropertyValue(x.Key)).ToArray());

            var list = q.CastToList<object>();
            if (list.Count > 0)
            {
                var repeatList = propertities.Where(x => list.Any(y => !y.GetPropertyValue(x.Key).IsNullOrEmpty()))
                    .Select(x => x.Value).ToList();

                invocation.ReturnValue = new ErrorResult($"{string.Join(",", repeatList)}已存在!");
            }
        }
    }
}
