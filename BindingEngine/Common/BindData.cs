// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BindData.cs" company="Illusion">
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
//   Represents a target and source value for a binding.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Illusion.Utility
{
    /// <summary>
    ///     Represents a target and source value for a binding.
    /// </summary>
    public class BindData
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BindData"/> class.
        /// </summary>
        /// <param name="target">
        ///     The target.
        /// </param>
        /// <param name="source">
        ///     The source.
        /// </param>
        public BindData(BindContext target, BindContext source)
        {
            this.Target = target;
            this.Source = source;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the target.
        /// </summary>
        public BindContext Target { get; set; }

        /// <summary>
        ///     Gets or sets the source.
        /// </summary>
        public BindContext Source { get; set; }

        #endregion
    }
}