// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WeakMethodBinding.cs" company="Illusion">
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
//  <summary>
//   The concrete <see cref="WeakBinding"/> that provides the binding for method.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Illusion.Utility
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///    The concrete <see cref="WeakBinding"/> that provides the binding for method.
    /// </summary>
    public class WeakMethodBinding : WeakBinding
    {
        #region Fields

        /// <summary>
        ///     The target can invoker.
        /// </summary>
        private MethodInvoker targetCanInvoker;

        /// <summary>
        ///     The source can invoker.
        /// </summary>
        private MethodInvoker sourceCanInvoker;

        /// <summary>
        ///     The target invoker.
        /// </summary>
        private MethodInvoker targetInvoker;

        /// <summary>
        ///     The source invoker.
        /// </summary>
        private MethodInvoker sourceInvoker;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="WeakMethodBinding"/> class.
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
        public WeakMethodBinding(object target, string targetProperty, object source, string sourceProperty)
            : base(target, targetProperty, source, sourceProperty)
        {
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Attaches the target can invoke method.
        /// </summary>
        /// <param name="canInvokeMethod">
        ///     The can invoke method.
        /// </param>
        /// <param name="parameters">
        ///     The parameters.
        /// </param>
        /// <returns>
        ///     The <see cref="WeakMethodBinding"/>.
        /// </returns>
        public WeakMethodBinding AttachTargetCanInvokeMethod(string canInvokeMethod, IEnumerable<BindContext> parameters)
        {
            return this.AttachTargetCanInvokeMethod(this.BindTarget, canInvokeMethod, parameters);
        }

        /// <summary>
        ///     Attaches the target can invoke method.
        /// </summary>
        /// <param name="context">
        ///     The source.
        /// </param>
        /// <param name="canInvokeMethod">
        ///     The can invoke method.
        /// </param>
        /// <param name="parameters">
        ///     The parameters.
        /// </param>
        /// <returns>
        ///     The <see cref="WeakMethodBinding"/>.
        /// </returns>
        public WeakMethodBinding AttachTargetCanInvokeMethod(
            BindContext context,
            string canInvokeMethod,
            IEnumerable<BindContext> parameters)
        {
            this.targetCanInvoker = new MethodInvoker(context, canInvokeMethod, parameters);
            return this;
        }

        /// <summary>
        ///     Attaches the target method.
        /// </summary>
        /// <param name="targetMethod">
        ///     The target method.
        /// </param>
        /// <param name="parameters">
        ///     The parameters.
        /// </param>
        /// <returns>
        ///     The <see cref="WeakMethodBinding"/>.
        /// </returns>
        public WeakMethodBinding AttachTargetMethod(string targetMethod, IEnumerable<BindContext> parameters)
        {
            return this.AttachTargetMethod(this.BindTarget, targetMethod, parameters);
        }

        /// <summary>
        ///     Attaches the target method.
        /// </summary>
        /// <param name="context">
        ///     The source.
        /// </param>
        /// <param name="targetMethod">
        ///     The target method.
        /// </param>
        /// <param name="parameters">
        ///     The parameters.
        /// </param>
        /// <returns>
        ///     The <see cref="WeakMethodBinding"/>.
        /// </returns>
        public WeakMethodBinding AttachTargetMethod(
            BindContext context,
            string targetMethod,
            IEnumerable<BindContext> parameters)
        {
            this.targetInvoker = new MethodInvoker(context, targetMethod, parameters);
            return this;
        }

        /// <summary>
        ///     Attaches the source can invoke method.
        /// </summary>
        /// <param name="canInvokeMethod">
        ///     The can invoke method.
        /// </param>
        /// <param name="parameters">
        ///     The parameters.
        /// </param>
        /// <returns>
        ///     The <see cref="WeakMethodBinding"/>.
        /// </returns>
        public WeakMethodBinding AttachSourceCanInvokeMethod(string canInvokeMethod, IEnumerable<BindContext> parameters)
        {
            return this.AttachSourceCanInvokeMethod(this.BindSource, canInvokeMethod, parameters);
        }

        /// <summary>
        ///     Attaches the source can invoke method.
        /// </summary>
        /// <param name="context">
        ///     The source.
        /// </param>
        /// <param name="canInvokeMethod">
        ///     The can invoke method.
        /// </param>
        /// <param name="parameters">
        ///     The parameters.
        /// </param>
        /// <returns>
        ///     The <see cref="WeakMethodBinding"/>.
        /// </returns>
        public WeakMethodBinding AttachSourceCanInvokeMethod(
            BindContext context, 
            string canInvokeMethod, 
            IEnumerable<BindContext> parameters)
        {
            this.sourceCanInvoker = new MethodInvoker(context, canInvokeMethod, parameters);
            return this;
        }

        /// <summary>
        ///     Attaches the source method.
        /// </summary>
        /// <param name="sourceMethod">
        ///     The source method.
        /// </param>
        /// <param name="parameters">
        ///     The parameters.
        /// </param>
        /// <returns>
        ///     The <see cref="WeakMethodBinding"/>.
        /// </returns>
        public WeakMethodBinding AttachSourceMethod(string sourceMethod, IEnumerable<BindContext> parameters)
        {
            return this.AttachSourceMethod(this.BindSource, sourceMethod, parameters);
        }

        /// <summary>
        ///     Attaches the source method.
        /// </summary>
        /// <param name="context">
        ///     The source.
        /// </param>
        /// <param name="sourceMethod">
        ///     The source method.
        /// </param>
        /// <param name="parameters">
        ///     The parameters.
        /// </param>
        /// <returns>
        ///     The <see cref="WeakMethodBinding"/>.
        /// </returns>
        public WeakMethodBinding AttachSourceMethod(
            BindContext context, 
            string sourceMethod, 
            IEnumerable<BindContext> parameters)
        {
            this.sourceInvoker = new MethodInvoker(context, sourceMethod, parameters);
            return this;
        }

        /// <summary>
        ///     Detaches the target can invoke method.
        /// </summary>
        /// <returns>The <see cref="WeakMethodBinding" />.</returns>
        public WeakMethodBinding DetachTargetCanInvokeMethod()
        {
            this.targetCanInvoker = null;
            return this;
        }

        /// <summary>
        ///     Detaches the target method.
        /// </summary>
        /// <returns>The <see cref="WeakMethodBinding" />.</returns>
        public WeakMethodBinding DetachTargetMethod()
        {
            this.targetInvoker = null;
            return this;
        }

        /// <summary>
        ///     Detaches the source can invoke method.
        /// </summary>
        /// <returns>The <see cref="WeakMethodBinding" />.</returns>
        public WeakMethodBinding DetachSourceCanInvokeMethod()
        {
            this.sourceCanInvoker = null;
            return this;
        }

        /// <summary>
        ///     Detaches the source method.
        /// </summary>
        /// <returns>The <see cref="WeakMethodBinding" />.</returns>
        public WeakMethodBinding DetachSourceMethod()
        {
            this.sourceInvoker = null;
            return this;
        }

        /// <summary>
        ///     Initialize the instance.
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
            // Set default SourceMode to Property
            this.SetTargetBindMode(SourceMode.Property);
            this.SetSourceBindMode(SourceMode.Property);

            return base.Initialize<T>(isActivate);
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

            this.InvokeSourceMethod();

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
        ///     <c>true</c> if occurred, <c>false</c> otherwise.
        /// </returns>
        protected override bool OnSourceEventOccurred(object sender, EventArgs args)
        {
            if (!base.OnSourceEventOccurred(sender, args))
            {
                return false;
            }

            this.InvokeTargetMethod();

            return true;
        }

        /// <summary>
        ///     Updates the target value.
        /// </summary>
        /// <param name="args">
        ///     The <see cref="EventArgs"/> instance containing the event data.
        /// </param>
        protected override void UpdateTargetValue(EventArgs args)
        {
            base.UpdateTargetValue(args);

            this.InvokeTargetMethod();
        }

        /// <summary>
        ///     Updates the source value.
        /// </summary>
        /// <param name="args">
        ///     The <see cref="EventArgs"/> instance containing the event data.
        /// </param>
        protected override void UpdateSourceValue(EventArgs args)
        {
            base.UpdateSourceValue(args);

            this.InvokeSourceMethod();
        }

        /// <summary>
        ///     Invokes the target method.
        /// </summary>
        private void InvokeTargetMethod()
        {
            if (this.targetInvoker != null)
            {
                if (this.targetCanInvoker != null && !this.targetCanInvoker.CanInvoke())
                {
                    return;
                }

                this.targetInvoker.Invoke();
            }
        }

        /// <summary>
        ///     Invokes the source method.
        /// </summary>
        private void InvokeSourceMethod()
        {
            if (this.sourceInvoker != null)
            {
                if (this.sourceCanInvoker != null && !this.sourceCanInvoker.CanInvoke())
                {
                    return;
                }

                this.sourceInvoker.Invoke();
            }
        }

        #endregion
    }
}