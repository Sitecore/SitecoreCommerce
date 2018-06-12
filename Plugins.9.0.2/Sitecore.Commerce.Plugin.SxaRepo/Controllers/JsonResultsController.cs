// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CartsController.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Commerce.Plugin.Carts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http.OData;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Sitecore.Commerce.Core;
    using Core.Commands;
    using Microsoft.ApplicationInsights;
    using System.Threading.Tasks;
    using Sitecore.Commerce.XA.Feature.Cart.Models.JsonResults;
    using Newtonsoft.Json;

    /// <summary>
    /// Defines the carts controller for the carts plugin.
    /// </summary>
    /// <seealso cref="Sitecore.Commerce.Core.CommerceController" />
    [Microsoft.AspNetCore.OData.EnableQuery]
    [Route("api/JsonResults")]
    public class JsonResultsController : CommerceController
    {
        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonResultsController"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="globalEnvironment">The global environment.</param>
        public JsonResultsController(IServiceProvider serviceProvider, CommerceEnvironment globalEnvironment, CommerceCommander commerceCommander) : base(serviceProvider, globalEnvironment)
        {
            this._commerceCommander = commerceCommander;
        }

        /// <summary>
        /// Gets a list of Carts.
        /// </summary>
        /// <returns>A <see cref="Cart"/></returns>
        [HttpPut]
        [Route("JsonResults/GetCartJson(Id={id})")]
        public async Task<CartJsonResult> GetCartJson([FromBody] ODataActionParameters value)
        {
            var id = value["Id"].ToString();

            //this.Logger.LogDebug($"GetCartJson: Id={id}");
            var cart = await this._commerceCommander
                .GetEntity<Cart>(this.CurrentContext, id, true);
            var cartJsonResult = new CartJsonResult { Id = id };
            cartJsonResult.Initialize(cart);

            return cartJsonResult;
        }


        /// <summary>
        /// Gets the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>A <see cref="Cart"/></returns>
        //[HttpGet]
        //[Route("(Id={id})")]
        //[Microsoft.AspNetCore.OData.EnableQuery]
        public async Task<IActionResult> Get(string id)
        {
            var modelPath = id;
            if (!this.ModelState.IsValid || string.IsNullOrEmpty(modelPath))
            {
                return this.NotFound();
            }

            this.CurrentContext.Logger.LogInformation($"JsonResultsController: ModelPath={modelPath}");
            var modelPaths = modelPath.Split(".".ToCharArray());

            id = modelPaths[0];

            var cart = await this._commerceCommander
                .GetEntity<Cart>(this.CurrentContext, id, true);
            var requestContext = this.CurrentContext.GetObject<RequestContext>();

            var cartJsonResult = new CartJsonResult { Id = id };
            cartJsonResult.Initialize(cart);

            if (cartJsonResult == null)
            {
                return this.NotFound();
            }

            var serializedResponse = JsonConvert.SerializeObject(cartJsonResult, new JsonSerializerSettings { Formatting = Formatting.Indented });
            this.CurrentContext.Logger.LogInformation($"JsonResultsController: JsonResult={serializedResponse}");


            //

            //return new ObjectResult(cartJsonResult);
            //return new JsonResult(cartJsonResult);
            return new JsonResult(cartJsonResult);
        }
    }
}