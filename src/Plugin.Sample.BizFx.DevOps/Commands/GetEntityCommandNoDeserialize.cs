namespace Plugin.Sample.BizFx.DevOps.Commands
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Core.Commands;
    using Sitecore.Commerce.Plugin.SQL;
   
    public class GetEntityCommandNoDeserialize : SQLCommerceCommand
    {
        public GetEntityCommandNoDeserialize( 
            IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public virtual async Task<string> Process(CommerceContext commerceContext, string entityId)
        {
            using (CommandActivity.Start(commerceContext, this))
            {
                var sqlContext = this.GetReadOnlySQLContext(commerceContext);

                try
                {
                    commerceContext.Logger.LogTrace($"DevOps.GetEntityCommandNoDeserialize.Converting: EntityId={entityId}|Environment={commerceContext.Environment.Name}");

                    using (var connection = new SqlConnection(sqlContext.ConnectionString))
                    {
                        await connection.OpenAsync();
                        var command = connection.CreateCommand();
                        command.CommandText = "sp_CommerceEntitiesSelect";
                        command.CommandType = CommandType.StoredProcedure;

                        var entityIdParameter = command.CreateParameter();
                        entityIdParameter.ParameterName = "@Id";
                        entityIdParameter.DbType = DbType.String;
                        entityIdParameter.Value = entityId;
                        command.Parameters.Add(entityIdParameter);

                        var environmentIdParameter = command.CreateParameter();
                        environmentIdParameter.ParameterName = "@EnvironmentId";
                        environmentIdParameter.DbType = DbType.Guid;
                        environmentIdParameter.Value = commerceContext.Environment.ArtifactStoreId;
                        command.Parameters.Add(environmentIdParameter);

                        using (var reader = await command.ExecuteReaderAsync(CommandBehavior.SequentialAccess))
                        {
                            if (await reader.ReadAsync())
                            {
                                try
                                {
                                    var result = reader.GetString(0);
                                    
                                    return result;
                                }
                                catch(Exception ex)
                                {
                                    commerceContext.LogException("DevOps.GetEntityCommandAsString", ex);
                                }
                            }
                        }

                        return null;
                    }
                }
                catch (SqlException ex)
                {
                    string message = $"SQL.GetEntityCommand.Exception: Id={entityId}|Environment={commerceContext.Environment.Id}|Message='{ex.Message}'|Number={ex.Number}|Procedure='{ex.Procedure}'|Line={ex.LineNumber}";
                    throw this.EvaluateSqlException(ex, message, commerceContext);
                }
            }
        }
    }
}
