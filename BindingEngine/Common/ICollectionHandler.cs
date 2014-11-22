// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICollectionHandler.cs" company="Illusion">
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
//   Denotes an instance which can handle the collection CRD operation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Illusion.Utility
{
    /// <summary>
    ///     Denotes an instance which can handle the collection CRD operation.
    /// </summary>
    public interface ICollectionHandler
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Adds the item.
        /// </summary>
        /// <param name="index">
        ///     The index.
        /// </param>
        /// <param name="item">
        ///     The item.
        /// </param>
        /// <param name="target">
        ///     The target.
        /// </param>
        /// <param name="targetProperty">
        ///     The target property.
        /// </param>
        /// <returns>
        ///     Return value indicates whether the <see cref="ICollectionHandler"/> handle the change, if handled, the
        ///     default handle logic will ignore it.
        /// </returns>
        bool AddItem(int index, object item, object target, object targetProperty);

        /// <summary>
        ///     Removes the item.
        /// </summary>
        /// <param name="index">
        ///     The index.
        /// </param>
        /// <param name="item">
        ///     The item.
        /// </param>
        /// <param name="target">
        ///     The source.
        /// </param>
        /// <param name="targetProperty">
        ///     The source property.
        /// </param>
        /// <returns>
        ///     Return value indicates whether the <see cref="ICollectionHandler"/> handle the change, if handled, the
        ///     default handle logic will ignore it.
        /// </returns>
        bool RemoveItem(int index, object item, object target, object targetProperty);

        /// <summary>
        ///     Clears the specified target.
        /// </summary>
        /// <param name="target">
        ///     The target.
        /// </param>
        /// <param name="targetProperty">
        ///     The target property.
        /// </param>
        /// <returns>
        ///     Return value indicates whether the <see cref="ICollectionHandler"/> handle the change, if handled, the
        ///     default handle logic will ignore it.
        /// </returns>
        bool Clear(object target, object targetProperty);

        #endregion
    }
}