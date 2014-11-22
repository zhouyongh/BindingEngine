// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WeakPropertyBinding.cs" company="Illusion">
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
//   The concrete <see cref="WeakBinding"/> that provides the binding for property.The weak property binding.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Illusion.Utility
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;

    /// <summary>
    ///     The concrete <see cref="WeakBinding"/> that provides the binding for property.
    /// </summary>
    public class WeakPropertyBinding : WeakBinding
    {
        #region Static Fields

        /// <summary>
        ///     The property changed event name.
        /// </summary>
        public static readonly string PropertyChangedEventName = "PropertyChanged";

        #endregion

        #region Fields

        /// <summary>
        ///     The target property getter.
        /// </summary>
        protected Func<BindData, object> targetPropertyGetter;

        /// <summary>
        ///     The source property getter.
        /// </summary>
        protected Func<BindData, object> sourcePropertyGetter;

        /// <summary>
        ///     The target property setter.
        /// </summary>
        protected Action<BindData, object> targetPropertySetter;

        /// <summary>
        ///     The source property setter.
        /// </summary>
        protected Action<BindData, object> sourcePropertySetter;

        /// <summary>
        ///     The data converter
        /// </summary>
        private IDataConverter dataConverter;

        /// <summary>
        ///     The boolean value indicate whether is updating.
        /// </summary>
        private bool isUpdating;

        /// <summary>
        ///     The parameter.
        /// </summary>
        private object parameter;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="WeakPropertyBinding"/> class.
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
        public WeakPropertyBinding(object target, string targetProperty, object source, string sourceProperty)
            : base(target, targetProperty, source, sourceProperty)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the data converter.
        /// </summary>
        /// <value>The data converter.</value>
        public IDataConverter DataConverter
        {
            get
            {
                return this.dataConverter;
            }

            set
            {
                this.dataConverter = value;
                this.Refresh();
            }
        }

        /// <summary>
        ///     Gets or sets the parameter.
        /// </summary>
        /// <value>The parameter.</value>
        public virtual object Parameter
        {
            get
            {
                return this.parameter;
            }

            set
            {
                this.parameter = value;
                this.Refresh();
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Sets the converter.
        /// </summary>
        /// <typeparam name="T">
        ///     The concrete <see cref="WeakBinding"/> to return.
        /// </typeparam>
        /// <param name="converter">
        ///     The converter.
        /// </param>
        /// <returns>
        ///     The <see cref="T"/>.
        /// </returns>
        public T SetConverter<T>(IDataConverter converter) where T : WeakPropertyBinding
        {
            return this.SetConverter(converter) as T;
        }

        /// <summary>
        ///     Sets the converter.
        /// </summary>
        /// <param name="converter">
        ///     The converter.
        /// </param>
        /// <returns>
        ///     The <see cref="WeakPropertyBinding"/>.
        /// </returns>
        public WeakPropertyBinding SetConverter(IDataConverter converter)
        {
            this.DataConverter = converter;
            this.Refresh();
            return this;
        }

        /// <summary>
        ///     Sets the parameter.
        /// </summary>
        /// <typeparam name="T">
        ///     The concrete <see cref="WeakBinding"/> to return.
        /// </typeparam>
        /// <param name="parameter">
        ///     The parameter.
        /// </param>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public T SetParameter<T>(object parameter) where T : WeakPropertyBinding
        {
            return this.SetParameter(parameter) as T;
        }

        /// <summary>
        ///     Sets the parameter.
        /// </summary>
        /// <param name="parameter">
        ///     The parameter.
        /// </param>
        /// <returns>
        ///     The <see cref="WeakPropertyBinding"/>.
        /// </returns>
        public WeakPropertyBinding SetParameter(object parameter)
        {
            this.Parameter = parameter;
            this.Refresh();
            return this;
        }

        /// <summary>
        ///     Sets the target property getter.
        /// </summary>
        /// <param name="targetPropGetter">
        ///     The target property getter.
        /// </param>
        /// <returns>
        ///     The <see cref="WeakPropertyBinding"/>.
        /// </returns>
        public WeakPropertyBinding SetTargetPropertyGetter(Func<BindData, object> targetPropGetter)
        {
            this.targetPropertyGetter = targetPropGetter;
            this.Refresh();
            return this;
        }

        /// <summary>
        ///     Sets the target property setter.
        /// </summary>
        /// <param name="targetPropSetter">
        ///     The target property setter.
        /// </param>
        /// <returns>
        ///     The <see cref="WeakPropertyBinding"/>.
        /// </returns>
        public WeakPropertyBinding SetTargetPropertySetter(Action<BindData, object> targetPropSetter)
        {
            this.targetPropertySetter = targetPropSetter;
            this.Refresh();
            return this;
        }

        /// <summary>
        ///     Sets the source property getter.
        /// </summary>
        /// <param name="sourcePropGetter">
        ///     The source property getter.
        /// </param>
        /// <returns>
        ///     The <see cref="WeakPropertyBinding"/>.
        /// </returns>
        public WeakPropertyBinding SetSourcePropertyGetter(Func<BindData, object> sourcePropGetter)
        {
            this.sourcePropertyGetter = sourcePropGetter;
            this.Refresh();
            return this;
        }

        /// <summary>
        ///     Sets the source property setter.
        /// </summary>
        /// <param name="sourcePropSetter">
        ///     The source property setter.
        /// </param>
        /// <returns>
        ///     The <see cref="WeakPropertyBinding"/>.
        /// </returns>
        public WeakPropertyBinding SetSourcePropertySetter(Action<BindData, object> sourcePropSetter)
        {
            this.sourcePropertySetter = sourcePropSetter;
            this.Refresh();
            return this;
        }

        /// <summary>
        ///     Updates the specified binding.
        /// </summary>
        /// <param name="sourceToTarget">
        ///     if set to <c>true</c> from source to target.
        /// </param>
        public void Update(bool sourceToTarget = true)
        {
            if (this.isUpdating)
            {
                return;
            }

            try
            {
                this.isUpdating = true;

                if (sourceToTarget)
                {
                    this.SourceToTarget();
                }
                else
                {
                    this.TargetToSource();
                }
            }
            finally
            {
                this.isUpdating = false;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Adds the convention.
        /// </summary>
        /// <param name="bindMode">
        ///     The bind mode.
        /// </param>
        /// <param name="bindTarget">
        ///     The bind target.
        /// </param>
        /// <param name="eventName">
        ///     Name of the event.
        /// </param>
        /// <param name="eventFilter">
        ///     The event filter.
        /// </param>
        protected void AddConvention(
            BindMode bindMode,
            BindTarget bindTarget,
            string eventName,
            WeakEventFilterHandler eventFilter = null)
        {
            if ((bindMode & this.bindMode) > 0)
            {
                this.AddConvention(bindTarget, eventName, eventFilter);
            }
        }

        /// <summary>
        ///     Does the conventions.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1126:PrefixCallsCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        protected override void DoConventions()
        {
            this.AddConvention(
                BindMode.OneWay,
                Utility.BindTarget.Source,
                PropertyChangedEventName,
                this.SourcePropertyChangedEventFilter);

            this.AddConvention(
                BindMode.OneWayToSource,
                Utility.BindTarget.Target,
                PropertyChangedEventName,
                this.TargetPropertyChangedEventFilter);
        }

        /// <summary>
        ///     Gets the target value.
        /// </summary>
        /// <returns>The <see cref="Object" />.</returns>
        protected virtual object GetTargetValue()
        {
            object value;

            if (this.targetPropertyGetter != null)
            {
                value = this.targetPropertyGetter(this.GetBindData());
            }
            else
            {
                value = this.BindTarget.GetValue();
            }

            if (this.DataConverter != null)
            {
                value = this.DataConverter.ConvertBack(value, this.BindSource.PropertyType, this.Parameter, CultureInfo.CurrentCulture);
            }

            return value;
        }

        /// <summary>
        ///     Gets the source value.
        /// </summary>
        /// <returns>The <see cref="Object" />.</returns>
        protected virtual object GetSourceValue()
        {
            object value;

            if (this.sourcePropertyGetter != null)
            {
                value = this.sourcePropertyGetter(this.GetBindData());
            }
            else
            {
                value = this.BindSource.GetValue();
            }

            if (this.DataConverter != null)
            {
                value = this.DataConverter.Convert(value, BindTarget.PropertyType, this.Parameter, CultureInfo.CurrentUICulture);
            }

            return value;
        }

        /// <summary>
        ///     Determines whether the specified <see cref="BindMode"/> is bind mode.
        /// </summary>
        /// <param name="bindMode">
        ///     The bind mode.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="BindMode"/> is bind mode; otherwise, <c>false</c>.
        /// </returns>
        protected bool IsBindMode(BindMode bindMode)
        {
            return (this.bindMode & bindMode) > 0;
        }

        /// <summary>
        ///     Updates the target value.
        /// </summary>
        /// <param name="args">
        ///     The <see cref="EventArgs"/> instance containing the event data.
        /// </param>
        protected override void UpdateTargetValue(EventArgs args)
        {
            if (this.IsBindMode(BindMode.OneWay))
            {
                this.Update();
            }
        }

        /// <summary>
        ///     Updates the source value.
        /// </summary>
        /// <param name="args">
        ///     The <see cref="EventArgs"/> instance containing the event data.
        /// </param>
        protected override void UpdateSourceValue(EventArgs args)
        {
            if (this.IsBindMode(BindMode.OneWayToSource))
            {
                this.Update(false);
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
        ///     <c>true</c> if it should be handled, <c>false</c> otherwise.
        /// </returns>
        protected override bool OnTargetEventOccurred(object sender, EventArgs args)
        {
            if (base.OnTargetEventOccurred(sender, args))
            {
                this.UpdateSourceValue(args);
            }

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
        ///     <c>true</c> if it is handled, <c>false</c> otherwise.
        /// </returns>
        protected override bool OnSourceEventOccurred(object sender, EventArgs args)
        {
            if (!base.OnSourceEventOccurred(sender, args))
            {
                return false;
            }
            
            this.UpdateTargetValue(args);
            return true;
        }

        /// <summary>
        ///     Sets the target value.
        /// </summary>
        /// <param name="value">
        ///     The value.
        /// </param>
        protected virtual void SetTargetValue(object value)
        {
            object oldValue = this.GetTargetValue();

            if (this.targetPropertySetter != null)
            {
                this.targetPropertySetter(this.GetBindData(), value);
            }
            else
            {
                if (this.BindTarget.PropertyType != null)
                {
                    value = value.ConvertTo(this.BindTarget.PropertyType);
                }

                this.BindTarget.Value = value;
            }

            if (!object.Equals(this.BindTarget.Value, oldValue))
            {
                this.NotifyTargetChanged(value, oldValue);
            }

            // If one time binding, deactivate the binding, can be recover from Activate.
            if (this.bindMode == BindMode.OneTime && this.BindTarget.Value != this.BindTarget.PropertyType.DefaultValue())
            {
                this.DeActivate();
            }
        }

        /// <summary>
        ///     Sets the source value.
        /// </summary>
        /// <param name="value">
        ///     The value.
        /// </param>
        protected virtual void SetSourceValue(object value)
        {
            object oldValue = this.GetSourceValue();

            if (this.sourcePropertySetter != null)
            {
                this.sourcePropertySetter(this.GetBindData(), value);
            }
            else
            {
                if (this.BindSource.PropertyType != null)
                {
                    value = value.ConvertTo(this.BindSource.PropertyType);
                }

                this.BindSource.Value = value;
            }

            if (!object.Equals(oldValue, value))
            {
                this.NotifySourceChanged(value, oldValue);
            }
        }

        /// <summary>
        ///     Targets the property changed event filter.
        /// </summary>
        /// <param name="o">
        ///     The object.
        /// </param>
        /// <param name="args">
        ///     The <see cref="EventArgs"/> instance containing the event data.
        /// </param>
        /// <returns>
        ///     <c>true</c> if equals, <c>false</c> otherwise.
        /// </returns>
        protected bool TargetPropertyChangedEventFilter(object o, EventArgs args)
        {
            return string.Equals(((PropertyChangedEventArgs)args).PropertyName, this.BindTarget.Property);
        }

        /// <summary>
        ///     Sources the property changed event filter.
        /// </summary>
        /// <param name="o">
        ///     The o.
        /// </param>
        /// <param name="args">
        ///     The <see cref="EventArgs"/> instance containing the event data.
        /// </param>
        /// <returns>
        ///     <c>true</c> if equals, <c>false</c> otherwise.
        /// </returns>
        protected bool SourcePropertyChangedEventFilter(object o, EventArgs args)
        {
            return string.Equals(((PropertyChangedEventArgs)args).PropertyName, this.BindSource.Property);
        }

        /// <summary>
        ///     Sources to target.
        /// </summary>
        protected virtual void SourceToTarget()
        {
            if (this.BindTarget.PropertyType == null && this.targetPropertySetter == null)
            {
                return;
            }

            object value = this.GetSourceValue();
            if (!object.Equals(value, WeakBinding.DoNothing))
            {
                this.SetTargetValue(value);
            }
        }

        /// <summary>
        ///     Targets to source.
        /// </summary>
        protected virtual void TargetToSource()
        {
            if (this.BindSource.PropertyType == null && this.sourcePropertySetter == null)
            {
                return;
            }

            object value = this.GetTargetValue();
            if (!object.Equals(value, WeakBinding.DoNothing))
            {
                this.SetSourceValue(value);
            }
        }

        #endregion
    }
}