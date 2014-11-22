// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WeakEvent.cs" company="Illusion">
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
//   The weak event.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Illusion.Utility
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;

    /// <summary>
    ///     Delegate WeakEventFilterHandler.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
    /// <returns><c>true</c> if registered, <c>false</c> otherwise.</returns>
    public delegate bool WeakEventFilterHandler(object sender, EventArgs args);

    /// <summary>
    ///     Represents a weak event, it holds the <see cref="WeakReference"/> for the specific instance.
    /// </summary>
    public class WeakEvent : WeakReference
    {
        #region Static Fields

        /// <summary>
        ///     The event handler info.
        /// </summary>
        private static readonly MethodInfo handlerInfo = typeof(WeakEvent).GetMethod(
            "OnAttachEventOccurred",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        #endregion

        #region Fields

        /// <summary>
        ///     The event action.
        /// </summary>
        private Delegate eventAction;

        /// <summary>
        ///     The event filter.
        /// </summary>
        private WeakEventFilterHandler eventFilter;

        /// <summary>
        ///     The event target.
        /// </summary>
        private WeakReference eventTarget;

        /// <summary>
        ///     The target type.
        /// </summary>
        private Type targetType;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="WeakEvent"/> class.
        /// </summary>
        /// <param name="target">
        ///     The target.
        /// </param>
        public WeakEvent(object target)
            : base(target)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the bind target.
        /// </summary>
        public object BindTarget
        {
            get
            {
                return this.eventTarget.Target;
            }
        }

        /// <summary>
        ///     Gets the event name.
        /// </summary>
        public string EventName { get; private set; }

        /// <summary>
        ///     Gets or sets the handler name.
        /// </summary>
        public string HandlerName { get; set; }

        /// <summary>
        ///     Gets a value indicating whether is attached.
        /// </summary>
        public bool IsAttached
        {
            get
            {
                return this.eventAction != null;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the data.
        /// </summary>
        internal BindSource Data { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Attaches the specific event.
        /// </summary>
        /// <param name="source">
        ///     The source.
        /// </param>
        /// <param name="sourceProperty">
        ///     The source property.
        /// </param>
        /// <param name="eventName">
        ///     The event name.
        /// </param>
        /// <param name="handlerName">
        ///     The handler name.
        /// </param>
        /// <param name="filter">
        ///     The filter.
        /// </param>
        public void AttachEvent(
            object source,
            string sourceProperty,
            string eventName,
            string handlerName,
            WeakEventFilterHandler filter = null)
        {
            this.AttachEvent(new BindSource(source, sourceProperty), eventName, handlerName, filter);
        }

        /// <summary>
        ///     Attaches the specific event.
        /// </summary>
        /// <param name="source">
        ///     The source.
        /// </param>
        /// <param name="eventName">
        ///     The event name.
        /// </param>
        /// <param name="handlerName">
        ///     The handler name.
        /// </param>
        /// <param name="filter">
        ///     The filter.
        /// </param>
        public void AttachEvent(
            BindSource source,
            string eventName,
            string handlerName,
            WeakEventFilterHandler filter = null)
        {
            this.DetachEvent();

            source.SourceChanged += this.OnSourceChanged;

            this.EventName = eventName;
            this.HandlerName = handlerName;

            this.Data = source;

            object value = this.Data.GeTTarget();
            if (value != null)
            {
                this.targetType = value.GetType();
                this.eventTarget = new WeakReference(value);
                this.eventAction = this.AttachEvent(eventName, handlerName, filter);
            }
        }

        /// <summary>
        ///     Attaches the specific event.
        /// </summary>
        /// <param name="type">
        ///     The type.
        /// </param>
        /// <param name="eventName">
        ///     The event name.
        /// </param>
        /// <param name="handlerName">
        ///     The handler name.
        /// </param>
        /// <param name="filter">
        ///     The filter.
        /// </param>
        public void AttachEvent(Type type, string eventName, string handlerName, WeakEventFilterHandler filter = null)
        {
            this.DetachEvent();

            this.EventName = eventName;
            this.Data = null;
            this.targetType = type;

            this.eventAction = this.AttachEvent(eventName, handlerName, filter);
        }

        /// <summary>
        ///     Detaches the specific event.
        /// </summary>
        public void DetachEvent()
        {
            if (this.eventAction != null)
            {
                EventInfo eventInfo = this.targetType.GetEvent(
                    this.EventName,
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                if (eventInfo != null && this.BindTarget != null)
                {
                    MethodInfo removeMethod = eventInfo.GetRemoveMethod(true);
                    removeMethod.Invoke(this.BindTarget, new object[] { this.eventAction });
                }

                this.eventTarget = null;
                this.eventAction = null;
            }

            if (this.Data != null)
            {
                this.Data.SourceChanged -= this.OnSourceChanged;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Handles the <see cref="E:AttachEventOccurred" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="EventArgs"/> instance containing the event data.</param>
        internal void OnAttachEventOccurred(object sender, EventArgs args)
        {
            if (!this.IsAlive || !this.eventTarget.IsNotNullAndAlive())
            {
                this.DetachEvent();
                return;
            }

            if (this.eventFilter != null && !this.eventFilter(sender, args))
            {
                return;
            }

            DynamicEngine.InvokeMethod(this.Target, this.HandlerName, new[] { sender, args });
        }

        /// <summary>
        ///     Attaches the specific event.
        /// </summary>
        /// <param name="eventName">
        ///     The event name.
        /// </param>
        /// <param name="handlerName">
        ///     The handler name.
        /// </param>
        /// <param name="filter">
        ///     The filter.
        /// </param>
        /// <returns>
        ///     The <see cref="Delegate"/>.
        /// </returns>
        private Delegate AttachEvent(string eventName, string handlerName, WeakEventFilterHandler filter = null)
        {
            if (this.targetType == null)
            {
                return null;
            }

            this.eventFilter = filter;

            this.HandlerName = handlerName;

            EventInfo eventInfo = this.targetType.GetEvent(
                eventName,
                BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (eventInfo == null)
            {
                return null; // No exception
            }

            ParameterExpression sender = Expression.Parameter(typeof(object), "sender");
            ParameterExpression args = Expression.Parameter(typeof(EventArgs), "e");

            Type eventHandlerType = eventInfo.EventHandlerType;
            MethodCallExpression body = Expression.Call(Expression.Constant(this), handlerInfo, sender, args);
            LambdaExpression lambda = Expression.Lambda(eventHandlerType, body, sender, args);

            Delegate action = lambda.Compile();

            MethodInfo addMethod = eventInfo.GetAddMethod(true);

            addMethod.Invoke(this.BindTarget, new object[] { action });

            return action;
        }

        /// <summary>
        ///     Handles the <see cref="E:SourceChanged" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="SourceChangedEventArgs"/> instance containing the event data.</param>
        private void OnSourceChanged(object sender, SourceChangedEventArgs e)
        {
            this.DetachEvent();
            this.AttachEvent(
                this.Data,
                this.EventName,
                this.HandlerName,
                this.eventFilter);
        }

        #endregion
    }
}