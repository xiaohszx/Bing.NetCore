﻿using System;
using Bing.Datas.Dapper.SqlServer;
using Bing.Datas.Sql.Queries.Builders.Clauses;
using Bing.Datas.Sql.Queries.Builders.Core;
using Bing.Datas.Test.Integration.Samples;
using Bing.Utils;
using Bing.Utils.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace Bing.Datas.Test.Integration.Dapper.SqlServer.Clauses
{
    /// <summary>
    /// 表连接子句测试
    /// </summary>
    public class JoinClauseTest:TestBase
    {
        /// <summary>
        /// Join子句
        /// </summary>
        private readonly JoinClause _clause;

        /// <summary>
        /// 测试初始化
        /// </summary>
        public JoinClauseTest(ITestOutputHelper output) : base(output)
        {
            _clause = new JoinClause(new SqlServerBuilder(), new SqlServerDialect(), new EntityResolver(), new EntityAliasRegister());
        }

        /// <summary>
        /// 获取Sql
        /// </summary>
        private string GetSql()
        {
            return _clause.ToSql();
        }

        /// <summary>
        /// 表连接
        /// </summary>
        [Fact]
        public void Test_Join_1()
        {
            _clause.Join("a");
            Assert.Equal("Join [a]", GetSql());
        }

        /// <summary>
        /// 表连接 - 架构
        /// </summary>
        [Fact]
        public void Test_Join_2()
        {
            _clause.Join("a.b");
            Assert.Equal("Join [a].[b]", GetSql());
        }

        /// <summary>
        /// 表连接 - 架构 - 别名
        /// </summary>
        [Fact]
        public void Test_Join_3()
        {
            _clause.Join("a.b as c");
            Assert.Equal("Join [a].[b] As [c]", GetSql());
        }

        /// <summary>
        /// 表连接 - 架构 - 别名
        /// </summary>
        [Fact]
        public void Test_Join_4()
        {
            _clause.Join("a.b", "c");
            Assert.Equal("Join [a].[b] As [c]", GetSql());
        }

        /// <summary>
        /// 表连接 - 泛型实体
        /// </summary>
        [Fact]
        public void Test_Join_5()
        {
            _clause.Join<Sample>();
            Assert.Equal("Join [Sample]", GetSql());
        }

        /// <summary>
        /// 表连接 - 泛型实体 - 别名
        /// </summary>
        [Fact]
        public void Test_Join_6()
        {
            _clause.Join<Sample>("a");
            Assert.Equal("Join [Sample] As [a]", GetSql());
        }

        /// <summary>
        /// 表连接 - 泛型实体 - 别名 - 架构
        /// </summary>
        [Fact]
        public void Test_Join_7()
        {
            _clause.Join<Sample>("a", "b");
            Assert.Equal("Join [b].[Sample] As [a]", GetSql());
        }

        /// <summary>
        /// 表连接 - 设置两个Join
        /// </summary>
        [Fact]
        public void Test_Join_8()
        {
            //结果
            var result = new Str();
            result.AppendLine("Join [a] ");
            result.Append("Join [b]");

            //操作
            _clause.Join("a");
            _clause.Join("b");

            //验证
            Assert.Equal(result.ToString(), GetSql());
        }

        /// <summary>
        /// 表连接
        /// </summary>
        [Fact]
        public void Test_Join_9()
        {
            //结果
            var result = new Str();
            result.AppendLine("Join a ");
            result.Append("Join b");

            //操作
            _clause.AppendJoin("a");
            _clause.AppendJoin("b");

            //验证
            Assert.Equal(result.ToString(), GetSql());
        }

        /// <summary>
        /// 表连接
        /// </summary>
        [Fact]
        public void Test_Join_10()
        {
            //结果
            var result = new Str();
            result.AppendLine("Join [a] ");
            result.Append("Join b");

            //操作
            _clause.Join("a");
            _clause.AppendJoin("b");

            //验证
            Assert.Equal(result.ToString(), GetSql());
        }

        /// <summary>
        /// 表连接条件 - 未设置join返回空
        /// </summary>
        [Fact]
        public void Test_On_1()
        {
            _clause.On("a.id", "b.id");
            Assert.Empty(GetSql());
        }

        /// <summary>
        /// 表连接条件
        /// </summary>
        [Fact]
        public void Test_On_2()
        {
            //结果
            var result = new Str();
            result.Append("Join [t] ");
            result.Append("On [a].[id]=[b].[id]");

            //操作
            _clause.Join("t");
            _clause.On("a.id", "b.id");

            //验证
            Assert.Equal(result.ToString(), GetSql());
        }

        /// <summary>
        /// 表连接条件 - 多个On
        /// </summary>
        [Fact]
        public void Test_On_3()
        {
            //结果
            var result = new Str();
            result.Append("Join [t] ");
            result.Append("On [a].[id]=[b].[id] And [c].[Aid]=[d].[Bid]");

            //操作
            _clause.Join("t");
            _clause.On("a.id", "b.id");
            _clause.On("c.Aid", "d.Bid");

            //验证
            Assert.Equal(result.ToString(), GetSql());
        }

        /// <summary>
        /// 表连接条件 - 多个Join和On
        /// </summary>
        [Fact]
        public void Test_On_4()
        {
            //结果
            var result = new Str();
            result.Append("Join [t] ");
            result.AppendLine("On [a].[id]=[b].[id] And [c].[Aid]=[d].[Bid] ");
            result.Append("Join [n] ");
            result.Append("On [t].[id]=[n].[id] And [t].[Aid]=[n].[Bid]");

            //操作
            _clause.Join("t");
            _clause.On("a.id", "b.id");
            _clause.On("c.Aid", "d.Bid");
            _clause.Join("n");
            _clause.On("t.id", "n.id");
            _clause.On("t.Aid", "n.Bid");

            //验证
            Assert.Equal(result.ToString(), GetSql());
        }

        /// <summary>
        /// 表连接条件 - 设置运算符
        /// </summary>
        [Fact]
        public void Test_On_5()
        {
            //结果
            var result = new Str();
            result.Append("Join [t] ");
            result.Append("On [a].[id]<[b].[id]");

            //操作
            _clause.Join("t");
            _clause.On("a.id", "b.id", Operator.Less);

            //验证
            Assert.Equal(result.ToString(), GetSql());
        }

        /// <summary>
        /// 表连接条件 - 实体
        /// </summary>
        [Fact]
        public void Test_On_6()
        {
            //结果
            var result = new Str();
            result.Append("Join [Sample] ");
            result.AppendLine("On [a].[id]=[b].[id] ");
            result.Append("Join [Sample2] ");
            result.Append("On [Sample].[BoolValue]<[Sample2].[IntValue]");

            //操作
            _clause.Join<Sample>();
            _clause.On("a.id", "b.id");
            _clause.Join<Sample2>();
            _clause.On<Sample, Sample2>(t => t.BoolValue, t => t.IntValue, Operator.Less);

            //验证
            Assert.Equal(result.ToString(), GetSql());
        }

        /// <summary>
        /// 表连接条件 - 实体
        /// </summary>
        [Fact]
        public void Test_On_7()
        {
            //结果
            var result = new Str();
            result.Append("Join [Sample] As [t] ");
            result.AppendLine("On [a].[id]=[b].[id] ");
            result.Append("Join [Sample2] As [t2] ");
            result.Append("On [t].[BoolValue]<[t2].[IntValue]");

            //操作
            _clause.Join<Sample>("t");
            _clause.On("a.id", "b.id");
            _clause.Join<Sample2>("t2");
            _clause.On<Sample, Sample2>(t => t.BoolValue, t => t.IntValue, Operator.Less);

            //验证
            Assert.Equal(result.ToString(), GetSql());
        }

        /// <summary>
        /// 表连接条件 - 谓词表达式
        /// </summary>
        [Fact]
        public void Test_On_8()
        {
            //结果
            var result = new Str();
            result.Append("Join [Sample] ");
            result.AppendLine("On [a].[id]=[b].[id] ");
            result.Append("Join [Sample2] ");
            result.Append("On [Sample].[ShortValue]>[Sample2].[IntValue]");

            //操作
            _clause.Join<Sample>();
            _clause.On("a.id", "b.id");
            _clause.Join<Sample2>();
            _clause.On<Sample, Sample2>((l, r) => l.ShortValue > r.IntValue);

            //验证
            Assert.Equal(result.ToString(), GetSql());
        }

        /// <summary>
        /// 表连接条件 - 谓词表达式 - 别名
        /// </summary>
        [Fact]
        public void Test_On_9()
        {
            //结果
            var result = new Str();
            result.Append("Join [Sample] As [t] ");
            result.AppendLine("On [a].[id]=[b].[id] ");
            result.Append("Join [Sample2] As [t2] ");
            result.Append("On [t].[ShortValue]>[t2].[IntValue]");

            //操作
            _clause.Join<Sample>("t");
            _clause.On("a.id", "b.id");
            _clause.Join<Sample2>("t2");
            _clause.On<Sample, Sample2>((l, r) => l.ShortValue > r.IntValue);

            //验证
            Assert.Equal(result.ToString(), GetSql());
        }

        /// <summary>
        /// 表连接条件 - 谓词表达式 - 与运算
        /// </summary>
        [Fact]
        public void Test_On_10()
        {
            //结果
            var result = new Str();
            result.Append("Join [Sample] As [t] ");
            result.AppendLine("On [a].[id]=[b].[id] ");
            result.Append("Join [Sample2] As [t2] ");
            result.Append("On [t].[ShortValue]>[t2].[IntValue] And [t].[DisplayValue]=[t2].[StringValue]");

            //操作
            _clause.Join<Sample>("t");
            _clause.On("a.id", "b.id");
            _clause.Join<Sample2>("t2");
            _clause.On<Sample, Sample2>((l, r) => l.ShortValue > r.IntValue && l.DisplayValue == r.StringValue);

            //验证
            Assert.Equal(result.ToString(), GetSql());
        }

        /// <summary>
        /// 表连接条件 - 谓词表达式 - 或运算
        /// </summary>
        [Fact]
        public void Test_On_11()
        {
            //结果
            var result = new Str();
            result.Append("Join [Sample] As [t] ");
            result.AppendLine("On [a].[id]=[b].[id] ");
            result.Append("Join [Sample2] As [t2] ");
            result.Append("On [t].[ShortValue]>[t2].[IntValue] Or [t].[DisplayValue]=[t2].[StringValue]");

            //操作
            _clause.Join<Sample>("t");
            _clause.On("a.id", "b.id");
            _clause.Join<Sample2>("t2");
            _clause.On<Sample, Sample2>((l, r) => l.ShortValue > r.IntValue || l.DisplayValue == r.StringValue);

            //验证
            Assert.Equal(result.ToString(), GetSql());
        }

        /// <summary>
        /// 表连接条件 - 值为数字
        /// </summary>
        [Fact]
        public void Test_On_12()
        {
            //结果
            var result = new Str();
            result.Append("Join [Sample] As [t] ");
            result.AppendLine("On [a].[id]=[b].[id] ");
            result.Append("Join [Sample2] As [t2] ");
            result.Append("On [t].[ShortValue]>[t2].[IntValue] And [t].[IntValue]=1");

            //操作
            _clause.Join<Sample>("t");
            _clause.On("a.id", "b.id");
            _clause.Join<Sample2>("t2");
            _clause.On<Sample, Sample2>((l, r) => l.ShortValue > r.IntValue && l.IntValue == 1);

            //验证
            Assert.Equal(result.ToString(), GetSql());
        }

        /// <summary>
        /// 表连接条件 - 值为数字 - 数字在左边
        /// </summary>
        [Fact]
        public void Test_On_13()
        {
            //结果
            var result = new Str();
            result.Append("Join [Sample] As [t] ");
            result.AppendLine("On [a].[id]=[b].[id] ");
            result.Append("Join [Sample2] As [t2] ");
            result.Append("On [t].[ShortValue]>[t2].[IntValue] And 1=[t].[IntValue]");

            //操作
            _clause.Join<Sample>("t");
            _clause.On("a.id", "b.id");
            _clause.Join<Sample2>("t2");
            _clause.On<Sample, Sample2>((l, r) => l.ShortValue > r.IntValue && 1 == l.IntValue);

            //验证
            Assert.Equal(result.ToString(), GetSql());
        }

        /// <summary>
        /// 表连接条件 - 交换左右操作数
        /// </summary>
        [Fact]
        public void Test_On_14()
        {
            //结果
            var result = new Str();
            result.Append("Join [Sample] As [t] ");
            result.AppendLine("On [a].[id]=[b].[id] ");
            result.Append("Join [Sample2] As [t2] ");
            result.Append("On [t2].[IntValue]>[t].[ShortValue]");

            //操作
            _clause.Join<Sample>("t");
            _clause.On("a.id", "b.id");
            _clause.Join<Sample2>("t2");
            _clause.On<Sample, Sample2>((l, r) => r.IntValue > l.ShortValue);

            //验证
            Assert.Equal(result.ToString(), GetSql());
        }

        /// <summary>
        /// 左外连接
        /// </summary>
        [Fact]
        public void Test_LeftJoin_1()
        {
            _clause.LeftJoin("a");
            Assert.Equal("Left Join [a]", GetSql());
        }

        /// <summary>
        /// 左外连接
        /// </summary>
        [Fact]
        public void Test_LeftJoin_2()
        {
            _clause.Join("a");
            _clause.LeftJoin("b");
            Assert.Equal("Join [a] \r\nLeft Join [b]", GetSql());
        }

        /// <summary>
        /// 左外连接 - 泛型实体
        /// </summary>
        [Fact]
        public void Test_LeftJoin_3()
        {
            _clause.LeftJoin<Sample>();
            Assert.Equal("Left Join [Sample]", GetSql());
        }

        /// <summary>
        /// 左外连接
        /// </summary>
        [Fact]
        public void Test_LeftJoin_4()
        {
            //结果
            var result = new Str();
            result.AppendLine("Join a ");
            result.Append("Left Join b");

            //操作
            _clause.AppendJoin("a");
            _clause.AppendLeftJoin("b");

            //验证
            Assert.Equal(result.ToString(), GetSql());
        }

        /// <summary>
        /// 右外连接
        /// </summary>
        [Fact]
        public void Test_RightJoin_1()
        {
            _clause.RightJoin("a");
            Assert.Equal("Right Join [a]", GetSql());
        }

        /// <summary>
        /// 右外连接
        /// </summary>
        [Fact]
        public void Test_RightJoin_2()
        {
            _clause.Join("a");
            _clause.RightJoin("b");
            Assert.Equal("Join [a] \r\nRight Join [b]", GetSql());
        }

        /// <summary>
        /// 右外连接 - 泛型实体
        /// </summary>
        [Fact]
        public void Test_RightJoin_3()
        {
            _clause.RightJoin<Sample>();
            Assert.Equal("Right Join [Sample]", GetSql());
        }

        /// <summary>
        /// 右外连接
        /// </summary>
        [Fact]
        public void Test_RightJoin_4()
        {
            //结果
            var result = new Str();            
            result.AppendLine("Join a ");
            result.Append("Right Join b");

            //操作
            _clause.AppendJoin("a");
            _clause.AppendRightJoin("b");

            //验证
            Assert.Equal(result.ToString(), GetSql());
        }

        /// <summary>
        /// 复制
        /// </summary>
        [Fact]
        public void Test_Clone_1()
        {
            _clause.Join("b");
            _clause.On("a.A", "b.B");

            //复制副本
            var copy = _clause.Clone(null, null);
            Assert.Equal("Join [b] On [a].[A]=[b].[B]", GetSql());
            Assert.Equal("Join [b] On [a].[A]=[b].[B]", copy.ToSql());

            //修改副本
            copy.On("a.C", "b.D");
            Assert.Equal("Join [b] On [a].[A]=[b].[B]", GetSql());
            Assert.Equal("Join [b] On [a].[A]=[b].[B] And [a].[C]=[b].[D]", copy.ToSql());
        }
    }
}
