// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Utilities;

namespace NuoDb.EntityFrameworkCore.NuoDb.Query.Internal
{
    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public class NuoDbQuerySqlGenerator : QuerySqlGenerator
    {
       
        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public NuoDbQuerySqlGenerator(QuerySqlGeneratorDependencies dependencies)
            : base(dependencies)
        {
            
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        protected override string GetOperator(SqlBinaryExpression binaryExpression)
        {
            Check.NotNull(binaryExpression, nameof(binaryExpression));

            return binaryExpression.OperatorType == ExpressionType.Add
                && binaryExpression.Type == typeof(string)
                    ? " || "
                    : base.GetOperator(binaryExpression);
        }

        public virtual Expression VisitNuoDbComplexFunctionArgumentExpression(NuoDbComplexFunctionArgumentExpression nuoDbComplexFunctionArgumentExpression)
        {
            Check.NotNull(nuoDbComplexFunctionArgumentExpression, nameof(nuoDbComplexFunctionArgumentExpression));

            var first = true;
            foreach (var argument in nuoDbComplexFunctionArgumentExpression.ArgumentParts)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    Sql.Append(nuoDbComplexFunctionArgumentExpression.Delimiter);
                }

                Visit(argument);
            }

            return nuoDbComplexFunctionArgumentExpression;
        }


        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        protected override void GenerateLimitOffset(SelectExpression selectExpression)
        {
            Check.NotNull(selectExpression, nameof(selectExpression));

            if (selectExpression.Limit != null
                || selectExpression.Offset != null)
            {
                
                Sql.AppendLine();
                if (selectExpression.Limit != null)
                {
                    Sql.Append("Limit ");
                    Visit(selectExpression.Limit);
                }

                if (selectExpression.Offset != null)
                {
                    Sql.Append(" OFFSET ");
                    Visit(selectExpression.Offset);
                }
            }
        }

        // protected override Expression VisitSqlFragment(SqlFragmentExpression sqlFragmentExpression)
        // {
        //     Check.NotNull(sqlFragmentExpression, nameof(sqlFragmentExpression));
        //
        //     return sqlFragmentExpression;
        // }

        protected override Expression VisitSqlUnary(SqlUnaryExpression sqlUnaryExpression)
            => sqlUnaryExpression.OperatorType == ExpressionType.Convert
                ? VisitConvert(sqlUnaryExpression)
                : base.VisitSqlUnary(sqlUnaryExpression);


        private SqlUnaryExpression VisitConvert(SqlUnaryExpression sqlUnaryExpression)
        {

            Visit(sqlUnaryExpression.Operand);

            return sqlUnaryExpression;
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        protected override void GenerateSetOperationOperand(SetOperationBase setOperation, SelectExpression operand)
        {
            Check.NotNull(setOperation, nameof(setOperation));
            Check.NotNull(operand, nameof(operand));

            // NuoDb doesn't support parentheses around set operation operands
            Visit(operand);
        }

        protected override void GeneratePseudoFromClause()
        {
            Sql.Append(" FROM DUAL");
        }

        protected override void CheckComposableSql(string sql)
        {

        }
    }
}
