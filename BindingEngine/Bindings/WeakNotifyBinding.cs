// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WeakNotifyBinding.cs" company="Illusion">
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
//   The weak notify binding.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Illusion.Utility
{
    using System;

    /// <summary>
    ///     The concrete <see cref="WeakBinding"/> that provides the binding for notification.
    /// </summary>
    public class WeakNotifyBinding : WeakPropertyBinding
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="WeakNotifyBinding"/> class.
        /// </summary>
        /// <param name="target">
        ///     The target.
        /// </param>
        /// <param name="targetProperty">
        ///     The target property.
        /// </param>
        /// <param name="source">
        ///     The source.
        /// </param>
        /// <param name="sourceProperty">
        ///     The source property.
        /// </param>
        public WeakNotifyBinding(object target, string targetProperty, object source, string sourceProperty)
            : base(target, targetProperty, source, sourceProperty)
        {
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Initialize the <see cref="WeakBinding"/>.
        /// </summary>
        /// <typeparam name="T">
        ///     The concrete <see cref="WeakBinding"/> to return.
        /// </typeparam>
        /// <param name="isActivate">
        ///     The isActivate.
        /// </param>
        /// <returns>
        ///     The <see cref="T"/>.
        /// </returns>
        public override T Initialize<T>(bool isActivate = true)
        {
            this.SetMode(BindMode.TwoWay);
            return base.Initialize<T>(isActivate);
        }

        /// <summary>
        ///     Sets the bind mode, the <see cref="WeakNotifyBinding"/> are always set to <see cref="BindMode.TwoWay"/> binding.
        /// </summary>
        /// <param name="mode">
        ///     The mode.
        /// </param>
        /// <returns>
        ///     The <see cref="WeakBinding"/>.
        /// </returns>
        public override WeakBinding SetMode(BindMode mode)
        {
            // Always TwoWay binding.
            return mode != BindMode.TwoWay ? this : base.SetMode(mode);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Called when attached target <see cref="WeakEvent"/> occurred.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="args">
        ///     The <see cref="EventArgs"/> instance containing the event data.
        /// </param>
        /// <returns>
        ///     <c>true</c> if occurred, <c>false</c> otherwise.
        /// </returns>
        protected override bool OnTargetEventOccurred(object sender, EventArgs args)
        {
            if (!base.OnTargetEventOccurred(sender, args))
            {
                return false;
            }

            this.NotifyTargetChanged(null, null);

            return true;
        }

        /// <summary>
        ///     Called when attached source <see cref="WeakEvent"/> occurred.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="args">
        ///     The <see cref="EventArgs"/> instance containing the event data.
        /// </param>
        /// <returns>
        ///     <c>true</c> if XXXX, <c>false</c> otherwise.
        /// </returns>
        protected override bool OnSourceEventOccurred(object sender, EventArgs args)
        {
            if (!base.OnSourceEventOccurred(sender, args))
            {
                return false;
            }

            this.NotifySourceChanged(null, null);

            return true;
        }

        /// <summary>
        ///     Sources to target.
        /// </summary>
        protected override void SourceToTarget()
        {
            // Do nothing
        }

        /// <summary>
        ///     Targets to source.
        /// </summary>
        protected override void TargetToSource()
        {
            // Do nothing
        }

        #endregion
    }
}