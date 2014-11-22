// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WeakCommandBinding.cs" company="Illusion">
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
//   The concrete <see cref="WeakBinding"/> that provides the binding for command.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Illusion.Utility
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Reflection;

    /// <summary>
    ///     The concrete <see cref="WeakBinding"/> that provides the binding for command.
    /// </summary>
    public class WeakCommandBinding : WeakBinding
    {
        #region Static Fields

        /// <summary>
        ///     The can execute changed callback method.
        /// </summary>
        public static string CanExecuteChangedCallbackMethod = "OnCanExecuteChanged";

        /// <summary>
        ///     The can execute changed method.
        /// </summary>
        public static string CanExecuteChangedMethod = "CanExecuteChanged";

        /// <summary>
        ///     The can execute method.
        /// </summary>
        public static string CanExecuteMethod = "CanExecute";

        /// <summary>
        ///     The execute method.
        /// </summary>
        public static string ExecuteMethod = "Execute";

        /// <summary>
        ///     The can execute changed method.
        /// </summary>
        public static Action<object> InvokeCanExecuteChanged =
            o => DynamicEngine.RaiseEvent(o, CanExecuteChangedMethod, EventArgs.Empty);

        /// <summary>
        ///     The can execute method.
        /// </summary>
        public static Func<object, object, bool> InvokeCanExecuteChangedWithParameter = (o, p) =>
            {
                var methodInfo = DynamicEngine.GetMethodInfo(o.GetType(), CanExecuteMethod, new[] { p });
                ParameterInfo[] parameters = methodInfo.GetParameters();
                if (parameters.Length > 0)
                {
                    p = p.ConvertTo(parameters[0].ParameterType);
                }

                return (bool)DynamicEngine.InvokeMethod(o, methodInfo, new[] { p });
            };

        /// <summary>
        ///     The execute method.
        /// </summary>
        public static Action<object, object> InvokeExecute = (o, p) =>
            {
                var methodInfo = DynamicEngine.GetMethodInfo(o.GetType(), CanExecuteMethod, new[] { p });
                ParameterInfo[] parameters = methodInfo.GetParameters();
                if (parameters.Length > 0)
                {
                    p = p.ConvertTo(parameters[0].ParameterType);
                }

                DynamicEngine.InvokeMethod(o, ExecuteMethod, new[] { p });
            };

        /// <summary>
        ///     The watch event callback method.
        /// </summary>
        public static string WatchEventCallbackMethod = "OnWatchEventOccurred";

        #endregion

        #region Fields

        /// <summary>
        ///     The command can execute event.
        /// </summary>
        private readonly WeakEvent commandCanExecuteEvent;

        /// <summary>
        ///     The enables properties.
        /// </summary>
        private readonly IList<string> enablesProperties = new List<string>();

        /// <summary>
        ///     The watch events.
        /// </summary>
        private readonly Dictionary<WeakEntry, WeakEvent> watchEvents = new Dictionary<WeakEntry, WeakEvent>();

        /// <summary>
        ///     The can execute callback.
        /// </summary>
        private WeakReference canExecuteCallback;

        /// <summary>
        ///     The parameter.
        /// </summary>
        private BindSource parameter;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="WeakCommandBinding"/> class.
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
        public WeakCommandBinding(object target, string targetProperty, object source, string sourceProperty)
            : base(target, targetProperty, source, sourceProperty)
        {
            this.commandCanExecuteEvent = new WeakEvent(this);
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Adds the enable property.
        /// </summary>
        /// <param name="propertyName">
        ///     Name of the property.
        /// </param>
        /// <returns>
        ///     The <see cref="WeakCommandBinding"/>.
        /// </returns>
        public WeakCommandBinding AddEnableProperty(string propertyName)
        {
            if (!this.enablesProperties.Contains(propertyName))
            {
                this.enablesProperties.Add(propertyName);
            }

            this.UpdateEnableValues();
            return this;
        }

        /// <summary>
        ///     Adds the enable property.
        /// </summary>
        /// <typeparam name="T">
        ///     The concrete <see cref="WeakBinding"/> to return.
        /// </typeparam>
        /// <param name="propertyExpression">
        ///     The property expression.
        /// </param>
        /// <returns>
        ///     The <see cref="WeakCommandBinding"/>.
        /// </returns>
        public WeakCommandBinding AddEnableProperty<T>(Expression<Func<T, bool>> propertyExpression) where T : class
        {
            return this.AddEnableProperty(propertyExpression.GetName());
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
            this.SetSourceBindMode(SourceMode.Property);

            return base.Initialize<T>(isActivate);
        }

        /// <summary>
        ///     Removes the enable property.
        /// </summary>
        /// <param name="propertyName">
        ///     Name of the property.
        /// </param>
        /// <returns>
        ///     The <see cref="WeakCommandBinding"/>.
        /// </returns>
        public WeakCommandBinding RemoveEnableProperty(string propertyName)
        {
            if (this.enablesProperties.Contains(propertyName))
            {
                this.enablesProperties.Remove(propertyName);
            }

            return this;
        }

        /// <summary>
        ///     Removes the enable property.
        /// </summary>
        /// <typeparam name="T">
        ///     The concrete <see cref="WeakBinding"/> to return.
        /// </typeparam>
        /// <param name="propertyExpression">
        ///     The property expression.
        /// </param>
        /// <returns>
        ///     The <see cref="WeakCommandBinding"/>.
        /// </returns>
        public WeakCommandBinding RemoveEnableProperty<T>(Expression<Func<T, bool>> propertyExpression) where T : class
        {
            return this.RemoveEnableProperty(propertyExpression.GetName());
        }

        /// <summary>
        ///     Sets the can execute changed.
        /// </summary>
        /// <param name="canExecuteChangedHandler">
        ///     The can execute changed handler.
        /// </param>
        /// <returns>
        ///     The <see cref="WeakCommandBinding"/>.
        /// </returns>
        public WeakCommandBinding SetCanExecuteChanged(CommandCanExecuteChangedHandler canExecuteChangedHandler)
        {
            this.canExecuteCallback = canExecuteChangedHandler != null
                                          ? new WeakReference(canExecuteChangedHandler)
                                          : null;
            this.RaiseCanExecuteChanged();
            return this;
        }

        /// <summary>
        ///     Sets the parameter.
        /// </summary>
        /// <param name="source">
        ///     The source.
        /// </param>
        /// <returns>
        ///     The <see cref="WeakCommandBinding"/>.
        /// </returns>
        public WeakCommandBinding SetParameter(object source)
        {
            return this.SetParameter(source, null);
        }

        /// <summary>
        ///     Sets the parameter.
        /// </summary>
        /// <param name="source">
        ///     The source.
        /// </param>
        /// <param name="property">
        ///     The property.
        /// </param>
        /// <returns>
        ///     The <see cref="WeakCommandBinding"/>.
        /// </returns>
        public WeakCommandBinding SetParameter(object source, string property)
        {
            this.parameter = source == null ? null : new BindSource(source, property);

            this.RaiseCanExecuteChanged();
            return this;
        }

        /// <summary>
        ///     UnWatch the target property.
        /// </summary>
        /// <typeparam name="T">
        ///     The concrete <see cref="WeakBinding"/> to return.
        /// </typeparam>
        /// <param name="propertyExpression">
        ///     The property expression.
        /// </param>
        /// <returns>
        ///     The <see cref="WeakCommandBinding"/>.
        /// </returns>
        public WeakCommandBinding UnWatch<T>(Expression<Func<T, object>> propertyExpression) where T : class
        {
            return this.UnWatch(propertyExpression.GetName());
        }

        /// <summary>
        ///     UnWatch the source property.
        /// </summary>
        /// <param name="sourceProperty">
        ///     The source property.
        /// </param>
        /// <returns>
        ///     The <see cref="WeakCommandBinding"/>.
        /// </returns>
        public WeakCommandBinding UnWatch(string sourceProperty)
        {
            return this.UnWatch(this.BindSource.Source, sourceProperty);
        }

        /// <summary>
        ///     UnWatch the source property.
        /// </summary>
        /// <param name="source">
        ///     The source.
        /// </param>
        /// <param name="property">
        ///     The property.
        /// </param>
        /// <returns>
        ///     The <see cref="WeakCommandBinding"/>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        ///     The source can not be null.
        /// </exception>
        public WeakCommandBinding UnWatch(object source, string property)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            var bindSource = new BindSource(source, property);

            var entry = new WeakEntry(null, bindSource.Source, property);
            if (this.watchEvents.ContainsKey(entry))
            {
                WeakEvent watchEvent = this.watchEvents[entry];
                watchEvent.DetachEvent();
                this.watchEvents.Remove(entry);
            }

            this.RaiseCanExecuteChanged();

            return this;
        }

        /// <summary>
        ///     Watches the specified property expression.
        /// </summary>
        /// <typeparam name="T">
        ///     The <see cref="WeakBinding"/>.
        /// </typeparam>
        /// <param name="propertyExpression">
        ///     The property expression.
        /// </param>
        /// <returns>
        ///     The <see cref="WeakCommandBinding"/>.
        /// </returns>
        public WeakCommandBinding Watch<T>(Expression<Func<T, object>> propertyExpression) where T : class
        {
            return this.Watch(propertyExpression.GetName());
        }

        /// <summary>
        ///     Watches the specified source property.
        /// </summary>
        /// <param name="sourceProperty">
        ///     The source property.
        /// </param>
        /// <returns>
        ///     The <see cref="WeakCommandBinding"/>.
        /// </returns>
        public WeakCommandBinding Watch(string sourceProperty)
        {
            return this.Watch(this.BindSource.Source, sourceProperty);
        }

        /// <summary>
        ///     Watches the specified source.
        /// </summary>
        /// <param name="source">
        ///     The source.
        /// </param>
        /// <param name="property">
        ///     The property.
        /// </param>
        /// <returns>
        ///     The <see cref="WeakCommandBinding"/>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        ///     The source can not be null.
        /// </exception>
        public WeakCommandBinding Watch(object source, string property)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            var bindSource = new BindSource(source, property);

            var entry = new WeakEntry(null, bindSource.Source, property);
            WeakEvent watchEvent;
            if (this.watchEvents.ContainsKey(entry))
            {
                watchEvent = this.watchEvents[entry];
            }
            else
            {
                watchEvent = new WeakEvent(this);
                this.watchEvents.Add(entry, watchEvent);
            }

            string properyName = property.Contains(".") ? property.Right(".") : property;

            watchEvent.AttachEvent(
                bindSource,
                WeakPropertyBinding.PropertyChangedEventName,
                WatchEventCallbackMethod,
                (o, args) => string.Equals(((PropertyChangedEventArgs)args).PropertyName, properyName));

            this.RaiseCanExecuteChanged();

            return this;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Does the conventions.
        /// </summary>
        protected override void DoConventions()
        {
            base.DoConventions();

            this.commandCanExecuteEvent.AttachEvent(
                this.BindSource,
                CanExecuteChangedMethod,
                CanExecuteChangedCallbackMethod);
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
            this.UpdateEnableValues();
        }

        /// <summary>
        ///     Handles the <see cref="E:CanExecuteChanged"/> event.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="args">
        ///     The <see cref="EventArgs"/> instance containing the event data.
        /// </param>
        protected virtual void OnCanExecuteChanged(object sender, EventArgs args)
        {
            bool enable = this.UpdateEnableValues();

            if (this.canExecuteCallback.IsNotNullAndAlive())
            {
                ((CommandCanExecuteChangedHandler)this.canExecuteCallback.Target)(
                    sender,
                    new CanExecuteChangedEventArgs(this.GetBindData(), enable));
            }
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
        protected override bool OnTargetEventOccurred(object sender, EventArgs args)
        {
            if (!base.OnTargetEventOccurred(sender, args))
            {
                return false;
            }

            object source = this.BindSource.Value;
            if (source != null)
            {
                object p = this.parameter.TryGetValue();
                if (InvokeCanExecuteChangedWithParameter(source, p))
                {
                    InvokeExecute(source, p);
                }

                this.RaiseCanExecuteChanged();
            }

            return true;
        }

        /// <summary>
        ///     Handles the <see cref="E:WatchEventOccurred"/> event.
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
        protected virtual bool OnWatchEventOccurred(object sender, EventArgs args)
        {
            this.RaiseCanExecuteChanged();

            return true;
        }

        /// <summary>
        ///     Raises the can execute changed.
        /// </summary>
        private void RaiseCanExecuteChanged()
        {
            if (this.BindSource.Value != null)
            {
                InvokeCanExecuteChanged(this.BindSource.Value);
            }
        }

        /// <summary>
        ///     Updates the enable values.
        /// </summary>
        /// <returns><c>true</c> if enable, <c>false</c> otherwise.</returns>
        private bool UpdateEnableValues()
        {
            object target = this.BindSource.Value;
            bool enable = target != null && InvokeCanExecuteChangedWithParameter(target, this.parameter.TryGetValue());
            foreach (string property in this.enablesProperties)
            {
                DynamicEngine.SetProperty(this.BindTarget.Source, property, enable);
            }

            return enable;
        }

        #endregion
    }
}