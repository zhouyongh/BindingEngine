// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WeakBinding.cs" company="Illusion">
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
//   The base binding class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Illusion.Utility
{
    using System;

    /// <summary>
    ///     The base binding class.
    /// </summary>
    public abstract class WeakBinding : WeakReference
    {
        #region Static Fields

        /// <summary>
        ///     Used as a default value to instruct the binding not to perform any action.
        /// </summary>
        public static readonly object DoNothing = new object();

        /// <summary>
        ///     The name of target event handler.
        /// </summary>
        public static readonly string TargetEventHandlerName = "OnTargetEventOccurred";

        /// <summary>
        ///     The name of source event handler.
        /// </summary>
        public static readonly string SourceEventHandlerName = "OnSourceEventOccurred";

        #endregion

        #region Fields

        /// <summary>
        ///     The bind mode.
        /// </summary>
        protected BindMode bindMode = Utility.BindMode.OneWay;

        /// <summary>
        ///     The flag indicates whether the <see cref="WeakBinding"/> is activate.
        /// </summary>
        private bool activate;

        /// <summary>
        ///     The <see cref="WeakReference"/> holds the target changed.
        /// </summary>
        private WeakReference weakTargetChanged;

        /// <summary>
        ///     The <see cref="WeakReference"/> holds the source changed.
        /// </summary>
        private WeakReference weakSourceChanged;

        /// <summary>
        ///     The target changed.
        /// </summary>
        private BindingValueChangedHandler targetChanged;

        /// <summary>
        ///     The source changed.
        /// </summary>
        private BindingValueChangedHandler sourceChanged;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="WeakBinding"/> class.
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
        protected WeakBinding(object target, string targetProperty, object source, string sourceProperty)
            : base(target)
        {
            this.TargetProperty = targetProperty;
            this.SourceProperty = sourceProperty;

            this.TargetEvent = new WeakEvent(this);
            this.SourceEvent = new WeakEvent(this);

            this.BindTarget = new BindSource(target, targetProperty);
            this.BindTarget.SourceChanged += this.OnBindTargetChanged;
            this.BindSource = new BindSource(source, sourceProperty);
            this.BindSource.SourceChanged += this.OnBindSourceChanged;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the target property.
        /// </summary>
        /// <value>The target property.</value>
        public string TargetProperty { get; set; }

        /// <summary>
        ///     Gets or sets the source property.
        /// </summary>
        /// <value>The source property.</value>
        public string SourceProperty { get; set; }

        /// <summary>
        ///     Gets a value indicating whether the binding is activate.
        /// </summary>
        /// <value>The activate property.</value>
        public bool IsActivate
        {
            get
            {
                return this.activate;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the bind target.
        /// </summary>
        /// <value>The bind target.</value>
        internal BindSource BindTarget { get; private set; }

        /// <summary>
        ///     Gets the bind source.
        /// </summary>
        /// <value>The bind source.</value>
        internal BindSource BindSource { get; private set; }

        /// <summary>
        ///     Gets or sets the target event.
        /// </summary>
        /// <value>The target event.</value>
        internal WeakEvent TargetEvent { get; set; }

        /// <summary>
        ///     Gets or sets the source event.
        /// </summary>
        /// <value>The source event.</value>
        internal WeakEvent SourceEvent { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Activate the instance.
        /// </summary>
        /// <param name="refresh">
        ///     The refresh.
        /// </param>
        public void Activate(bool refresh = true)
        {
            this.activate = true;
            if (refresh)
            {
                this.Refresh();
            }
        }

        /// <summary>
        ///     Attach event to bind target.
        /// </summary>
        /// <param name="target">
        ///     The target.
        /// </param>
        /// <param name="eventName">
        ///     The event name.
        /// </param>
        /// <param name="filter">
        ///     The filter.
        /// </param>
        /// <returns>
        ///     The <see cref="WeakBinding"/>.
        /// </returns>
        public WeakBinding AttachTargetEvent(object target, string eventName, WeakEventFilterHandler filter = null)
        {
            BindSource bindSource = this.GetBindSource(target, null);
            return this.AttachTargetEvent(bindSource, eventName, filter);
        }

        /// <summary>
        ///     Attach event to bind target.
        /// </summary>
        /// <param name="target">
        ///     The target.
        /// </param>
        /// <param name="property">
        ///     The property.
        /// </param>
        /// <param name="eventName">
        ///     The event name.
        /// </param>
        /// <param name="filter">
        ///     The filter.
        /// </param>
        /// <returns>
        ///     The <see cref="WeakBinding"/>.
        /// </returns>
        public WeakBinding AttachTargetEvent(
            object target,
            string property,
            string eventName,
            WeakEventFilterHandler filter = null)
        {
            BindSource bindSource = this.GetBindSource(target, property);
            return this.AttachTargetEvent(bindSource, eventName, filter);
        }

        /// <summary>
        ///     Attach event to bind target.
        /// </summary>
        /// <param name="eventName">
        ///     The event name.
        /// </param>
        /// <param name="filter">
        ///     The filter.
        /// </param>
        /// <returns>
        ///     The <see cref="WeakBinding"/>.
        /// </returns>
        public WeakBinding AttachTargetEvent(string eventName, WeakEventFilterHandler filter = null)
        {
            return this.AttachTargetEvent(this.BindTarget, eventName, filter);
        }

        /// <summary>
        ///     Attach event to bind target.
        /// </summary>
        /// <param name="type">
        ///     The type.
        /// </param>
        /// <param name="eventName">
        ///     The event name.
        /// </param>
        /// <param name="filter">
        ///     The filter.
        /// </param>
        /// <returns>
        ///     The <see cref="WeakBinding"/>.
        /// </returns>
        public WeakBinding AttachTargetEvent(Type type, string eventName, WeakEventFilterHandler filter = null)
        {
            this.TargetEvent.AttachEvent(type, eventName, TargetEventHandlerName, filter);
            return this;
        }

        /// <summary>
        ///     Attach event to bind source.
        /// </summary>
        /// <param name="target">
        ///     The target.
        /// </param>
        /// <param name="eventName">
        ///     The event name.
        /// </param>
        /// <param name="filter">
        ///     The filter.
        /// </param>
        /// <returns>
        ///     The <see cref="WeakBinding"/>.
        /// </returns>
        public WeakBinding AttachSourceEvent(object target, string eventName, WeakEventFilterHandler filter = null)
        {
            BindSource bindSource = this.GetBindSource(target, null);
            return this.AttachSourceEvent(bindSource, eventName, filter);
        }

        /// <summary>
        ///     Attach event to bind source.
        /// </summary>
        /// <param name="target">
        ///     The target.
        /// </param>
        /// <param name="property">
        ///     The property.
        /// </param>
        /// <param name="eventName">
        ///     The event name.
        /// </param>
        /// <param name="filter">
        ///     The filter.
        /// </param>
        /// <returns>
        ///     The <see cref="WeakBinding"/>.
        /// </returns>
        public WeakBinding AttachSourceEvent(
            object target,
            string property,
            string eventName,
            WeakEventFilterHandler filter = null)
        {
            BindSource bindSource = this.GetBindSource(target, property);
            return this.AttachSourceEvent(bindSource, eventName, filter);
        }

        /// <summary>
        ///     Attach event to bind source.
        /// </summary>
        /// <param name="eventName">
        ///     The event name.
        /// </param>
        /// <param name="filter">
        ///     The filter.
        /// </param>
        /// <returns>
        ///     The <see cref="WeakBinding"/>.
        /// </returns>
        public WeakBinding AttachSourceEvent(string eventName, WeakEventFilterHandler filter = null)
        {
            return this.AttachSourceEvent(this.BindSource, eventName, filter);
        }

        /// <summary>
        ///     Attach event to bind source.
        /// </summary>
        /// <param name="type">
        ///     The type.
        /// </param>
        /// <param name="eventName">
        ///     The event name.
        /// </param>
        /// <param name="filter">
        ///     The filter.
        /// </param>
        /// <returns>
        ///     The <see cref="WeakBinding"/>.
        /// </returns>
        public WeakBinding AttachSourceEvent(Type type, string eventName, WeakEventFilterHandler filter = null)
        {
            this.SourceEvent.AttachEvent(type, eventName, SourceEventHandlerName, filter);
            return this;
        }

        /// <summary>
        ///     Clear the instance.
        /// </summary>
        public virtual void Clear()
        {
            this.BindTarget.Clear();
            this.BindSource.Clear();

            this.sourceChanged = null;
            this.targetChanged = null;

            this.DetachTargetEvent();
            this.DetachSourceEvent();
        }

        /// <summary>
        ///     DeActivate the <see cref="WeakBinding"/>.
        /// </summary>
        public void DeActivate()
        {
            this.activate = false;
        }

        /// <summary>
        ///     Detach the event from bind target.
        /// </summary>
        /// <returns>The <see cref="WeakBinding" />.</returns>
        public WeakBinding DetachTargetEvent()
        {
            this.TargetEvent.DetachEvent();
            return this;
        }

        /// <summary>
        ///     Detach the event from bind source.
        /// </summary>
        /// <returns>The <see cref="WeakBinding" />.</returns>
        public WeakBinding DetachSourceEvent()
        {
            this.SourceEvent.DetachEvent();
            return this;
        }

        /// <summary>
        ///      Initialize the instance.
        /// </summary>
        /// <typeparam name="T">
        ///     The concrete <see cref="WeakBinding"/> to return.
        /// </typeparam>
        /// <param name="isActivate">
        ///     The activate flag.
        /// </param>
        /// <returns>
        ///     The <see cref="T"/>.
        /// </returns>
        public virtual T Initialize<T>(bool isActivate = true) where T : WeakBinding
        {
            this.DoConventions();

            if (isActivate)
            {
                this.Activate();
            }

            return this as T;
        }

        /// <summary>
        ///     Return the specific <see cref="WeakBinding"/>.
        /// </summary>
        /// <typeparam name="T">The concrete <see cref="WeakBinding" /> to return.</typeparam>
        /// <returns>The <see cref="T" />.</returns>
        public T OfType<T>() where T : WeakBinding
        {
            return this as T;
        }

        /// <summary>
        ///     Refresh the instance.
        /// </summary>
        public void Refresh()
        {
            if (this.CheckValidation())
            {
                this.BindTarget.NotifyValueChanged();
                this.BindSource.NotifyValueChanged();
            }
        }

        /// <summary>
        ///     Set the <see cref="SourceMode"/> to bind target.
        /// </summary>
        /// <param name="sourceMode">
        ///     The source mode.
        /// </param>
        /// <returns>
        ///     The <see cref="WeakBinding"/>.
        /// </returns>
        public WeakBinding SetTargetBindMode(SourceMode sourceMode)
        {
            this.BindTarget.SourceMode = sourceMode;
            return this;
        }

        /// <summary>
        ///     Sets the source source mode.
        /// </summary>
        /// <param name="sourceMode">
        ///     The source mode.
        /// </param>
        /// <returns>
        ///     The <see cref="WeakBinding"/>.
        /// </returns>
        public WeakBinding SetSourceBindMode(SourceMode sourceMode)
        {
            this.BindSource.SourceMode = sourceMode;
            return this;
        }

        /// <summary>
        ///     Sets the target changed.
        /// </summary>
        /// <typeparam name="T">
        ///     The concrete <see cref="WeakBinding"/> to return.
        /// </typeparam>
        /// <param name="targetChangedHandler">
        ///     The source changed.
        /// </param>
        /// <param name="isWeakReference">
        ///     Indicates whether the <see cref="WeakNotifyBinding"/> should hold the targetChangedHandler in <see cref="WeakReference"/>, in this case, the callee should hold the strong reference of targetChangedHandler to prevent it collected by GC.
        /// </param>
        /// <returns>
        ///     The <see cref="T"/>.
        /// </returns>
        public T SetTargetChanged<T>(BindingValueChangedHandler targetChangedHandler, bool isWeakReference = false) where T : WeakBinding
        {
            return this.SetTargetChanged(targetChangedHandler) as T;
        }

        /// <summary>
        ///     Sets the target changed.
        /// </summary>
        /// <param name="targetChangedHandler">
        ///     The target changed handler.
        /// </param>
        /// <param name="isWeakReference">
        ///     Indicates whether the <see cref="WeakNotifyBinding"/> should hold the targetChangedHandler in <see cref="WeakReference"/>, in this case, the callee should hold the strong reference of targetChangedHandler to prevent it collected by GC.
        /// </param>
        /// <returns>
        ///     The <see cref="WeakBinding"/>.
        /// </returns>
        public WeakBinding SetTargetChanged(BindingValueChangedHandler targetChangedHandler, bool isWeakReference = false)
        {
            if (isWeakReference)
            {
                this.weakTargetChanged = new WeakReference(targetChangedHandler);
            }
            else
            {
                this.targetChanged = targetChangedHandler;
            }

            return this;
        }

        /// <summary>
        ///     Sets the source changed.
        /// </summary>
        /// <typeparam name="T">
        ///     The concrete <see cref="WeakBinding"/> to return.
        /// </typeparam>
        /// <param name="sourceChangedHandler">
        ///     The source changed.
        /// </param>
        /// <param name="isWeakReference">
        ///     Indicates whether the <see cref="WeakNotifyBinding"/> should hold the sourceChangedHandler in <see cref="WeakReference"/>, in this case, the callee should hold the strong reference of sourceChangedHandler to prevent it collected by GC.
        /// </param>  
        /// <returns>
        ///     The <see cref="T"/>.
        /// </returns>
        public T SetSourceChanged<T>(BindingValueChangedHandler sourceChangedHandler, bool isWeakReference = false) where T : WeakBinding
        {
            return this.SetSourceChanged(sourceChangedHandler, isWeakReference) as T;
        }

        /// <summary>
        ///     Sets the source changed.
        /// </summary>
        /// <param name="sourceChangedHandler">
        ///     The source changed handler.
        /// </param>
        /// <param name="isWeakReference">
        ///     Indicates whether the <see cref="WeakNotifyBinding"/> should hold the sourceChangedHandler in <see cref="WeakReference"/>, in this case, the callee should hold the strong reference of sourceChangedHandler to prevent it collected by GC.
        /// </param>        
        /// <returns>
        ///     The <see cref="WeakBinding"/>.
        /// </returns>
        public WeakBinding SetSourceChanged(BindingValueChangedHandler sourceChangedHandler, bool isWeakReference = false)
        {
            if (isWeakReference)
            {
                this.weakSourceChanged = new WeakReference(sourceChangedHandler);
            }
            else
            {
                this.sourceChanged = sourceChangedHandler;
            }

            return this;
        }

        /// <summary>
        ///     Sets the mode.
        /// </summary>
        /// <typeparam name="T">
        ///     The concrete <see cref="WeakBinding"/> to return.
        /// </typeparam>
        /// <param name="mode">
        ///     The mode.
        /// </param>
        /// <returns>
        ///     The <see cref="T"/>.
        /// </returns>
        public T SetMode<T>(BindMode mode) where T : WeakBinding
        {
            return this.SetMode(mode) as T;
        }

        /// <summary>
        ///     Sets the mode.
        /// </summary>
        /// <param name="mode">
        ///     The mode.
        /// </param>
        /// <returns>
        ///     The <see cref="WeakBinding"/>.
        /// </returns>
        public virtual WeakBinding SetMode(BindMode mode)
        {
            if (this.bindMode == mode)
            {
                return this;
            }

            this.bindMode = mode;

            this.DoConventions();
            this.Refresh();

            return this;
        }

        /// <summary>
        ///     Updates the specified source property.
        /// </summary>
        /// <param name="targetProperty">
        ///     The target property.
        /// </param>
        /// <param name="source">
        ///     The source.
        /// </param>
        /// <param name="sourceProperty">
        ///     The source property.
        /// </param>
        public void Update(string targetProperty, object source, string sourceProperty)
        {
            this.Clear();

            this.TargetProperty = targetProperty;
            this.SourceProperty = sourceProperty;

            this.BindTarget = new BindSource(this.Target, targetProperty);
            this.BindTarget.SourceChanged += this.OnBindTargetChanged;
            this.BindSource = new BindSource(source, sourceProperty);
            this.BindSource.SourceChanged += this.OnBindSourceChanged;

            this.Initialize<WeakBinding>();
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Attaches the event to bind target.
        /// </summary>
        /// <param name="data">
        ///     The data.
        /// </param>
        /// <param name="eventName">
        ///     Name of the event.
        /// </param>
        /// <param name="filter">
        ///     The filter.
        /// </param>
        /// <returns>
        ///     The <see cref="WeakBinding"/>.
        /// </returns>
        internal WeakBinding AttachTargetEvent(BindSource data, string eventName, WeakEventFilterHandler filter = null)
        {
            this.TargetEvent.AttachEvent(data, eventName, TargetEventHandlerName, filter);
            return this;
        }

        /// <summary>
        ///     Attaches the event to bind source.
        /// </summary>
        /// <param name="data">
        ///     The data.
        /// </param>
        /// <param name="eventName">
        ///     The event name.
        /// </param>
        /// <param name="filter">
        ///     The filter.
        /// </param>
        /// <returns>
        ///     The <see cref="WeakBinding"/>.
        /// </returns>
        internal WeakBinding AttachSourceEvent(BindSource data, string eventName, WeakEventFilterHandler filter = null)
        {
            this.SourceEvent.AttachEvent(data, eventName, SourceEventHandlerName, filter);
            return this;
        }

        /// <summary>
        ///     Gets the bind source.
        /// </summary>
        /// <param name="target">
        ///     The target.
        /// </param>
        /// <param name="property">
        ///     The property.
        /// </param>
        /// <returns>
        ///     The <see cref="BindSource"/>.
        /// </returns>
        internal BindSource GetBindSource(object target, string property)
        {
            BindSource data;
            if (target == this.BindTarget.OriginalSource && property == this.BindTarget.Property)
            {
                data = this.BindTarget;
            }            
            else if (target == this.BindSource.OriginalSource && property == this.BindSource.Property)
            {
                data = this.BindSource;
            }
            else
            {
                data = new BindSource(target, property);
            }

            return data;
        }

        /// <summary>
        ///     Adds the convention.
        /// </summary>
        /// <param name="bindTarget">
        ///     The bind target.
        /// </param>
        /// <param name="eventName">
        ///     Name of the event.
        /// </param>
        /// <param name="eventFilter">
        ///     The event filter.
        /// </param>
        protected void AddConvention(BindTarget bindTarget, string eventName, WeakEventFilterHandler eventFilter = null)
        {
            switch (bindTarget)
            {
                case Utility.BindTarget.Target:
                    if (!this.TargetEvent.IsAttached)
                    {
                        this.AttachTargetEvent(eventName, eventFilter);
                    }

                    break;

                case Utility.BindTarget.Source:
                    if (!this.SourceEvent.IsAttached)
                    {
                        this.AttachSourceEvent(eventName, eventFilter);
                    }

                    break;
            }
        }

        /// <summary>
        ///     Do the conventions, used for the specific <see cref="WeakBinding"/> to attach its own event.
        /// </summary>
        protected virtual void DoConventions()
        {
        }

        /// <summary>
        ///     Gets the bind data.
        /// </summary>
        /// <returns>The <see cref="BindData" />.</returns>
        protected virtual BindData GetBindData()
        {
            return new BindData(this.BindTarget, this.BindSource);
        }

        /// <summary>
        ///     Notifies the target changed.
        /// </summary>
        /// <param name="value">
        ///     The value.
        /// </param>
        /// <param name="oldValue">
        ///     The old value.
        /// </param>
        protected void NotifyTargetChanged(object value, object oldValue)
        {
            var targetChangedHandler = this.GetTargetChanged();
            if (targetChangedHandler != null)
            {
                targetChangedHandler(
                    this,
                    new BindingValueChangedEventArgs(this.GetBindData(), oldValue, value));
            }
        }

        /// <summary>
        ///     Notifies the source changed.
        /// </summary>
        /// <param name="value">
        ///     The value.
        /// </param>
        /// <param name="oldValue">
        ///     The old value.
        /// </param>
        protected void NotifySourceChanged(object value, object oldValue)
        {
            var sourceChangedHandler = this.GetSourceChanged();
            if (sourceChangedHandler != null)
            {
                sourceChangedHandler(
                    this,
                    new BindingValueChangedEventArgs(this.GetBindData(), oldValue, value));
            }
        }

        /// <summary>
        ///     Updates the target value.
        /// </summary>
        /// <param name="args">
        ///     The <see cref="EventArgs"/> instance containing the event data.
        /// </param>
        protected virtual void UpdateTargetValue(EventArgs args)
        {
        }

        /// <summary>
        ///     Updates the source value.
        /// </summary>
        /// <param name="args">
        ///     The <see cref="EventArgs"/> instance containing the event data.
        /// </param>
        protected virtual void UpdateSourceValue(EventArgs args)
        {
        }

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
        protected virtual bool OnTargetEventOccurred(object sender, EventArgs args)
        {
            return this.CheckValidation();
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
        ///     <c>true</c> if the event is valid, <c>false</c> otherwise.
        /// </returns>
        protected virtual bool OnSourceEventOccurred(object sender, EventArgs args)
        {
            return this.CheckValidation();
        }

        /// <summary>
        ///     Check the validation of binding, if the bind target is collected by GC, clear the resource.
        /// </summary>
        /// <returns>
        ///     <c>true</c> if valid, <c>false</c> otherwise.
        /// </returns>
        protected bool CheckValidation()
        {
            if (!this.activate)
            {
                return false;
            }

            if (!this.IsAlive)
            {
                this.Clear();
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Gets the source changed handler, returns null if not exist.
        /// </summary>
        /// <returns>The source changed handler</returns>
        private BindingValueChangedHandler GetSourceChanged()
        {
            if (this.sourceChanged != null)
            {
                return this.sourceChanged;
            }

            if (this.weakSourceChanged != null && this.weakTargetChanged.IsNotNullAndAlive())
            {
                return (BindingValueChangedHandler)this.weakSourceChanged.Target;
            }

            return null;
        }

        /// <summary>
        ///     Gets the target changed handler, returns null if not exist.
        /// </summary>
        /// <returns>The target changed handler</returns>
        private BindingValueChangedHandler GetTargetChanged()
        {
            if (this.targetChanged != null)
            {
                return this.targetChanged;
            }

            if (this.weakTargetChanged != null && this.weakTargetChanged.IsNotNullAndAlive())
            {
                return (BindingValueChangedHandler)this.weakTargetChanged.Target;
            }

            return null;
        }

        /// <summary>
        ///     Called when BindTarget changed.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The <see cref="SourceChangedEventArgs"/> instance containing the event data.
        /// </param>
        private void OnBindTargetChanged(object sender, SourceChangedEventArgs e)
        {
            if (this.CheckValidation())
            {
                this.UpdateSourceValue(e);
            }
        }

        /// <summary>
        ///     Called when BindSource changed.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The <see cref="SourceChangedEventArgs"/> instance containing the event data.
        /// </param>
        private void OnBindSourceChanged(object sender, SourceChangedEventArgs e)
        {
            if (this.CheckValidation())
            {
                this.UpdateTargetValue(e);
            }
        }

        #endregion
    }
}