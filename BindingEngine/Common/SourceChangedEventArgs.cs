// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SourceChangedEventArgs.cs" company="Illusion">
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
//   Provides data for the source changed event raised.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Illusion.Utility
{
    using System;

    /// <summary>
    ///     Provides data for the source changed event raised.
    /// </summary>
    public class SourceChangedEventArgs : EventArgs
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SourceChangedEventArgs"/> class.
        /// </summary>
        /// <param name="sourceMode">
        ///     The source mode.
        /// </param>
        /// <param name="oldSource">
        ///     The old source.
        /// </param>
        /// <param name="newSource">
        ///     The new source.
        /// </param>
        public SourceChangedEventArgs(SourceMode sourceMode, object oldSource, object newSource)
        {
            this.SourceMode = sourceMode;
            this.OldSource = oldSource;
            this.NewSource = newSource;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the new source.
        /// </summary>
        public object NewSource { get; set; }

        /// <summary>
        ///     Gets or sets the old source.
        /// </summary>
        public object OldSource { get; set; }

        /// <summary>
        ///     Gets or sets the source mode.
        /// </summary>
        public SourceMode SourceMode { get; set; }

        #endregion
    }
}