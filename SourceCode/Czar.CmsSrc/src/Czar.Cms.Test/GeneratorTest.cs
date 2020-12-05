
using Czar.Cms.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;
using System.Linq;
using Czar.Cms.IRepository;
using Czar.Cms.Repository.SqlServer;
using Czar.Cms.Core.Models;
using Czar.Cms.Core.CodeGenerator;
using Czar.Cms.Core.Options;

namespace Czar.Cms.Test
{
    /// <summary>
    /// yilezhu
    /// 2018.12.12
    /// ���Դ���������
    /// ��ʱֻʵ����SqlServer��ʵ���������
    /// </summary>
    public class GeneratorTest
    {
        [Fact]
        public void GeneratorModelForSqlServer()
        {
            var serviceProvider = Common.BuildServiceForSqlServer();
            var codeGenerator = serviceProvider.GetRequiredService<CodeGenerator>();
            codeGenerator.GenerateTemplateCodesFromDatabase(true);
            Assert.Equal("SQLServer", DatabaseType.SqlServer.ToString(), ignoreCase: true);

        }

      
    }
}
