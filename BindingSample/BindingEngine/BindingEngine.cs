using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows.Input;

namespace BindingSample
{
    /// <summary>
    ///  A bind engine supports custom data binding.
    /// </summary>
    /// <remarks>
    /// The binding engine use weak reference to hold the dependent, that means if u miss to clear data binding,
    /// the dependent will not leak memory.
    /// </remarks>
    /// <author>
    ///  yohan zhou 
    ///  http://www.cnblogs.com/zhouyongh
    /// </author>
    public class BindingEngine
    {
        #region Field

        private static readonly Dictionary<int, WeakSource> weakSources = new Dictionary<int, WeakSource>();

        #endregion

        #region Binding Methods

        public static WeakMethodBinding SetMethodBinding<TSource, TTarget>(TSource source, Expression<Func<TSource, object>> sourceProperty, TTarget target, Expression<Func<TTarget, object>> targetProperty)
        {
            var weakSource = GetWeakSource(source);
            return weakSource.SetBindng<WeakMethodBinding>(target, sourceProperty.GetName(), targetProperty.GetName());
        }

        public static WeakCommandBinding SetCommandBinding<TSource, TTarget>(TSource source, string sourceEvent, TTarget target, Expression<Func<TTarget, ICommand>> targetExpression)
        {
            var weakSource = GetWeakSource(source);
            return weakSource.SetBindng<WeakCommandBinding>(target, null, targetExpression.GetName()).AttachSourceEvent(sourceEvent) as WeakCommandBinding;
        }

        public static WeakCollectionBinding SetCollectionBinding<TSource, TTarget>(TSource source, Expression<Func<TSource, object>> sourceProperty, TTarget target, Expression<Func<TTarget, object>> targetProperty)
        {
            return SetCollectionBinding(source, sourceProperty.GetName(), target, targetProperty.GetName());
        }

        public static WeakCollectionBinding SetCollectionBinding(object source, string sourceProperty, object target, string targetProperty)
        {
            var weakSource = GetWeakSource(source);
            var weakCollectionBinding = weakSource.SetBindng<WeakCollectionBinding>(target, sourceProperty, targetProperty);
            return weakCollectionBinding;
        }

        public static WeakProperyBinding SetPropertyBinding<TSource, TTarget>(TSource source, Expression<Func<TSource, object>> sourceProperty, TTarget target,
            Expression<Func<TTarget, object>> targetExpression)
        {
            return SetPropertyBinding(source, sourceProperty.GetName(), target, targetExpression.GetName());
        }

        public static WeakProperyBinding SetPropertyBinding(object source, string sourceProperty, object target, string targetProperty)
        {
            var weakSource = GetWeakSource(source);
            return weakSource.SetBindng<WeakProperyBinding>(target, sourceProperty, targetProperty);
        }

        public static void ClearBinding(object source, string sourceProperty, object target, string targetProperty)
        {
            var weakSource = GetWeakSource(source);
            weakSource.ClearBinding(target, sourceProperty, targetProperty);
        }

        public static void ClearAllBindings()
        {
            foreach (var weakSource in weakSources.Values)
            {
                weakSource.ClearBindings();
            }
            weakSources.Clear();
        }

        private static WeakSource GetWeakSource(object source)
        {
            int hashCode = source.GetHashCode();
            if (weakSources.ContainsKey(hashCode))
            {
                return weakSources[hashCode];
            }

            //Clear dead weak source
            var deadSources = weakSources.Where(item => !item.Value.IsAlive).Select(item => item.Key);
            foreach (int deadSource in deadSources)
            {
                weakSources[deadSource].ClearBindings();
                weakSources.Remove(deadSource);
            }

            WeakSource weakSource = new WeakSource(source);
            weakSources.Add(source.GetHashCode(), weakSource);
            return weakSource;
        }

        #endregion

        private struct WeakEntry
        {
            private readonly int _targetHashcode;
            private readonly string _sourceProperty;
            private readonly string _targetProperty;

            public WeakEntry(int targetHashcode, string sourceProperty, string targetProperty)
            {
                _targetHashcode = targetHashcode;
                _sourceProperty = sourceProperty;
                _targetProperty = targetProperty;
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hash = 17;
                    hash = hash * 23 + _targetHashcode;
                    hash = hash * 23 + (_sourceProperty ?? String.Empty).GetHashCode();
                    hash = hash * 23 + (_targetProperty ?? String.Empty).GetHashCode();
                    return hash;
                }
            }

            public override bool Equals(object obj)
            {
                WeakEntry entry = (WeakEntry)obj;
                return ((this._targetHashcode == entry._targetHashcode) &&
                    (this._sourceProperty == entry._sourceProperty) && (this._targetProperty == entry._targetProperty));
            }

            public static bool operator ==(WeakEntry obj1, WeakEntry obj2)
            {
                return obj1.Equals(obj2);
            }

            public static bool operator !=(WeakEntry obj1, WeakEntry obj2)
            {
                return !(obj1 == obj2);
            }
        }

        private class WeakSource : WeakReference
        {
            private readonly Dictionary<WeakEntry, WeakBinding> _bindings = new Dictionary<WeakEntry, WeakBinding>();

            public WeakSource(object source)
                : base(source)
            {
            }

            public T SetBindng<T>(object target, string sourceProperty, string targetProperty) where T : WeakBinding
            {
                WeakEntry entry = new WeakEntry(target.GetHashCode(), sourceProperty, targetProperty);
                WeakBinding binding;
                if (_bindings.ContainsKey(entry))
                {
                    binding = _bindings[entry];
                    if (typeof(T) != binding.GetType())
                    {
                        throw new NotSupportedException(string.Format("The new {0} is not compatible with the existing {1}, ", typeof(T), binding.GetType()) +
                                                        "please clear the binding and rebind or update WeakSource.SetBinding method to clear the existing binding first");
                    }
                }
                else
                {
                    binding = EmitEngine.CreateInstance<T>(Target, target, sourceProperty, targetProperty);
                    _bindings.Add(entry, binding);
                }

                return binding as T;
            }

            public void ClearBinding(object target, string sourceProperty, string targetProperty)
            {
                WeakEntry entry = new WeakEntry(target.GetHashCode(), sourceProperty, targetProperty);
                if (_bindings.ContainsKey(entry))
                {
                    _bindings[entry].Clear();
                    _bindings.Remove(entry);
                }
            }

            public void ClearBindings()
            {
                foreach (var binding in _bindings.Values)
                {
                    binding.Clear();
                }
                _bindings.Clear();
            }
        }
    }

    public class WeakBindSource : WeakReference
    {
        private readonly NotifyPropertyObservation _propertyObservation;
        public event EventHandler<SourceChangedEventArgs> SourceChanged;

        public WeakBindSource(object target, string targetProperty)
            : base(target)
        {
            if (targetProperty != null && targetProperty.Contains("."))
            {
                _propertyObservation = new NotifyPropertyObservation(this, targetProperty.LastLeft("."));

                INotifyPropertyChanged notifyProperty = target as INotifyPropertyChanged;
                if (notifyProperty == null)
                {
                    throw new ArgumentException("The recursive binding only support INotifyPropertyChanged", "target");
                }
                _propertyObservation.Source = notifyProperty;
                Property = targetProperty.Right(".");
            }
            else
            {
                Source = target;
                Property = targetProperty;
            }
        }

        private object _source;
        public object Source
        {
            get { return _source; }
            set
            {
                if (_source != value)
                {
                    SourceChangedEventArgs changedEventArgs = new SourceChangedEventArgs(_source, value);
                    _source = value;
                    OnSourceChanged(changedEventArgs);
                }
            }
        }

        public object Value
        {
            get
            {
                if (Property == null || Source == null)
                {
                    return Source;
                }
                return EmitEngine.GetProperty(Source, Property);
            }
        }

        public string Property { get; set; }

        public object GetPropertyValue(string propertyExpression)
        {
            if (propertyExpression == null)
            {
                return Source;
            }

            int length = propertyExpression.Split('.').Length;
            NotifyPropertyObservation npo = _propertyObservation;
            while (length > 1 && npo != null)
            {
                npo = npo.Child;
                length--;
            }
            return npo != null ? npo.Value : (Source != null ? EmitEngine.GetProperty(Source, Property) : null);
        }

        public void NotifyValueChanged()
        {
            if (Source != null)
            {
                SourceChangedEventArgs changedEventArgs = new SourceChangedEventArgs(null, Source);
                OnSourceChanged(changedEventArgs);
            }
        }

        public void Clear()
        {
            if (_propertyObservation != null)
            {
                _propertyObservation.Clear();
            }
        }

        private void OnSourceChanged(SourceChangedEventArgs args)
        {
            if (SourceChanged != null)
                SourceChanged(this, args);
        }

        private class NotifyPropertyObservation
        {
            private readonly WeakBindSource _weakBindSource;
            private readonly string _property;
            private readonly NotifyPropertyObservation _child;

            public NotifyPropertyObservation(WeakBindSource weakBindTarget, string property)
            {
                _weakBindSource = weakBindTarget;
                if (property.Contains("."))
                {
                    int index = property.IndexOf(".", StringComparison.Ordinal);
                    _property = property.Substring(0, index);

                    index++;
                    _child = new NotifyPropertyObservation(weakBindTarget, property.Substring(index, property.Length - index));
                }
                else
                {
                    _property = property;
                }
            }

            private INotifyPropertyChanged _source;
            public INotifyPropertyChanged Source
            {
                private get { return _source; }
                set
                {
                    if (_source != null)
                    {
                        _source.PropertyChanged -= OnValueChanged;
                    }
                    if (value != null)
                    {
                        value.PropertyChanged += OnValueChanged;
                    }
                    _source = value;

                    Update();
                }
            }

            public NotifyPropertyObservation Child { get { return _child; } }

            public object Value
            {
                get
                {
                    return Source == null ? null : EmitEngine.GetProperty(Source, _property);
                }
            }

            public void Clear()
            {
                if (_child != null)
                {
                    _child.Clear();
                }
                if (_source != null)
                {
                    _source.PropertyChanged -= OnValueChanged;
                }
            }

            void OnValueChanged(object sender, PropertyChangedEventArgs e)
            {
                if (string.Equals(e.PropertyName, _property))
                {
                    Update();
                }
            }

            private void Update()
            {
                if (_child != null)
                {
                    _child.Source = Value as INotifyPropertyChanged;
                }
                else
                {
                    _weakBindSource.Source = Value;
                }
            }
        }

        public class SourceChangedEventArgs : EventArgs
        {
            public object OldSource { get; set; }
            public object NewSource { get; set; }

            public SourceChangedEventArgs(object oldSource, object newSource)
            {
                OldSource = oldSource;
                NewSource = newSource;
            }
        }
    }

    public class WeakBindingEvent : WeakReference
    {
        private readonly WeakBinding _binding;
        private Delegate _eventAction;

        public WeakBindingEvent(WeakBinding binding)
            : base(null)
        {
            _binding = binding;
        }

        public string EventName { get; private set; }
        public object BindSource { get; private set; }
        public string SourceExpression { get; private set; }
        public bool IsAttached { get { return _eventAction != null; } }
        public BindObjectMode SourceMode { get; set; }

        public void AttachEvent(object source, string sourceExpression, string eventName, string handlerName)
        {
            if (_eventAction != null)
            {
                DetachEvent();
            }
            EventName = eventName;
            SourceExpression = sourceExpression;

            WeakBindSource ww = source as WeakBindSource;
            if (ww != null)
            {
                source = SourceExpression != null ? ww.GetPropertyValue(sourceExpression) : ww.Source;
            }
            else if (SourceExpression != null)
            {
                var properties = sourceExpression.Split('.');
                foreach (var property in properties)
                {
                    source = EmitEngine.GetProperty(source, property);
                    if (source == null)
                    {
                        break;
                    }
                }
            }

            BindSource = source;
            if (source != null)
            {
                _eventAction = AttactEvent(source, eventName, handlerName);
            }
        }

        public void DetachEvent()
        {
            if (_eventAction != null)
            {
                var eventInfo = BindSource.GetType().GetEvent(EventName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                if (eventInfo != null)
                {
                    var removeMethod = eventInfo.GetRemoveMethod(true);
                    removeMethod.Invoke(BindSource, new object[] { _eventAction });
                    _eventAction = null;
                }
            }
        }

        private Delegate AttactEvent(object instance, string eventName, string handlerName)
        {
            var eventInfo = instance.GetType().GetEvent(eventName,
                                                       BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (eventInfo == null)
            {
                throw new ArgumentException(string.Format("The event {0} does not exist in {1}", eventName, instance.GetType()));
            }

            ParameterExpression eventSender = Expression.Parameter(typeof(object), "sender");
            ParameterExpression eventArgs = Expression.Parameter(typeof(EventArgs), "e");

            var eventHandlerType = eventInfo.EventHandlerType;
            var body = Expression.Call(Expression.Constant(_binding),
                                       _binding.GetType().GetMethod(handlerName,
                                                           BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public), eventSender,
                                       eventArgs);
            var lambda = Expression.Lambda(eventHandlerType, body, eventSender, eventArgs);

            var action = lambda.Compile();

            var addMethod = eventInfo.GetAddMethod(true);
            addMethod.Invoke(instance, new object[] { action });

            return action;
        }
    }

    public abstract class WeakBinding : WeakReference
    {
        public static readonly string SourceEventHandlerName = "OnSourceEventOccured";
        public static readonly string TargetEventHandlerName = "OnTargetEventOccured";

        public WeakBindSource BindSource
        {
            get;
            private set;
        }

        public WeakBindSource BindTarget
        {
            get;
            private set;
        }

        public string SourceProperty { get; set; }
        public string TargetProperty { get; set; }

        public Type SourcePropertyType { get; set; }
        public Type TargetPropertyType { get; set; }

        public WeakBindingEvent SourceEvent { get; set; }
        public WeakBindingEvent TargetEvent { get; set; }

        public Func<object, EventArgs, bool> SourceEventFilter;
        public Func<object, EventArgs, bool> TargetEventFilter;


        protected WeakBinding(object source, object target, string sourceProperty, string targetProperty)
            : base(source)
        {
            SourceProperty = sourceProperty;
            TargetProperty = targetProperty;

            SourceEvent = new WeakBindingEvent(this);
            TargetEvent = new WeakBindingEvent(this);

            BindSource = new WeakBindSource(source, sourceProperty);
            BindSource.SourceChanged += OnBindSourceChanged;
            BindTarget = new WeakBindSource(target, targetProperty);
            BindTarget.SourceChanged += OnBindTargetChanged;

            BindSource.NotifyValueChanged();
            BindTarget.NotifyValueChanged();
        }

        private void UpdateBindEvents(BindObjectMode mode)
        {
            if (mode == BindObjectMode.Other)
            {
                return;
            }

            if (SourceEvent.IsAttached && SourceEvent.SourceMode == mode)
            {
                DetachSourceEvent();
                AttachSourceEvent(mode, SourceEvent.SourceExpression, null, SourceEvent.EventName);
            }
            if (TargetEvent.IsAttached && TargetEvent.SourceMode == mode)
            {
                DetachTargetEvent();
                AttachTargetEvent(mode, TargetEvent.SourceExpression, null, TargetEvent.EventName);
            }
        }

        protected virtual void OnBindSourceChanged(object sender, WeakBindSource.SourceChangedEventArgs e)
        {
            if (SourceProperty != null && e.NewSource != null)
            {
                SourcePropertyType = e.NewSource.GetType().GetProperty(BindSource.Property).PropertyType;
            }
            UpdateBindEvents(BindObjectMode.Source);
        }

        protected virtual void OnBindTargetChanged(object sender, WeakBindSource.SourceChangedEventArgs e)
        {
            if (TargetProperty != null && e.NewSource != null)
            {
                TargetPropertyType = e.NewSource.GetType().GetProperty(BindTarget.Property).PropertyType;
            }
            UpdateBindEvents(BindObjectMode.Target);
        }

        protected virtual bool OnSourceEventOccured(object sender, EventArgs args)
        {
            if (!IsAlive)
            {
                Clear();
                return false;
            }
            if (SourceEventFilter != null && !SourceEventFilter(sender, args))
            {
                return false;
            }
            return true;
        }
        protected virtual bool OnTargetEventOccured(object sender, EventArgs args)
        {
            if (!IsAlive)
            {
                Clear();
                return false;
            }
            if (TargetEventFilter != null && !TargetEventFilter(sender, args))
            {
                return false;
            }
            return true;
        }

        public WeakBinding AttachSourceEvent(BindObjectMode objectMode, string propertyExpression, object target, string eventName)
        {
            if (objectMode == BindObjectMode.Source)
            {
                SourceEvent.AttachEvent(BindSource, propertyExpression, eventName, SourceEventHandlerName);
            }
            else if (objectMode == BindObjectMode.Target)
            {
                SourceEvent.AttachEvent(BindTarget, propertyExpression, eventName, SourceEventHandlerName);
            }
            else
            {
                SourceEvent.AttachEvent(target, propertyExpression, eventName, SourceEventHandlerName);
            }
            SourceEvent.SourceMode = objectMode;
            return this;
        }

        public WeakBinding AttachSourceEvent(BindObjectMode objectMode, string propertyExpression, string eventName)
        {
            return AttachSourceEvent(objectMode, propertyExpression, null, eventName);
        }

        public WeakBinding AttachSourceEvent(string eventName)
        {
            return AttachSourceEvent(BindObjectMode.Source, null, null, eventName);
        }

        public WeakBinding AttachSourceEvent(string propertyExpresion, string eventName)
        {
            return AttachSourceEvent(BindObjectMode.Source, propertyExpresion, null, eventName);
        }

        public WeakBinding DetachSourceEvent()
        {
            SourceEvent.DetachEvent();
            return this;
        }

        public WeakBinding AttachTargetEvent(BindObjectMode objectMode, string propertyExpression, object target, string eventName)
        {
            if (objectMode == BindObjectMode.Source)
            {
                TargetEvent.AttachEvent(BindSource, propertyExpression, eventName, TargetEventHandlerName);
            }
            else if (objectMode == BindObjectMode.Target)
            {
                TargetEvent.AttachEvent(BindTarget, propertyExpression, eventName, TargetEventHandlerName);
            }
            else
            {
                TargetEvent.AttachEvent(target, eventName, propertyExpression, TargetEventHandlerName);
            }
            TargetEvent.SourceMode = objectMode;
            return this;
        }

        public WeakBinding AttachTargetEvent(BindObjectMode objectMode, string propertyExpression, string eventName)
        {
            return AttachTargetEvent(objectMode, propertyExpression, null, eventName);
        }

        public WeakBinding AttachTargetEvent(string propertyExpresion, string eventName)
        {
            return AttachTargetEvent(BindObjectMode.Target, propertyExpresion, null, eventName);
        }

        public WeakBinding AttachTargetEvent(string eventName)
        {
            return AttachTargetEvent(BindObjectMode.Target, null, null, eventName);
        }

        public WeakBinding DetachTargetEvent()
        {
            TargetEvent.DetachEvent();
            return this;
        }

        public virtual void Clear()
        {
            BindSource.Clear();
            BindTarget.Clear();
            DetachSourceEvent();
            DetachTargetEvent();
        }
    }

    public class WeakProperyBinding : WeakBinding
    {
        public WeakProperyBinding(object source, object target, string sourceProperty, string targetProperty)
            : base(source, target, sourceProperty, targetProperty)
        {

        }

        protected override bool OnSourceEventOccured(object sender, EventArgs args)
        {
            if (base.OnSourceEventOccured(sender, args))
            {
                if (_bindMode == BindMode.TwoWay || _bindMode == BindMode.OneWayToTarget)
                {
                    Update(false);
                }
            }
            return true;
        }

        protected override bool OnTargetEventOccured(object sender, EventArgs args)
        {
            if (base.OnTargetEventOccured(sender, args) && _bindMode != BindMode.OneWayToTarget)
            {
                Update();
            }
            return true;
        }

        protected override void OnBindSourceChanged(object sender, WeakBindSource.SourceChangedEventArgs e)
        {
            base.OnBindSourceChanged(sender, e);
            DoSourceConventions();
        }

        protected override void OnBindTargetChanged(object sender, WeakBindSource.SourceChangedEventArgs e)
        {
            base.OnBindTargetChanged(sender, e);
            DoTargetConventions();
        }

        protected virtual void DoSourceConventions()
        {
            if (SourceEvent.IsAttached)
            {
                if (_bindMode == BindMode.OneWay)
                {
                    DetachSourceEvent();
                }
            }
            else if (_bindMode != BindMode.OneWay && BindSource.Source is INotifyPropertyChanged)
            {
                SourceEventFilter = (o, args) => ((PropertyChangedEventArgs)args).PropertyName == BindSource.Property;
                AttachSourceEvent("PropertyChanged");
            }

            Update(false);
        }

        protected virtual void DoTargetConventions()
        {
            if (TargetEvent.IsAttached)
            {
                if (_bindMode == BindMode.OneWayToTarget)
                {
                    DetachTargetEvent();
                }
            }
            else if (_bindMode != BindMode.OneWayToTarget && BindTarget.Source is INotifyPropertyChanged)
            {
                TargetEventFilter = (o, args) => ((PropertyChangedEventArgs)args).PropertyName == BindTarget.Property;
                AttachTargetEvent("PropertyChanged");
            }

            Update();
        }

        private bool _isUpdating;
        public void Update(bool targetToSource = true)
        {
            if (_isUpdating)
            {
                return;
            }

            try
            {
                _isUpdating = true;

                if (targetToSource)
                {
                    TargetToSource();
                }
                else
                {
                    SourceToTarget();
                }

            }
            finally
            {
                _isUpdating = false;
            }
        }

        protected virtual void TargetToSource()
        {
            if (BindSource.Source == null || BindSource.Property == null || BindTarget.Property == null || SourcePropertyType == null)
            {
                return;
            }

            object value = BindTarget.Source != null ? EmitEngine.GetProperty(BindTarget.Source, BindTarget.Property) : EmitEngine.CreateDefaultValue(SourcePropertyType);

            if (DataConverter != null)
            {
                EmitEngine.SetProperty(BindSource.Source, BindSource.Property, DataConverter.ConvertBack(value, Parameter));
            }
            else
            {
                if (value != null && !SourcePropertyType.IsInstanceOfType(value))
                {
                    value = Convert.ChangeType(value, SourcePropertyType, CultureInfo.CurrentCulture);
                }
                EmitEngine.SetProperty(BindSource.Source, BindSource.Property, value);
            }
        }

        protected virtual void SourceToTarget()
        {
            if (BindTarget.Source == null || BindSource.Property == null || BindTarget.Property == null || TargetPropertyType == null)
            {
                return;
            }

            object value = BindSource.Source != null ? EmitEngine.GetProperty(BindSource.Source, BindSource.Property) : EmitEngine.CreateDefaultValue(TargetPropertyType);

            if (DataConverter != null)
            {
                EmitEngine.SetProperty(BindTarget.Source, BindTarget.Property, DataConverter.Convert(value, Parameter));
            }
            else
            {
                if (value != null && !TargetPropertyType.IsInstanceOfType(value))
                {
                    value = Convert.ChangeType(value, TargetPropertyType, CultureInfo.CurrentCulture);
                }
                EmitEngine.SetProperty(BindTarget.Source, BindTarget.Property, value);
            }
        }

        private BindMode _bindMode;
        public WeakProperyBinding SetMode(BindMode mode)
        {
            if (_bindMode == mode)
            {
                return this;
            }
            _bindMode = mode;

            DoSourceConventions();
            DoTargetConventions();

            return this;
        }

        public IDataConverter DataConverter { get; set; }

        public object Parameter { get; set; }
    }

    public class WeakCollectionBinding : WeakProperyBinding
    {
        public WeakCollectionBinding(object source, object target, string sourceProperty, string targetProperty)
            : base(source, target, sourceProperty, targetProperty)
        {
            UpdateCollection();
        }

        protected override void SourceToTarget()
        {
            UpdateCollection();
        }

        protected override void TargetToSource()
        {
            //Do nothing
        }

        protected override void OnBindTargetChanged(object sender, WeakBindSource.SourceChangedEventArgs e)
        {
            base.OnBindTargetChanged(sender, e);
            UpdateCollection();
        }

        protected override void DoTargetConventions()
        {
            if (!TargetEvent.IsAttached)
            {
                if (typeof(INotifyCollectionChanged).IsAssignableFrom(TargetPropertyType))
                {
                    AttachTargetEvent(TargetProperty, "CollectionChanged");
                }
                else
                {
                    AttachTargetEvent(TargetProperty.LastLeft("."), "CollectionChanged");
                }
            }

            Update();
        }

        private void UpdateCollection()
        {
            //1. Clear
            if (Hanlder != null)
            {
                Hanlder.Clear(BindSource.Value);
            }

            IList sources = BindSource.Value as IList;
            if (sources != null)
            {
                sources.Clear();
            }

            //2. Regenerate
            IList targets = BindTarget.Value as IList;
            if (sources != null && targets != null)
            {
                for (int i = 0; i < targets.Count; i++)
                {
                    var target = targets[i];
                    if (Hanlder != null)
                    {
                        Hanlder.AddItem(i, target, BindSource.Value);
                    }
                    else if (DataGenerator != null)
                    {
                        sources.Add(DataGenerator.Generate(target, Parameter));
                    }
                    else if (Generator != null)
                    {
                        sources.Add(Generator(BindSource.Value, target));
                    }
                    else
                    {
                        sources.Add(target);
                    }

                }
            }
        }

        protected override bool OnTargetEventOccured(object sender, EventArgs args)
        {
            if (!base.OnTargetEventOccured(sender, args))
            {
                return false;
            }

            NotifyCollectionChangedEventArgs notifyEventArgs = args as NotifyCollectionChangedEventArgs;
            if (notifyEventArgs == null)
            {
                throw new NotSupportedException("Currently WeakCollectionBinding only support INotifyCollectionChanged");
            }

            if (Hanlder != null)
            {
                switch (notifyEventArgs.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        Hanlder.AddItem(notifyEventArgs.NewStartingIndex, notifyEventArgs.NewItems[0], BindSource.Value);
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        Hanlder.RemoveItem(notifyEventArgs.OldStartingIndex, notifyEventArgs.OldItems[0], BindSource.Value);
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        Hanlder.Clear(BindSource.Value);
                        break;
                    case NotifyCollectionChangedAction.Replace: //todo
                    case NotifyCollectionChangedAction.Move:
                    default:
                        break;

                }
            }
            else
            {
                IList list = BindSource.Value as IList;
                if (list == null)
                {
                    throw new NotSupportedException("Currently WeakCollectionBinding only support IList or ICollectionHandler");
                }
                switch (notifyEventArgs.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        if (DataGenerator != null)
                        {
                            list.Add(DataGenerator.Generate(notifyEventArgs.NewItems[0], Parameter));
                        }
                        else if (Generator != null)
                        {
                            list.Add(Generator(BindSource.Source, notifyEventArgs.NewItems[0]));
                        }
                        else
                        {
                            list.Add(notifyEventArgs.NewItems[0]);
                        }
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        list.RemoveAt(notifyEventArgs.OldStartingIndex);
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        list.Clear();
                        break;
                    case NotifyCollectionChangedAction.Replace: //todo
                    case NotifyCollectionChangedAction.Move:
                    default:
                        break;

                }
            }
            return true;
        }

        public IDataGenerator DataGenerator { get; set; }

        public Func<object, object, object> Generator;

        private ICollectionHandler _handler;
        public ICollectionHandler Hanlder
        {
            get { return _handler; }
            set { _handler = value; UpdateCollection(); }
        }

        public object Parameter { get; set; }
    }

    public class WeakCommandBinding : WeakProperyBinding
    {
        private ICommand _command;
        private MethodInfo _canExecuteChanged;
        private readonly IList<string> _enablesProperties = new List<string>();
        private readonly IList<string> _watchProperties = new List<string>();

        public WeakCommandBinding(object source, object target, string sourceProperty, string targetProperty)
            : base(source, target, sourceProperty, targetProperty)
        {
            UpdateCommand();
        }

        protected override void SourceToTarget()
        {
            //Do nothing
        }

        protected override void TargetToSource()
        {
            UpdateCommand();
        }

        protected override bool OnSourceEventOccured(object sender, EventArgs args)
        {
            if (!base.OnSourceEventOccured(sender, args))
            {
                return false;
            }

            if (_command != null)
            {
                if (_command.CanExecute(SourceEvent.BindSource))
                {
                    _command.Execute(SourceEvent.BindSource);
                }
            }
            return true;
        }

        private void UpdateCommand()
        {
            ICommand command = null;
            if (BindTarget.Source != null)
            {
                command = EmitEngine.GetProperty(BindTarget.Source, BindTarget.Property) as ICommand;
            }

            if (_command != null)
            {
                _command.CanExecuteChanged -= CommandCanExecuteChanged;
            }

            if (command != null)
            {
                command.CanExecuteChanged += CommandCanExecuteChanged;

                _canExecuteChanged = command.GetType().GetMethod("RaiseCanExecuteChanged", BindingFlags.Instance | BindingFlags.Public);
                if (_canExecuteChanged == null)
                {
                    throw new ArgumentException("The Command does not implement RaiseCanExecuteChanged to raise CanExecuteChanged event");
                }
            }
            _command = command;
        }

        private void RaiseCanExecuteChanged()
        {
            _canExecuteChanged.Invoke(_command, null);
        }

        private void CommandCanExecuteChanged(object sender, EventArgs e)
        {
            foreach (var property in _enablesProperties)
            {
                EmitEngine.SetProperty(BindSource.Source, property, _command.CanExecute(SourceEvent.BindSource));
            }
        }

        protected override bool OnTargetEventOccured(object sender, EventArgs args)
        {
            if (!base.OnTargetEventOccured(sender, args))
            {
                return false;
            }

            PropertyChangedEventArgs e = args as PropertyChangedEventArgs;
            if (e != null && _watchProperties.Contains(e.PropertyName))
            {
                RaiseCanExecuteChanged();
            }

            return true;
        }

        public WeakCommandBinding AddEnableProperty(string propertyName)
        {
            if (!_enablesProperties.Contains(propertyName))
            {
                _enablesProperties.Add(propertyName);
            }
            return this;
        }

        public WeakCommandBinding AddEnableProperty<T>(Expression<Func<T, bool>> propertyExpression) where T : class
        {
            return AddEnableProperty(propertyExpression.GetName());
        }

        public WeakCommandBinding Watch(params string[] properties)
        {
            if (properties == null)
            {
                return this;
            }
            foreach (var property in properties)
            {
                if (!_watchProperties.Contains(property))
                {
                    _watchProperties.Add(property);
                }
            }
            RaiseCanExecuteChanged();
            return this;
        }

        public WeakCommandBinding Watch<T>(params Expression<Func<T, object>>[] propertyExpression) where T : class
        {
            if (propertyExpression == null)
            {
                return this;
            }
            return Watch(propertyExpression.Select(item => item.GetName()).ToArray());
        }

        public override void Clear()
        {
            base.Clear();
            if (_command != null)
            {
                _command.CanExecuteChanged -= CommandCanExecuteChanged;
            }
        }
    }

    public class WeakMethodBinding : WeakProperyBinding
    {
        private string _sourceExpression;
        private string _targetExpression;
        private List<BindMethodParameter> _sourceParameters = new List<BindMethodParameter>();
        private List<BindMethodParameter> _targetParameters = new List<BindMethodParameter>();

        public WeakMethodBinding(object source, object target, string sourceProperty, string targetProperty)
            : base(source, target, sourceProperty, targetProperty)
        {
        }

        private IEnumerable<object> GetMethodParameters(IEnumerable<BindMethodParameter> parameters)
        {
            if (!parameters.Any())
            {
                return null;
            }

            List<object> ps = new List<object>();
            foreach (var parameter in parameters)
            {
                object obj;
                if (parameter.Kind == BindObjectMode.Source)
                {
                    obj = parameter.Property == null ? BindSource.Source : EmitEngine.GetProperty(BindSource.Source, parameter.Property);
                }
                else
                {
                    obj = parameter.Property == null ? BindTarget.Source : EmitEngine.GetProperty(BindTarget.Source, parameter.Property);
                }
                ps.Add(obj);
            }
            return ps;
        }

        protected override void SourceToTarget()
        {
            var source = BindSource.GetPropertyValue(_sourceExpression);
            if (source == null || SourceMethod == null)
            {
                return;
            }

            Type type;
            object value = EmitEngine.InvokeMethod(source, SourceMethod, out type, GetMethodParameters(_sourceParameters));
            if (type == typeof(void))
            {
                return;
            }

            if (DataConverter != null)
            {
                EmitEngine.SetProperty(BindTarget.Source, BindTarget.Property, DataConverter.ConvertBack(value, Parameter));
            }
            else
            {
                if (value != null && !TargetPropertyType.IsInstanceOfType(value))
                {
                    value = Convert.ChangeType(value, TargetPropertyType, CultureInfo.CurrentCulture);
                }
                EmitEngine.SetProperty(BindTarget.Source, BindTarget.Property, value);
            }
        }

        protected override void TargetToSource()
        {
            var target = BindTarget.GetPropertyValue(_targetExpression);
            if (target == null || TargetMethod == null)
            {
                return;
            }

            Type type;
            object value = EmitEngine.InvokeMethod(target, TargetMethod, out type, GetMethodParameters(_targetParameters));
            if (type == typeof(void))
            {
                return;
            }

            if (DataConverter != null)
            {
                EmitEngine.SetProperty(BindSource.Source, BindSource.Property, DataConverter.ConvertBack(value, Parameter));
            }
            else
            {
                if (value != null && !SourcePropertyType.IsInstanceOfType(value))
                {
                    value = Convert.ChangeType(value, SourcePropertyType, CultureInfo.CurrentCulture);
                }
                EmitEngine.SetProperty(BindSource.Source, BindSource.Property, value);
            }
        }

        public WeakMethodBinding AttachSourceMethod(string sourceMethod, params BindMethodParameter[] parameters)
        {
            return AttachSourceMethod(null, sourceMethod, parameters);
        }

        public WeakMethodBinding AttachSourceMethod(string sourceExpression, string sourceMethod, params BindMethodParameter[] parameters)
        {
            _sourceExpression = sourceExpression;
            _sourceParameters.Clear();
            if (parameters != null)
            {
                _sourceParameters.AddRange(parameters);
            }

            SourceMethod = sourceMethod;
            Update(false);
            return this;
        }

        public WeakMethodBinding AttachTargetMethod(string targetMethod, params BindMethodParameter[] parameters)
        {
            return AttachTargetMethod(null, targetMethod, parameters);
        }

        public WeakMethodBinding AttachTargetMethod(string targetExpression, string targetMethod, params BindMethodParameter[] parameters)
        {
            _targetExpression = targetExpression;
            _targetParameters.Clear();
            if (parameters != null)
            {
                _targetParameters.AddRange(parameters);
            }

            TargetMethod = targetMethod;
            Update();
            return this;
        }

        public string SourceMethod { get; set; }
        public string TargetMethod { get; set; }
    }

    public struct BindMethodParameter
    {
        public BindObjectMode Kind;
        public string Property;

        public BindMethodParameter(BindObjectMode kind)
            : this(kind, null)
        {
        }

        public BindMethodParameter(BindObjectMode kind, string property)
        {
            Kind = kind;
            Property = property;
        }
    }

    public enum BindObjectMode
    {
        Source,
        Target,
        Other
    }

    public enum BindMode
    {
        OneWay,
        OneWayToTarget,
        TwoWay
    }

    public interface IDataConverter
    {
        object Convert(object value, object parameter);
        object ConvertBack(object value, object parameter);
    }

    public interface IDataGenerator
    {
        object Generate(object value, object parameter);
    }

    public interface ICollectionHandler
    {
        void AddItem(int index, object item, object source);
        void RemoveItem(int index, object item, object source);
        void Clear(object source);
    }

    public static class ExpressionExtension
    {
        public static string GetName(this Expression expression)
        {
            if (expression == null)
            {
                return null;
            }
            var lambda = (LambdaExpression)expression;

            MemberExpression memberExpression;
            if (lambda.Body is UnaryExpression)
            {
                var unaryExpression = (UnaryExpression)lambda.Body;
                memberExpression = (MemberExpression)unaryExpression.Operand;
            }
            else
            {
                memberExpression = (MemberExpression)lambda.Body;
            }

            return memberExpression.ToString().FirstRight(".");
        }
    }

    public static class StringExtension
    {
        public static string Right(this string str, string content)
        {
            if (!str.Contains(content))
            {
                return string.Empty;
            }
            int index = str.LastIndexOf(content, StringComparison.Ordinal);
            return index < str.Length - 1 ? str.Substring(index + content.Length, str.Length - index - content.Length) : string.Empty;
        }

        public static string FirstRight(this string str, string content)
        {
            if (!str.Contains(content))
            {
                return string.Empty;
            }
            int index = str.IndexOf(content, StringComparison.Ordinal);
            return index < str.Length - 1 ? str.Substring(index + content.Length, str.Length - index - content.Length) : string.Empty;
        }

        public static string LastLeft(this string str, string content)
        {
            if (!str.Contains(content))
            {
                return string.Empty;
            }
            int index = str.LastIndexOf(content, StringComparison.Ordinal);
            return str.Substring(0, index);
        }
    }
}
