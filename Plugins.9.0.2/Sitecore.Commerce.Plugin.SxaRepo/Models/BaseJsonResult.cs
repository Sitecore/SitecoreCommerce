//-----------------------------------------------------------------------
// <copyright file="BaseJsonResult.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// <summary>Defines the BaseJsonResult class.</summary>
//-----------------------------------------------------------------------
// Copyright 2016 Sitecore Corporation A/S
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file 
// except in compliance with the License. You may obtain a copy of the License at
//       http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed under the 
// License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, 
// either express or implied. See the License for the specific language governing permissions 
// and limitations under the License.
// -------------------------------------------------------------------------------------------

namespace Sitecore.Commerce.XA.Foundation.Common.Models.JsonResults
{
    using Microsoft.AspNetCore.Mvc;
    //using Services;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    //using System.Web.Mvc;
    using System.Web.Script.Serialization;

    /// <summary>
    /// Defines the BaseJsonResult class.
    /// </summary>
    public class BaseJsonResult // : JsonResult
    {
        private readonly List<string> _errors = new List<string>();
        private readonly List<string> _info = new List<string>();
        private readonly List<string> _warnings = new List<string>();

        #region "Constructors"

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseJsonResult"/> class.
        /// </summary>
        /// <param name="storefrontContext">The storefront context.</param>
        //public BaseJsonResult( [NotNull] IStorefrontContext storefrontContext)
        //{
        //    this.Success = true;
        //    this.StorefrontContext = storefrontContext;
        //}

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseJsonResult"/> class.
        /// </summary>
        /// <param name="result">The service provider result.</param>
        /// <param name="storefrontContext">The storefront context.</param>
        //public BaseJsonResult()
        //{
        //    this.Success = true;
        //    this.StorefrontContext = storefrontContext;

        //    if (result != null)
        //    {
        //        this.SetErrors(result);
        //    }
        //}

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseJsonResult" /> class.
        /// </summary>
        /// <param name="area">The area.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="storefrontContext">The storefront context.</param>
        //public BaseJsonResult(string area, Exception exception, [NotNull] IStorefrontContext storefrontContext)
        //{
        //    this.Success = false;
        //    this.StorefrontContext = storefrontContext;

        //    this.SetErrors(area, exception);
        //}

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseJsonResult"/> class.
        /// </summary>
        /// <param name="url">The redirect URL.</param>
        /// <param name="storefrontContext">The storefront context.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "0#", Justification = "I want to use them as strings.")]
        //public BaseJsonResult(string url, [NotNull] IStorefrontContext storefrontContext)
        //{
        //    this.Success = false;
        //    this.StorefrontContext = storefrontContext;

        //    this.Url = url;
        //}
        #endregion

        /// <summary>
        /// Gets or sets the storefront context.
        /// </summary>
        /// <value>
        /// The storefront context.
        /// </value>
        //[ScriptIgnore]
        //public IStorefrontContext StorefrontContext { get; set; }

        /// <summary>
        /// Gets the errors.
        /// </summary>
        /// <value>
        /// The errors.
        /// </value>
        public List<string> Errors
        {
            get { return this._errors; }
        }

        /// <summary>
        /// Gets the infos.
        /// </summary>
        /// <value>
        /// The infos.
        /// </value>
        public List<string> Info
        {
            get { return this._info; }
        }

        /// <summary>
        /// Gets the warnings.
        /// </summary>
        /// <value>
        /// The warnings.
        /// </value>
        public List<string> Warnings
        {
            get { return this._warnings; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has errors.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has errors; otherwise, <c>false</c>.
        /// </value>
        public bool HasErrors
        {
            get { return this._errors != null && this._errors.Any(); }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has infos.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has infos; otherwise, <c>false</c>.
        /// </value>
        public bool HasInfo
        {
            get { return this._info != null && this._info.Any(); }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has warnings.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has warnings; otherwise, <c>false</c>.
        /// </value>
        public bool HasWarnings
        {
            get { return this._warnings != null && this._warnings.Any(); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="BaseJsonResult"/> is success.
        /// </summary>
        /// <value>
        ///   <c>true</c> if success; otherwise, <c>false</c>.
        /// </value>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the redirect URL.
        /// </summary>
        /// <value>
        /// The redirect URL.
        /// </value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "I want to use them as strings.")]
        public string Url { get; set; }

        /// <summary>
        /// Sets the error.
        /// </summary>
        /// <param name="error">The error.</param>
        public void SetError(string error)
        {
            if (!string.IsNullOrEmpty(error))
            {
                this.Success = false;
                this.Errors.Add(error);
            }
        }

        /// <summary>
        /// Sets the errors.
        /// </summary>
        /// <param name="result">The result.</param>
        //public void SetErrors(ServiceProviderResult result)
        //{
        //    this.Success = result.Success;
        //    if (result.SystemMessages.Count <= 0)
        //    {
        //        return;
        //    }

        //    var errors = result.SystemMessages;
        //    foreach (var error in errors)
        //    {
        //        var message = this.StorefrontContext.GetSystemMessage(error.Message, false);
        //        this.Errors.Add(string.IsNullOrEmpty(message) ? error.Message : message);
        //    }
        //}

        /// <summary>
        /// Sets the errors.
        /// </summary>
        /// <param name="area">The area.</param>
        /// <param name="exception">The exception.</param>
        public void SetErrors(string area, Exception exception)
        {
            this._errors.Add(string.Format(CultureInfo.InvariantCulture, "{0}: {1}", area, exception.Message));
            this.Success = false;
        }

        /// <summary>
        /// Sets the errors.
        /// </summary>
        /// <param name="errors">The errors.</param>
        public void SetErrors(List<string> errors)
        {
            if (!errors.Any())
            {
                return;
            }

            this.Success = false;
            this._errors.AddRange(errors);
        }

        /// <summary>
        /// Sets the info.
        /// </summary>
        /// <param name="info">The info.</param>
        public void SetInfo(string info)
        {
            if (!string.IsNullOrEmpty(info))
            {
                this._info.AddRange(new List<string> { info });
            }
        }

        /// <summary>
        /// Sets the infos.
        /// </summary>
        /// <param name="info">The infos.</param>
        public void SetInfo(List<string> info)
        {
            if (!info.Any())
            {
                return;
            }

            this._info.AddRange(info);
        }

        /// <summary>
        /// Sets the errors.
        /// </summary>
        /// <param name="warnings">The errors.</param>
        public void SetWarnings(List<string> warnings)
        {
            if (!warnings.Any())
            {
                return;
            }

            this._warnings.AddRange(warnings);
        }
    }
}