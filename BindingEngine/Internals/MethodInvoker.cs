// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MethodInvoker.cs" company="Illusion">
//   The MIT License (MIT)
//      
//   Copyright (c) 2014 yohan zhou 
//      
//   Permission is hereby granted, free of charge, to any person obtaining a
//   copy of this software and associated documentation files (the
//   "Software"), to deal in the Software without restriction, including
//   without limitation the rights to use, copy, modify, merge, publish,
//   distribute, sublicense, and/or sell copies of the Software, and to
//   permit persons to whom the Software is furnished to do so, subject to
//   the following conditions:
//      
//   The above copyright notice and this permission notice shall be included
//   in all copies or substantial portions of the Software.
//      
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
//   OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
//   MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
//   IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
//   CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
//   TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
//   SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// </copyright>
// <summary>
//   Represents the operation of method invoke.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Illusion.Utility
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    ///     Represents the operation of method invoke.
    /// </summary>
    internal class MethodInvoker
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MethodInvoker"/> class.
        /// </summary>
        /// <param name="context">
        ///     The source.
        /// </param>
        /// <param name="method">
        ///     The method.
        /// </param>
        /// <param name="parameters">
        ///     The parameters.
        /// </param>
        public MethodInvoker(BindContext context, string method, IEnumerable<BindContext> parameters)
        {
            this.Context = context;
            this.Method = method;
            this.Parameters = parameters;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the method.
        /// </summary>
        /// <value>The method.</value>
        public string Method { get; set; }

        /// <summary>
        ///     Gets or sets the parameters.
        /// </summary>
        /// <value>The parameters.</value>
        public IEnumerable<BindContext> Parameters { get; set; }

        /// <summary>
        ///     Gets or sets the source.
        /// </summary>
        /// <value>The source.</value>
        public BindContext Context { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Determines whether this instance can invoke.
        /// </summary>
        /// <returns><c>true</c> if this instance can invoke; otherwise, <c>false</c>.</returns>
        /// <exception cref="MissingMemberException">The attach method does not return boolean value.</exception>
        public bool CanInvoke()
        {
            object source = this.Context.Value;
            if (source != null)
            {
                object[] paras = this.Parameters != null ? this.Parameters.Select(item => item.Value).ToArray() : null;
                var methodInfo = DynamicEngine.GetMethodInfo(source.GetType(), this.Method, paras, false);
                if (methodInfo != null)
                {
                    object value = DynamicEngine.InvokeMethod(source, methodInfo, paras);
                    if (!(value is bool))
                    {
                        throw new MissingMemberException(
                            string.Format("The attach method {0} does not return Boolean value", this.Method));
                    }

                    return (bool)value;
                }
            }

            return false;
        }

        /// <summary>
        ///     Invokes this instance.
        /// </summary>
        public void Invoke()
        {
            object source = this.Context.Value;
            if (source != null)
            {
                object[] paras = this.Parameters != null ? this.Parameters.Select(item => item.Value).ToArray() : null;
                var methodInfo = DynamicEngine.GetMethodInfo(source.GetType(), this.Method, paras, false);
                if (methodInfo != null)
                {
                    DynamicEngine.InvokeMethod(source, methodInfo, paras);
                }
            }
        }

        #endregion
    }
}