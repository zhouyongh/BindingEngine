// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BindContext.cs" company="Illusion">
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
//   Represents the source for binding, it support the recursive source, the recursive source link listen to the 
// <see cref="INotifyPropertyChanged"/>'s <see cref="PropertyChangedEventHandler"/> event.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Illusion.Utility
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    ///     Defines the source mode of <see cref="BindContext"/>.
    /// </summary>
    /// <remarks>
    ///     It specify the source mode when use the recursive binding. 
    ///     e.g, 
    ///     BindContext context = new BindContext(viewModel, "Group.CurrentPeople.Name"); 
    ///     if the <see cref="SourceMode"/> is source, the attach event will be used to CurrentPeople. if Property, the
    ///     attach event will be used to <see cref="String"/> : viewModel.Group.CurrentPeople.Name.
    /// </remarks>
    public enum SourceMode
    {
        /// <summary>
        ///     The source.
        /// </summary>
        Source,

        /// <summary>
        ///     The property.
        /// </summary>
        Property
    }

    /// <summary>
    ///     Represents the context for binding, it support the recursive source, the recursive source link listen to the 
    /// <see cref="INotifyPropertyChanged"/>'s <see cref="PropertyChangedEventHandler"/> event.
    /// </summary>
    public class BindContext
    {
        #region Fields

        /// <summary>
        ///     The property observation.
        /// </summary>
        private readonly NotifyPropertyObservation propertyObservation;

        /// <summary>
        ///     The source mode.
        /// </summary>
        private SourceMode sourceMode = SourceMode.Source;

        /// <summary>
        ///     The original source reference.
        /// </summary>
        private WeakReference originalSourceReference;

        /// <summary>
        ///     The source reference.
        /// </summary>
        private WeakReference sourceReference;

        /// <summary>
        ///     The old value reference.
        /// </summary>
        private WeakReference oldValueReference;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BindContext"/> class.
        /// </summary>
        /// <param name="target">
        ///     The target.
        /// </param>
        /// <param name="targetProperty">
        ///     The target property.
        /// </param>
        public BindContext(object target, string targetProperty)
        {
            this.OriginalSource = target;
            this.propertyObservation = this.CreatePropertyObservation(targetProperty);

            if (this.propertyObservation != null)
            {
                this.propertyObservation.Source = target;
                this.Property = this.Tail.Property;
            }
            else
            {
                this.Source = target;
            }

        }

        #endregion

        #region Public Events

        /// <summary>
        ///     The source changed.
        /// </summary>
        public event EventHandler<SourceChangedEventArgs> SourceChanged;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the property.
        /// </summary>
        public string Property { get; set; }

        /// <summary>
        ///     Gets the property type.
        /// </summary>
        public Type PropertyType
        {
            get
            {
                if (this.Tail != null)
                {
                    return this.Tail.PropertyType;
                }

                return null;
            }
        }

        /// <summary>
        ///     Gets or sets the original source.
        /// </summary>
        public object OriginalSource
        {
            get
            {
                return this.originalSourceReference != null ? this.originalSourceReference.Target : null;
            }

            set
            {
                this.originalSourceReference = new WeakReference(value);
            }
        }

        /// <summary>
        ///     Gets or sets the source.
        /// </summary>
        public object Source
        {
            get
            {
                return this.sourceReference != null ? this.sourceReference.Target : null;
            }

            set
            {
                this.sourceReference = new WeakReference(value);
            }
        }

        /// <summary>
        ///     Gets or sets the source mode.
        /// </summary>
        public SourceMode SourceMode
        {
            get
            {
                return this.sourceMode;
            }

            set
            {
                this.sourceMode = value;
                this.Update();
            }
        }

        /// <summary>
        ///     Gets or sets the value.
        /// </summary>
        public object Value
        {
            get
            {
                return this.GetValueCore();
            }

            set
            {
                this.SetValueCore(value);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the tail.
        /// </summary>
        private NotifyPropertyObservation Tail
        {
            get
            {
                NotifyPropertyObservation observation = this.propertyObservation;
                while (observation != null && observation.Child != null)
                {
                    observation = observation.Child;
                }

                return observation;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The clear.
        /// </summary>
        public void Clear()
        {
            if (this.propertyObservation != null)
            {
                this.propertyObservation.Clear();
            }
        }

        /// <summary>
        ///     Gets the source.
        /// </summary>
        /// <returns>
        ///     The <see cref="object" />.
        /// </returns>
        public object GetTarget()
        {
            return this.SourceMode == SourceMode.Source ? this.Source : this.Value;
        }

        /// <summary>
        ///     Gets the value.
        /// </summary>
        /// <returns>
        ///     The <see cref="object"/>.
        /// </returns>
        public object GetValue()
        {
            return this.GetValueCore();
        }

        /// <summary>
        ///     Update the instance.
        /// </summary>
        /// <param name="forceEvent">
        ///     Indicates whether force to raise the SourceChangedEvent.
        /// </param>
        public void Update(bool forceEvent = false)
        {
            if (this.propertyObservation == null)
            {
                return;
            }

            var tail = this.Tail;
            if (tail != null)
            {
                this.Source = tail.Source;
            }

            var value = this.SourceMode == SourceMode.Property ? this.Value : this.Source;
            var oldValue = this.oldValueReference != null ? this.oldValueReference.Target : null;
            if (!forceEvent && object.Equals(oldValue, value))
            {
                return;
            }

            var changedEventArgs = new SourceChangedEventArgs(this.SourceMode, oldValue, value);
            this.OnSourceChanged(changedEventArgs);

            oldValueReference = new WeakReference(value);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Creates property observation.
        /// </summary>
        /// <param name="targetProperty">
        ///     The target property.
        /// </param>
        /// <returns>
        ///     The <see cref="NotifyPropertyObservation"/>.
        /// </returns>
        private NotifyPropertyObservation CreatePropertyObservation(string targetProperty)
        {
            if (targetProperty == null)
            {
                return null;
            }

            int indexerIndex = targetProperty.IndexOf(BindingManager.IndexerLeftMark, StringComparison.Ordinal);
            int pointIndex = targetProperty.IndexOf(".", StringComparison.Ordinal);
            bool isIndex = indexerIndex == 0;

            string property = targetProperty;
            string childProperty = null;

            if (isIndex)
            {
                // Index Property
                int rightIndex = property.IndexOf(BindingManager.IndexerRightMark, StringComparison.Ordinal);
                property = property.Substring(1, rightIndex - 1);
                childProperty =
                    targetProperty.Substring(rightIndex + 1, targetProperty.Length - rightIndex - 1).TrimStart('.');
            }
            else if (indexerIndex + pointIndex > -1)
            {
                if (indexerIndex > -1 && (pointIndex == -1 || indexerIndex < pointIndex))
                {
                    property = targetProperty.Substring(0, indexerIndex);
                    childProperty = targetProperty.Substring(indexerIndex, targetProperty.Length - indexerIndex);
                }
                else if (pointIndex > -1)
                {
                    property = targetProperty.Substring(0, pointIndex);
                    childProperty = targetProperty.Substring(pointIndex + 1, targetProperty.Length - pointIndex - 1);
                }
            }

            NotifyPropertyObservation observation = isIndex
                                                                ? new NotifyIndexPropertyObservation(
                                                                      this, 
                                                                      property, 
                                                                      childProperty)
                                                                : new NotifyPropertyObservation(
                                                                      this, 
                                                                      property, 
                                                                      childProperty);

            return observation;
        }

        /// <summary>
        ///     Gets the value.
        /// </summary>
        /// <returns>
        ///     The <see cref="object" />.
        /// </returns>
        private object GetValueCore()
        {
            if (this.Tail != null)
            {
                return this.Tail.Value;
            }

            return this.Source;
        }

        /// <summary>
        ///     Called when source changed.
        /// </summary>
        /// <param name="args">
        ///     The args.
        /// </param>
        private void OnSourceChanged(SourceChangedEventArgs args)
        {
            if (this.SourceChanged != null)
            {
                this.SourceChanged(this, args);
            }
        }

        /// <summary>
        ///     Sets the value.
        /// </summary>
        /// <param name="value">
        ///     The value.
        /// </param>
        private void SetValueCore(object value)
        {
            if (this.Tail != null)
            {
                this.Tail.Value = value;
            }
        }

        #endregion

        #region Private Class NotifyPropertyObservation

        /// <summary>
        ///     The observation of notify property that support recursive link.
        /// </summary>
        private class NotifyPropertyObservation
        {
            #region Fields

            /// <summary>
            ///     The child.
            /// </summary>
            private readonly NotifyPropertyObservation child;

            /// <summary>
            ///     The data.
            /// </summary>
            private readonly BindContext data;

            /// <summary>
            ///     The source reference.
            /// </summary>
            private WeakReference sourceReference;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="NotifyPropertyObservation"/> class.
            /// </summary>
            /// <param name="data">
            ///     The data.
            /// </param>
            /// <param name="property">
            ///     The property.
            /// </param>
            /// <param name="childProperty">
            ///     The child property.
            /// </param>
            public NotifyPropertyObservation(BindContext data, string property, string childProperty)
            {
                this.data = data;
                if (!string.IsNullOrEmpty(childProperty))
                {
                    this.child = data.CreatePropertyObservation(childProperty);
                }

                this.Property = property;
            }

            #endregion

            #region Public Properties

            /// <summary>
            ///     Gets the child.
            /// </summary>
            public NotifyPropertyObservation Child
            {
                get
                {
                    return this.child;
                }
            }

            /// <summary>
            ///     Gets or sets the property.
            /// </summary>
            public string Property { get; set; }

            /// <summary>
            ///     Gets or sets the property type.
            /// </summary>
            public Type PropertyType { get; protected set; }

            /// <summary>
            ///     Gets or sets the source.
            /// </summary>
            public object Source
            {
                get
                {
                    return this.sourceReference != null ? this.sourceReference.Target : null;
                }

                set
                {
                    this.DetachSource(value);
                    this.AttachSource(value);

                    this.sourceReference = new WeakReference(value);

                    this.UpdatePropertyType();
                    this.Update();
                }
            }

            /// <summary>
            ///     Gets or sets the value.
            /// </summary>
            public virtual object Value
            {
                get
                {
                    return this.Source == null
                               ? this.PropertyType.DefaultValue()
                               : DynamicEngine.GetProperty(this.Source, this.Property);
                }

                set
                {
                    if (this.Source != null)
                    {
                        DynamicEngine.SetProperty(this.Source, this.Property, value);
                    }
                }
            }

            #endregion

            #region Public Methods and Operators

            /// <summary>
            ///     The clear.
            /// </summary>
            public void Clear()
            {
                if (this.child != null)
                {
                    this.child.Clear();
                }

                this.DetachSource(this.Source);
            }

            #endregion

            #region Methods

            /// <summary>
            ///     Attaches the source.
            /// </summary>
            /// <param name="source">
            ///     The source.
            /// </param>
            protected virtual void AttachSource(object source)
            {
                var changed = source as INotifyPropertyChanged;
                if (changed != null)
                {
                    changed.PropertyChanged += this.OnValueChanged;
                }
            }

            /// <summary>
            ///     Detaches the source.
            /// </summary>
            /// <param name="source">
            ///     The source.
            /// </param>
            protected virtual void DetachSource(object source)
            {
                var changed = source as INotifyPropertyChanged;
                if (changed != null)
                {
                    changed.PropertyChanged -= this.OnValueChanged;
                }
            }

            /// <summary>
            ///     Called when value changed.
            /// </summary>
            /// <param name="sender">
            ///     The sender.
            /// </param>
            /// <param name="e">
            ///     The e.
            /// </param>
            protected virtual void OnValueChanged(object sender, PropertyChangedEventArgs e)
            {
                if (string.Equals(e.PropertyName, this.Property))
                {
                    this.Update();
                }
            }

            /// <summary>
            ///     Updates the instance.
            /// </summary>
            protected void Update()
            {
                if (this.child != null)
                {
                    this.child.Source = this.Value;
                }
                else
                {
                    this.data.Update();
                }
            }

            /// <summary>
            ///     Updates the property type.
            /// </summary>
            /// <exception cref="MissingMemberException">
            ///     The property does not found.
            /// </exception>
            protected virtual void UpdatePropertyType()
            {
                if (this.Source != null)
                {
                    PropertyInfo property = this.Source.GetType().GetProperty(this.Property);
                    if (property == null)
                    {
                        throw new MissingMemberException(
                            string.Format("The property {0} does not found in {1}", this.Property, this.Source));
                    }

                    this.PropertyType = property.PropertyType;
                }
            }

            #endregion
        }
        
        #endregion

        #region Private Class NotifyIndexPropertyObservation

        /// <summary>
        ///     The observation of notify property that support recursive link, used for the index property.
        /// </summary>
        private class NotifyIndexPropertyObservation : NotifyPropertyObservation
        {
            #region Fields

            /// <summary>
            ///     The key.
            /// </summary>
            private object key;

            /// <summary>
            ///     The <see cref="WeakEvent"/> used when the collection is observable.
            /// </summary>
            private WeakEvent collectionWeakEvent;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="NotifyIndexPropertyObservation"/> class.
            /// </summary>
            /// <param name="data">
            ///     The data.
            /// </param>
            /// <param name="property">
            ///     The property.
            /// </param>
            /// <param name="childProperty">
            ///     The child property.
            /// </param>
            public NotifyIndexPropertyObservation(BindContext data, string property, string childProperty)
                : base(data, property, childProperty)
            {
            }

            #endregion

            #region Public Properties

            /// <summary>
            ///     Gets or sets the value.
            /// </summary>
            public override object Value
            {
                get
                {
                    return this.ContainsValue()
                               ? DynamicEngine.GetIndexProperty(this.Source, this.key)
                               : this.PropertyType.DefaultValue();
                }

                set
                {
                    if (this.ContainsValue())
                    {
                        DynamicEngine.SetIndexProperty(this.Source, this.key, value);
                    }
                }
            }

            #endregion

            #region Methods

            /// <summary>
            ///     Attaches the source.
            /// </summary>
            /// <param name="source">
            ///     The source.
            /// </param>
            protected override void AttachSource(object source)
            {
                base.AttachSource(source);

                if (source != null && source.GetType().ContainsEvent(WeakCollectionBinding.CollectionChangedEventName))
                {
                    if (collectionWeakEvent != null)
                    {
                        collectionWeakEvent.DetachEvent();
                    }

                    if (collectionWeakEvent == null)
                    {
                        collectionWeakEvent = new WeakEvent(this);
                    }

                    collectionWeakEvent.AttachEvent(source, null, WeakCollectionBinding.CollectionChangedEventName, "OnSourceCollectionChanged");
                }
            }

            /// <summary>
            ///     Detaches the source.
            /// </summary>
            /// <param name="source">
            ///     The source.
            /// </param>
            protected override void DetachSource(object source)
            {
                base.DetachSource(source);

                if (source != null && source.GetType().ContainsEvent(WeakCollectionBinding.CollectionChangedEventName))
                {
                    if (collectionWeakEvent != null)
                    {
                        collectionWeakEvent.DetachEvent();
                        collectionWeakEvent = null;
                    }
                }
            }

            /// <summary>
            ///     Called when value changed.
            /// </summary>
            /// <param name="sender">
            ///     The sender.
            /// </param>
            /// <param name="e">
            ///     The e.
            /// </param>
            protected override void OnValueChanged(object sender, PropertyChangedEventArgs e)
            {
                base.OnValueChanged(sender, e);

                if (string.Equals(e.PropertyName, BindingManager.IndexerName))
                {
                    this.Update();
                }
            }

            /// <summary>
            ///     Updates the property type.
            /// </summary>
            protected override void UpdatePropertyType()
            {
                if (this.Source != null)
                {
                    Type type = this.Source.GetType();
                    Type parameterType = null;

                    if (type.IsArray)
                    {
                        this.PropertyType = type.GetElementType();
                        parameterType = type.GetGenericArguments()[0];
                    }
                    else
                    {
                        var propertyInfos = type.GetProperties().Where(item => item.Name.Equals("Item")).ToArray();
                        if (propertyInfos.Count() == 1)
                        {
                            parameterType = propertyInfos[0].GetIndexParameters()[0].ParameterType;
                            this.PropertyType = propertyInfos[0].PropertyType;
                        }
                        else
                        {
                            foreach (var propertyInfo in propertyInfos)
                            {
                                parameterType = propertyInfo.GetIndexParameters()[0].ParameterType;
                                if (parameterType.IsConvertableFrom(this.Property))
                                {
                                    this.PropertyType = propertyInfo.PropertyType;
                                    break;
                                }
                                parameterType = null;
                            }
                        }
                    }

                    if (parameterType != null)
                    {
                        this.key = Convert.ChangeType(this.Property, parameterType, null);
                    }
                }
            }

            /// <summary>
            ///     Called when source collection changed.
            /// </summary>
            /// <param name="sender">
            ///     The sender.
            /// </param>
            /// <param name="args">
            ///     The args.
            /// </param>
            protected void OnSourceCollectionChanged(object sender, EventArgs args)
            {
                var arg = WeakCollectionBinding.GetCollectionChangedEventArgs(args);
                switch (arg.Action)
                {
                    case WeakCollectionAction.Add:
                    case WeakCollectionAction.Replace:
                        if (arg.NewStartingIndex.Equals(this.key))
                        {
                            this.Update();
                        }

                        break;
                    case WeakCollectionAction.Remove:
                        if (arg.OldStartingIndex.Equals(this.key))
                        {
                            this.Update();
                        }

                        break;
                    case WeakCollectionAction.Reset:
                        this.Update();
                        break;
                }

            }

            /// <summary>
            ///     Gets the value whether contains the key.
            /// </summary>
            /// <returns>
            ///     The <see cref="bool" />.
            /// </returns>
            /// <exception cref="NotSupportedException">
            ///     The recursion source only support IList and ICollection.
            /// </exception>
            private bool ContainsValue()
            {
                if (this.Source != null)
                {
                    var dictionary = this.Source as IDictionary;
                    if (dictionary != null)
                    {
                        return dictionary.Contains(this.key);
                    }

                    var list = this.Source as IList;
                    if (list == null)
                    {
                        throw new NotSupportedException("The recursion source only support IList and ICollection");
                    }

                    if (this.key is int)
                    {
                        return ((int)this.key) >= 0 && ((int)this.key) < list.Count;
                    }

                    return true;
                }

                return false;
            }

            #endregion
        }
        
        #endregion
    }
}