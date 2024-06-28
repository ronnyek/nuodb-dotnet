// <auto-generated />

using System;
using System.Reflection;
using System.Resources;
using System.Threading;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;

#nullable enable

namespace NuoDb.EntityFrameworkCore.NuoDb.Internal
{
    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public static class NuoDbStrings
    {
        private static readonly ResourceManager _resourceManager
            = new ResourceManager("NuoDb.EntityFrameworkCore.NuoDb.Properties.NuoDbStrings", typeof(NuoDbStrings).Assembly);

        /// <summary>
        ///     NuoDb cannot apply aggregate operator '{aggregateOperator}' on expressions of type '{type}'. Convert the values to a supported type, or use LINQ to Objects to aggregate the results on the client side.
        /// </summary>
        public static string AggregateOperationNotSupported(object? aggregateOperator, object? type)
            => string.Format(
                GetString("AggregateOperationNotSupported", nameof(aggregateOperator), nameof(type)),
                aggregateOperator, type);

        /// <summary>
        ///     To change the IDENTITY property of a column, the column needs to be dropped and recreated.
        /// </summary>
        public static string AlterIdentityColumn
            => GetString("AlterIdentityColumn");

        /// <summary>
        ///     Translating this query requires the SQL APPLY operation, which is not supported on NuoDb.
        /// </summary>
        public static string ApplyNotSupported
            => GetString("ApplyNotSupported");

        /// <summary>
        ///     NuoDb does not support Cross Joins
        /// </summary>
        public static string CrossJoinNotSupported
            => GetString("CrossJoinNotSupported");

        /// <summary>
        ///     '{entityType1}.{property1}' and '{entityType2}.{property2}' are both mapped to column '{columnName}' in '{table}', but are configured with different SRIDs.
        /// </summary>
        public static string DuplicateColumnNameSridMismatch(object? entityType1, object? property1, object? entityType2, object? property2, object? columnName, object? table)
            => string.Format(
                GetString("DuplicateColumnNameSridMismatch", nameof(entityType1), nameof(property1), nameof(entityType2), nameof(property2), nameof(columnName), nameof(table)),
                entityType1, property1, entityType2, property2, columnName, table);

        /// <summary>
        ///     NuoDb does not support Except set operations
        /// </summary>
        public static string ExceptNotSupported
            => GetString("ExceptNotSupported");

        /// <summary>
        ///     NuoDb does not support Intercept set operations
        /// </summary>
        public static string InterceptNotSupported
            => GetString("InterceptNotSupported");

        /// <summary>
        ///     NuoDb does not support this migration operation ('{operation}'). See http://go.microsoft.com/fwlink/?LinkId=723262 for more information.
        /// </summary>
        public static string InvalidMigrationOperation(object? operation)
            => string.Format(
                GetString("InvalidMigrationOperation", nameof(operation)),
                operation);

        /// <summary>
        ///     Generating idempotent scripts for migrations is not currently supported for NuoDb. See http://go.microsoft.com/fwlink/?LinkId=723262 for more information.
        /// </summary>
        public static string MigrationScriptGenerationNotSupported
            => GetString("MigrationScriptGenerationNotSupported");

        /// <summary>
        ///     NuoDb does not support expressions of type '{type}' in ORDER BY clauses. Convert the values to a supported type, or use LINQ to Objects to order the results on the client side.
        /// </summary>
        public static string OrderByNotSupported(object? type)
            => string.Format(
                GetString("OrderByNotSupported", nameof(type)),
                type);

        /// <summary>
        ///     NuoDb does not support sequences. See http://go.microsoft.com/fwlink/?LinkId=723262 for more information.
        /// </summary>
        public static string SequencesNotSupported
            => GetString("SequencesNotSupported");

        /// <summary>
        ///     NuoDb does not support subqueries in order by
        /// </summary>
        public static string SubqueriesInOrderByNotSupported
            => GetString("SubqueriesInOrderByNotSupported");

        private static string GetString(string name, params string[] formatterNames)
        {
            var value = _resourceManager.GetString(name)!;
            for (var i = 0; i < formatterNames.Length; i++)
            {
                value = value.Replace("{" + formatterNames[i] + "}", "{" + i + "}");
            }

            return value;
        }
    }
}

namespace NuoDb.EntityFrameworkCore.NuoDb.Internal
{
    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public static class NuoDbResources
    {
        private static readonly ResourceManager _resourceManager
            = new ResourceManager("NuoDb.EntityFrameworkCore.NuoDb.Properties.NuoDbStrings", typeof(NuoDbResources).Assembly);

        /// <summary>
        ///     Skipping foreign key with identity '{id}' on table '{tableName}' since principal table '{principalTableName}' was not found in the model. This usually happens when the principal table was not included in the selection set.
        /// </summary>
        public static EventDefinition<string?, string?, string?> LogForeignKeyScaffoldErrorPrincipalTableNotFound(IDiagnosticsLogger logger)
        {
            var definition = ((Diagnostics.Internal.NuoDbLoggingDefinitions)logger.Definitions).LogForeignKeyScaffoldErrorPrincipalTableNotFound;
            if (definition == null)
            {
                definition = NonCapturingLazyInitializer.EnsureInitialized(
                    ref ((Diagnostics.Internal.NuoDbLoggingDefinitions)logger.Definitions).LogForeignKeyScaffoldErrorPrincipalTableNotFound,
                    logger,
                    static logger => new EventDefinition<string?, string?, string?>(
                        logger.Options,
                        NuoDbEventId.ForeignKeyReferencesMissingTableWarning,
                        LogLevel.Warning,
                        "NuoDbEventId.ForeignKeyReferencesMissingTableWarning",
                        level => LoggerMessage.Define<string?, string?, string?>(
                            level,
                            NuoDbEventId.ForeignKeyReferencesMissingTableWarning,
                            _resourceManager.GetString("LogForeignKeyScaffoldErrorPrincipalTableNotFound")!)));
            }

            return (EventDefinition<string?, string?, string?>)definition;
        }

        /// <summary>
        ///     Found column on table '{tableName}' with name: '{columnName}', data type: {dataType}, not nullable: {notNullable}, default value: {defaultValue}.
        /// </summary>
        public static EventDefinition<string?, string?, string?, bool, string?> LogFoundColumn(IDiagnosticsLogger logger)
        {
            var definition = ((Diagnostics.Internal.NuoDbLoggingDefinitions)logger.Definitions).LogFoundColumn;
            if (definition == null)
            {
                definition = NonCapturingLazyInitializer.EnsureInitialized(
                    ref ((Diagnostics.Internal.NuoDbLoggingDefinitions)logger.Definitions).LogFoundColumn,
                    logger,
                    static logger => new EventDefinition<string?, string?, string?, bool, string?>(
                        logger.Options,
                        NuoDbEventId.ColumnFound,
                        LogLevel.Debug,
                        "NuoDbEventId.ColumnFound",
                        level => LoggerMessage.Define<string?, string?, string?, bool, string?>(
                            level,
                            NuoDbEventId.ColumnFound,
                            _resourceManager.GetString("LogFoundColumn")!)));
            }

            return (EventDefinition<string?, string?, string?, bool, string?>)definition;
        }

        /// <summary>
        ///     Found foreign key on table '{tableName}', id: {id}, principal table: {principalTableName}, delete action: {deleteAction}.
        /// </summary>
        public static EventDefinition<string, string, string, string> LogFoundForeignKey(IDiagnosticsLogger logger)
        {
            var definition = ((Diagnostics.Internal.NuoDbLoggingDefinitions)logger.Definitions).LogFoundForeignKey;
            if (definition == null)
            {
                definition = NonCapturingLazyInitializer.EnsureInitialized(
                    ref ((Diagnostics.Internal.NuoDbLoggingDefinitions)logger.Definitions).LogFoundForeignKey,
                    logger,
                    static logger => new EventDefinition<string, string, string, string>(
                        logger.Options,
                        NuoDbEventId.ForeignKeyFound,
                        LogLevel.Debug,
                        "NuoDbEventId.ForeignKeyFound",
                        level => LoggerMessage.Define<string, string, string, string>(
                            level,
                            NuoDbEventId.ForeignKeyFound,
                            _resourceManager.GetString("LogFoundForeignKey")!)));
            }

            return (EventDefinition<string, string, string, string>)definition;
        }

        /// <summary>
        ///     Found index on table '{tableName}' with name '{indexName}', is unique: {isUnique}.
        /// </summary>
        public static EventDefinition<string?, string?, bool?> LogFoundIndex(IDiagnosticsLogger logger)
        {
            var definition = ((Diagnostics.Internal.NuoDbLoggingDefinitions)logger.Definitions).LogFoundIndex;
            if (definition == null)
            {
                definition = NonCapturingLazyInitializer.EnsureInitialized(
                    ref ((Diagnostics.Internal.NuoDbLoggingDefinitions)logger.Definitions).LogFoundIndex,
                    logger,
                    static logger => new EventDefinition<string?, string?, bool?>(
                        logger.Options,
                        NuoDbEventId.IndexFound,
                        LogLevel.Debug,
                        "NuoDbEventId.IndexFound",
                        level => LoggerMessage.Define<string?, string?, bool?>(
                            level,
                            NuoDbEventId.IndexFound,
                            _resourceManager.GetString("LogFoundIndex")!)));
            }

            return (EventDefinition<string?, string?, bool?>)definition;
        }

        /// <summary>
        ///     Found primary key on table '{tableName}' with name {primaryKeyName}.
        /// </summary>
        public static EventDefinition<string?, string?> LogFoundPrimaryKey(IDiagnosticsLogger logger)
        {
            var definition = ((Diagnostics.Internal.NuoDbLoggingDefinitions)logger.Definitions).LogFoundPrimaryKey;
            if (definition == null)
            {
                definition = NonCapturingLazyInitializer.EnsureInitialized(
                    ref ((Diagnostics.Internal.NuoDbLoggingDefinitions)logger.Definitions).LogFoundPrimaryKey,
                    logger,
                    static logger => new EventDefinition<string?, string?>(
                        logger.Options,
                        NuoDbEventId.PrimaryKeyFound,
                        LogLevel.Debug,
                        "NuoDbEventId.PrimaryKeyFound",
                        level => LoggerMessage.Define<string?, string?>(
                            level,
                            NuoDbEventId.PrimaryKeyFound,
                            _resourceManager.GetString("LogFoundPrimaryKey")!)));
            }

            return (EventDefinition<string?, string?>)definition;
        }

        /// <summary>
        ///     Found sequence with '{name}'
        /// </summary>
        public static FallbackEventDefinition LogFoundSequence(IDiagnosticsLogger logger)
        {
            var definition = ((Diagnostics.Internal.NuoDbLoggingDefinitions)logger.Definitions).LogFoundSequence;
            if (definition == null)
            {
                definition = NonCapturingLazyInitializer.EnsureInitialized(
                    ref ((Diagnostics.Internal.NuoDbLoggingDefinitions)logger.Definitions).LogFoundSequence,
                    logger,
                    static logger => new FallbackEventDefinition(
                        logger.Options,
                        NuoDbEventId.SequenceFound,
                        LogLevel.Debug,
                        "NuoDbEventId.SequenceFound",
                        _resourceManager.GetString("LogFoundSequence")!));
            }

            return (FallbackEventDefinition)definition;
        }

        /// <summary>
        ///     Found table with name: '{name}'.
        /// </summary>
        public static EventDefinition<string?> LogFoundTable(IDiagnosticsLogger logger)
        {
            var definition = ((Diagnostics.Internal.NuoDbLoggingDefinitions)logger.Definitions).LogFoundTable;
            if (definition == null)
            {
                definition = NonCapturingLazyInitializer.EnsureInitialized(
                    ref ((Diagnostics.Internal.NuoDbLoggingDefinitions)logger.Definitions).LogFoundTable,
                    logger,
                    static logger => new EventDefinition<string?>(
                        logger.Options,
                        NuoDbEventId.TableFound,
                        LogLevel.Debug,
                        "NuoDbEventId.TableFound",
                        level => LoggerMessage.Define<string?>(
                            level,
                            NuoDbEventId.TableFound,
                            _resourceManager.GetString("LogFoundTable")!)));
            }

            return (EventDefinition<string?>)definition;
        }

        /// <summary>
        ///     Found unique constraint on table '{tableName}' with name: {uniqueConstraintName}.
        /// </summary>
        public static EventDefinition<string?, string?> LogFoundUniqueConstraint(IDiagnosticsLogger logger)
        {
            var definition = ((Diagnostics.Internal.NuoDbLoggingDefinitions)logger.Definitions).LogFoundUniqueConstraint;
            if (definition == null)
            {
                definition = NonCapturingLazyInitializer.EnsureInitialized(
                    ref ((Diagnostics.Internal.NuoDbLoggingDefinitions)logger.Definitions).LogFoundUniqueConstraint,
                    logger,
                    static logger => new EventDefinition<string?, string?>(
                        logger.Options,
                        NuoDbEventId.UniqueConstraintFound,
                        LogLevel.Debug,
                        "NuoDbEventId.UniqueConstraintFound",
                        level => LoggerMessage.Define<string?, string?>(
                            level,
                            NuoDbEventId.UniqueConstraintFound,
                            _resourceManager.GetString("LogFoundUniqueConstraint")!)));
            }

            return (EventDefinition<string?, string?>)definition;
        }

        /// <summary>
        ///     Unable to find a table in the database matching the selected table '{table}'.
        /// </summary>
        public static EventDefinition<string?> LogMissingTable(IDiagnosticsLogger logger)
        {
            var definition = ((Diagnostics.Internal.NuoDbLoggingDefinitions)logger.Definitions).LogMissingTable;
            if (definition == null)
            {
                definition = NonCapturingLazyInitializer.EnsureInitialized(
                    ref ((Diagnostics.Internal.NuoDbLoggingDefinitions)logger.Definitions).LogMissingTable,
                    logger,
                    static logger => new EventDefinition<string?>(
                        logger.Options,
                        NuoDbEventId.MissingTableWarning,
                        LogLevel.Warning,
                        "NuoDbEventId.MissingTableWarning",
                        level => LoggerMessage.Define<string?>(
                            level,
                            NuoDbEventId.MissingTableWarning,
                            _resourceManager.GetString("LogMissingTable")!)));
            }

            return (EventDefinition<string?>)definition;
        }

        /// <summary>
        ///     Skipping foreign key with identity '{id}' on table '{tableName}', since the principal column '{principalColumnName}' on the foreign key's principal table, '{principalTableName}', was not found in the model.
        /// </summary>
        public static EventDefinition<string?, string?, string?, string?> LogPrincipalColumnNotFound(IDiagnosticsLogger logger)
        {
            var definition = ((Diagnostics.Internal.NuoDbLoggingDefinitions)logger.Definitions).LogPrincipalColumnNotFound;
            if (definition == null)
            {
                definition = NonCapturingLazyInitializer.EnsureInitialized(
                    ref ((Diagnostics.Internal.NuoDbLoggingDefinitions)logger.Definitions).LogPrincipalColumnNotFound,
                    logger,
                    static logger => new EventDefinition<string?, string?, string?, string?>(
                        logger.Options,
                        NuoDbEventId.ForeignKeyPrincipalColumnMissingWarning,
                        LogLevel.Warning,
                        "NuoDbEventId.ForeignKeyPrincipalColumnMissingWarning",
                        level => LoggerMessage.Define<string?, string?, string?, string?>(
                            level,
                            NuoDbEventId.ForeignKeyPrincipalColumnMissingWarning,
                            _resourceManager.GetString("LogPrincipalColumnNotFound")!)));
            }

            return (EventDefinition<string?, string?, string?, string?>)definition;
        }

        /// <summary>
        ///     The entity type '{entityType}' is configured to use schema '{schema}', but NuoDb does not support schemas. This configuration will be ignored by the NuoDb provider.
        /// </summary>
        public static EventDefinition<string, string> LogSchemaConfigured(IDiagnosticsLogger logger)
        {
            var definition = ((Diagnostics.Internal.NuoDbLoggingDefinitions)logger.Definitions).LogSchemaConfigured;
            if (definition == null)
            {
                definition = NonCapturingLazyInitializer.EnsureInitialized(
                    ref ((Diagnostics.Internal.NuoDbLoggingDefinitions)logger.Definitions).LogSchemaConfigured,
                    logger,
                    static logger => new EventDefinition<string, string>(
                        logger.Options,
                        NuoDbEventId.SchemaConfigured,
                        LogLevel.Warning,
                        "NuoDbEventId.SchemaConfiguredWarning",
                        level => LoggerMessage.Define<string, string>(
                            level,
                            NuoDbEventId.SchemaConfigured,
                            _resourceManager.GetString("LogSchemaConfigured")!)));
            }

            return (EventDefinition<string, string>)definition;
        }

        /// <summary>
        ///     The model was configured with the database sequence '{sequence}'. NuoDb does not support sequences.
        /// </summary>
        public static EventDefinition<string> LogSequenceConfigured(IDiagnosticsLogger logger)
        {
            var definition = ((Diagnostics.Internal.NuoDbLoggingDefinitions)logger.Definitions).LogSequenceConfigured;
            if (definition == null)
            {
                definition = NonCapturingLazyInitializer.EnsureInitialized(
                    ref ((Diagnostics.Internal.NuoDbLoggingDefinitions)logger.Definitions).LogSequenceConfigured,
                    logger,
                    static logger => new EventDefinition<string>(
                        logger.Options,
                        NuoDbEventId.SequenceConfiguredWarning,
                        LogLevel.Warning,
                        "NuoDbEventId.SequenceConfiguredWarning",
                        level => LoggerMessage.Define<string>(
                            level,
                            NuoDbEventId.SequenceConfiguredWarning,
                            _resourceManager.GetString("LogSequenceConfigured")!)));
            }

            return (EventDefinition<string>)definition;
        }

        /// <summary>
        ///     An operation of type '{operationType}' will be attempted while a rebuild of table '{tableName}' is pending. The database may not be in an expected state. Review the SQL generated by this migration to help diagnose any failures. Consider moving these operations to a subsequent migration.
        /// </summary>
        public static EventDefinition<string, string> LogTableRebuildPendingWarning(IDiagnosticsLogger logger)
        {
            var definition = ((Diagnostics.Internal.NuoDbLoggingDefinitions)logger.Definitions).LogTableRebuildPendingWarning;
            if (definition == null)
            {
                definition = NonCapturingLazyInitializer.EnsureInitialized(
                    ref ((Diagnostics.Internal.NuoDbLoggingDefinitions)logger.Definitions).LogTableRebuildPendingWarning,
                    logger,
                    static logger => new EventDefinition<string, string>(
                        logger.Options,
                        NuoDbEventId.TableRebuildPendingWarning,
                        LogLevel.Warning,
                        "NuoDbEventId.TableRebuildPendingWarning",
                        level => LoggerMessage.Define<string, string>(
                            level,
                            NuoDbEventId.TableRebuildPendingWarning,
                            _resourceManager.GetString("LogTableRebuildPendingWarning")!)));
            }

            return (EventDefinition<string, string>)definition;
        }

        /// <summary>
        ///     A connection of an unexpected type ({type}) is being used. The SQL functions prefixed with 'ef_' could not be created automatically. Manually define them if you encounter errors while querying.
        /// </summary>
        public static EventDefinition<string> LogUnexpectedConnectionType(IDiagnosticsLogger logger)
        {
            var definition = ((Diagnostics.Internal.NuoDbLoggingDefinitions)logger.Definitions).LogUnexpectedConnectionType;
            if (definition == null)
            {
                definition = NonCapturingLazyInitializer.EnsureInitialized(
                    ref ((Diagnostics.Internal.NuoDbLoggingDefinitions)logger.Definitions).LogUnexpectedConnectionType,
                    logger,
                    static logger => new EventDefinition<string>(
                        logger.Options,
                        NuoDbEventId.UnexpectedConnectionTypeWarning,
                        LogLevel.Warning,
                        "NuoDbEventId.UnexpectedConnectionTypeWarning",
                        level => LoggerMessage.Define<string>(
                            level,
                            NuoDbEventId.UnexpectedConnectionTypeWarning,
                            _resourceManager.GetString("LogUnexpectedConnectionType")!)));
            }

            return (EventDefinition<string>)definition;
        }

        /// <summary>
        ///     NuoDb doesn't support schemas. The specified schema selection arguments will be ignored.
        /// </summary>
        public static EventDefinition LogUsingSchemaSelectionsWarning(IDiagnosticsLogger logger)
        {
            var definition = ((Diagnostics.Internal.NuoDbLoggingDefinitions)logger.Definitions).LogUsingSchemaSelectionsWarning;
            if (definition == null)
            {
                definition = NonCapturingLazyInitializer.EnsureInitialized(
                    ref ((Diagnostics.Internal.NuoDbLoggingDefinitions)logger.Definitions).LogUsingSchemaSelectionsWarning,
                    logger,
                    static logger => new EventDefinition(
                        logger.Options,
                        NuoDbEventId.SchemasNotSupportedWarning,
                        LogLevel.Warning,
                        "NuoDbEventId.SchemasNotSupportedWarning",
                        level => LoggerMessage.Define(
                            level,
                            NuoDbEventId.SchemasNotSupportedWarning,
                            _resourceManager.GetString("LogUsingSchemaSelectionsWarning")!)));
            }

            return (EventDefinition)definition;
        }
    }
}
