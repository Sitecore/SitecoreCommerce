// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetEntityCommandNoDeserialize.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.BizFx.DevOps
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;

    //using Core.Commands;

    using Microsoft.Extensions.Logging;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Core.Commands;
    using Sitecore.Commerce.Plugin.SQL;

    /// <summary>
    /// Gets a serialized entity from the store.
    /// </summary>
    /// <seealso cref="Sitecore.Commerce.Core.Commands.CommerceCommand" />
    [ExcludeFromCodeCoverage]
    public class GetEntityCommandNoDeserialize : SQLCommerceCommand
    {
        /// <summary>
        /// The entity serializer command.
        /// </summary>
        private readonly EntitySerializerCommand entitySerializerCommand;
        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetEntityCommandNoDeserialize" /> class.
        /// </summary>
        /// <param name="entitySerializerCommand">The entity serializer command.</param>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        /// windows update
        public GetEntityCommandNoDeserialize(EntitySerializerCommand entitySerializerCommand, 
            IServiceProvider serviceProvider, 
            CommerceCommander commerceCommander) : base(serviceProvider)
        {
            this.entitySerializerCommand = entitySerializerCommand;
            this._commerceCommander = commerceCommander;
        }

        /// <summary>
        /// Retrieves a Shop entity.
        /// </summary>
        /// <param name="commerceContext">The commerce context.</param>
        /// <param name="entityId">The entityId.</param>
        /// <returns>
        /// A <see cref="string" />
        /// </returns>
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
                                

                                //using (var stream = reader.GetStream("Entity"))
                                //{
                                //    //arg.EntityAsStream = stream;
                                //    //return await this.entitySerializerCommand.DeserializeEntityFromStream(arg, commerceContext);
                                //    var result = stream..ToString();

                                //}
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
