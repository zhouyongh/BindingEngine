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

        public static WeakCommandBinding SetCommandBinding<TSource>(TSource source, Expression<Func<TSource, ICommand>> sourceExpression, object target, string eventName)
        {
            var weakSource = GetWeakSource(source);
            return weakSource.SetBindng<WeakCommandBinding>(target, sourceExpression.GetName(), null).AttachEvent(eventName) as WeakCommandBinding;
        }

        public static WeakCollectionBinding SetCollectionBinding<TSource>(TSource source, Expression<Func<TSource, object>> sourceExpression, INotifyCollectionChanged target)
        {
            return SetCollectionBinding(source, target, sourceExpression.GetName());
        }

        public static WeakCollectionBinding SetCollectionBinding(object source, INotifyCollectionChanged target, string sourceProperty)
        {
            var weakSource = GetWeakSource(source);
            var weakCollectionBinding = weakSource.SetBindng<WeakCollectionBinding>(target, sourceProperty, null);
            weakCollectionBinding.AttachEvent("CollectionChanged");
            return weakCollectionBinding;
        }

        public static WeakProperyBinding SetPropertyBinding<TSource, TTarget>(TSource source, Expression<Func<TSource, object>> sourceExpression, TTarget target,
            Expression<Func<TTarget, object>> targetExpression)
        {
            var weakSource = GetWeakSource(source);
            var propertyBinding = weakSource.SetBindng<WeakProperyBinding>(target, sourceExpression.GetName(), targetExpression.GetName());
            propertyBinding.Update();
            return propertyBinding;
        }

        public static void SetPropertyBinding(object source, string sourceProperty, INotifyPropertyChanged target, string targetProperty)
        {
            var weakSource = GetWeakSource(source);
            weakSource.SetBindng<WeakProperyBinding>(target, sourceProperty, targetProperty).AttachEvent("PropertyChanged");
        }

        public static void ClearBinding(object source, string sourceProperty, object target, string targetProperty)
        {
            var weakSource = GetWeakSource(source);
            weakSource.ClearBinding(target, sourceProperty, targetProperty);
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
            return weakSource;
        }

        #endregion

        private struct WeakEntry
        {
            private readonly Type _targetType;
            private readonly string _sourceProperty;
            private readonly string _targetProperty;

            public WeakEntry(Type targetType, string sourceProperty, string targetProperty)
            {
                _targetType = targetType;
                _sourceProperty = sourceProperty;
                _targetProperty = targetProperty;
            }

            public override int GetHashCode()
            {
                return _targetType.GetHashCode() ^ _sourceProperty.GetHashCode() ^ _targetProperty.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                WeakEntry entry = (WeakEntry)obj;
                return ((this._targetType == entry._targetType) &&
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
                WeakEntry entry = new WeakEntry(target.GetType(), sourceProperty, targetProperty);
                WeakBinding binding;
                if (_bindings.ContainsKey(entry))
                {
                    binding = _bindings[entry];
                }
                else
                {
                    binding = EmitEngine.CreateInstance<T>(Target, target, sourceProperty, targetProperty);
                }

                return binding as T;
            }

            public void ClearBinding(object target, string sourceProperty, string targetProperty)
            {
                WeakEntry entry = new WeakEntry(target.GetType(), sourceProperty, targetProperty);
                if (_bindings.ContainsKey(entry))
                {
                    _bindings[entry].DetachEvent();
                    _bindings.Remove(entry);
                }
            }

            public void ClearBindings()
            {
                foreach (var binding in _bindings.Values)
                {
                    binding.DetachEvent();
                }
                _bindings.Clear();
            }
        }

        public class WeakTarget : WeakReference
        {
            private readonly NotifyPropertyObservation _propertyObservation;
            public event EventHandler<ValueChangedEventArgs> ValueChanged;

            public WeakTarget(object target, string targetProperty)
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
                    Value = target;
                    Property = targetProperty;
                }
            }

            private object _value;
            public object Value
            {
                get { return _value; }
                set
                {
                    ValueChangedEventArgs changedEventArgs = new ValueChangedEventArgs(_value, value);
                    _value = value;
                    OnValueChanged(changedEventArgs);
                }
            }

            public string Property { get; set; }

            private void OnValueChanged(ValueChangedEventArgs args)
            {
                if (ValueChanged != null)
                    ValueChanged(this, args);
            }

            private class NotifyPropertyObservation
            {
                private readonly WeakTarget _weakTarget;
                private readonly string _property;
                private readonly NotifyPropertyObservation _child;

                public NotifyPropertyObservation(WeakTarget weakTarget, string property)
                {
                    _weakTarget = weakTarget;
                    if (property.Contains("."))
                    {
                        int index = property.IndexOf(".", StringComparison.Ordinal);
                        _property = property.Substring(0, index);

                        index++;
                        _child = new NotifyPropertyObservation(weakTarget, property.Substring(index, property.Length - index));
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

                void OnValueChanged(object sender, PropertyChangedEventArgs e)
                {
                    if (string.Equals(e.PropertyName, _property))
                    {
                        Update();
                    }
                }

                private void Update()
                {
                    object value = Source == null ? null : EmitEngine.GetProperty(Source, _property);

                    if (_child != null)
                    {
                        _child.Source = value as INotifyPropertyChanged;
                    }
                    else
                    {
                        _weakTarget.Value = value;
                    }
                }
            }

            public class ValueChangedEventArgs : EventArgs
            {
                public object OldValue { get; set; }
                public object NewValue { get; set; }

                public ValueChangedEventArgs(object oldValue, object newValue)
                {
                    OldValue = oldValue;
                    NewValue = newValue;
                }
            }
        }

        public abstract class WeakBinding : WeakReference
        {
            private object _target;
            public object Source
            {
                get { return Target; }
                set { Target = value; }
            }

            public WeakTarget BindTarget
            {
                get;
                private set;
            }

            protected WeakBinding(object source, object target, string sourceProperty, string targetProperty)
                : base(source)
            {
                BindTarget = new WeakTarget(target, targetProperty);
                BindTarget.ValueChanged += BindTargetValueChanged;
                _target = BindTarget.Value;

                TargetProperty = BindTarget.Property;
                SourceProperty = sourceProperty;
            }

            void BindTargetValueChanged(object sender, WeakTarget.ValueChangedEventArgs e)
            {
                OnBindTargetValueChanged(e.OldValue, e.NewValue);
            }

            protected virtual void OnBindTargetValueChanged(object oldValue, object newValue)
            {
                DetachEvent();
                _target = newValue;
                AttachEvent(EventName);
            }

            public WeakBinding AttachEvent(string eventName)
            {
                if (_target == null || eventName == null)
                {
                    return this;
                }
                var eventInfo = _target.GetType().GetEvent(eventName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                if (eventInfo == null)
                {
                    throw new ArgumentException(string.Format("The event {0} does not exist in {1}", eventName, _target.GetType()));
                }
                EventName = eventName;

                ParameterExpression eventSender = Expression.Parameter(typeof(object), "sender");
                ParameterExpression eventArgs = Expression.Parameter(typeof(EventArgs), "e");

                var eventHandlerType = eventInfo.EventHandlerType;
                var body = Expression.Call(Expression.Constant(this),
                    GetType().GetMethod("OnEventOccured", BindingFlags.Instance | BindingFlags.NonPublic), eventSender, eventArgs);
                var lambda = Expression.Lambda(eventHandlerType, body, eventSender, eventArgs);

                var action = lambda.Compile();

                BindingAction = action;

                var addMethod = eventInfo.GetAddMethod(true);
                addMethod.Invoke(_target, new object[] { action });

                return this;
            }

            public WeakBinding DetachEvent()
            {
                if (BindingAction != null && _target != null)
                {
                    var eventInfo = _target.GetType().GetEvent(EventName);
                    if (eventInfo != null)
                    {
                        var removeMethod = eventInfo.GetRemoveMethod(true);
                        removeMethod.Invoke(_target, new object[] { BindingAction });
                        BindingAction = null;
                    }
                }
                return this;
            }

            protected abstract void OnEventOccured(object sender, EventArgs args);

            public string SourceProperty { get; set; }
            public string TargetProperty { get; set; }

            public Delegate BindingAction { get; set; }

            public string EventName { get; set; }
        }

        public class WeakProperyBinding : WeakBinding
        {
            public WeakProperyBinding(object source, object bindTarget, string sourceProperty, string targetProperty)
                : base(source, bindTarget, sourceProperty, targetProperty)
            {
                SourcePropertyType = source.GetType().GetProperty(sourceProperty).PropertyType;

                AddINPCConvention();

                if (BindTarget.Value != null)
                {
                    TargetPropertyType = BindTarget.Value.GetType().GetProperty(TargetProperty).PropertyType;
                }
            }

            protected override void OnEventOccured(object sender, EventArgs args)
            {
                if (EventFilter != null && !EventFilter(sender, args))
                {
                    return;
                }

                Update();
            }

            protected override void OnBindTargetValueChanged(object oldValue, object newValue)
            {
                base.OnBindTargetValueChanged(oldValue, newValue);

                var inpc = oldValue as INotifyPropertyChanged;
                if (inpc != null)
                {
                    inpc.PropertyChanged -= SourcePropertyChanged;
                }

                if (newValue != null)
                {
                    TargetPropertyType = newValue.GetType().GetProperty(TargetProperty).PropertyType;
                }

                AddINPCConvention();
            }

            private void AddINPCConvention() //INPC = INotifyPorpertyChanged
            {
                if (BindingAction == null &&
                    EventName == null &&
                    BindTarget.Value is INotifyPropertyChanged)
                {
                    EventFilter = (o, args) => ((PropertyChangedEventArgs)args).PropertyName == TargetProperty;
                    AttachEvent("PropertyChanged");
                }

                Update();
            }

            void SourcePropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == SourceProperty)
                {
                    Update(false);
                }
            }

            private bool _isUpdating;
            public void Update(bool targetToSource = true)
            {
                if (_isUpdating || BindTarget.Value == null)
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

            private void TargetToSource()
            {
                object value = EmitEngine.GetProperty(BindTarget.Value, TargetProperty);

                if (DataConverter != null)
                {
                    EmitEngine.SetProperty(Source, SourceProperty, DataConverter.ConvertBack(value, Parameter));
                }
                else
                {
                    if (value != null && !SourcePropertyType.IsInstanceOfType(value))
                    {
                        value = Convert.ChangeType(value, SourcePropertyType, CultureInfo.CurrentCulture);
                    }
                    EmitEngine.SetProperty(Source, SourceProperty, value);
                }
            }

            private void SourceToTarget()
            {
                if (BindTarget.Value == null)
                {
                    return;
                }

                object value = EmitEngine.GetProperty(Source, SourceProperty);

                if (DataConverter != null)
                {
                    EmitEngine.SetProperty(BindTarget.Value, TargetProperty, DataConverter.Convert(value, Parameter));
                }
                else
                {
                    if (value != null && !TargetPropertyType.IsInstanceOfType(value))
                    {
                        value = Convert.ChangeType(value, TargetPropertyType, CultureInfo.CurrentCulture);
                    }
                    EmitEngine.SetProperty(BindTarget.Value, TargetProperty, value);
                }
            }

            private BindMode _bindMode;
            public WeakProperyBinding SetMode(BindMode mode)
            {
                if (_bindMode == mode)
                {
                    return this;
                }
                INotifyPropertyChanged inpc = Source as INotifyPropertyChanged;
                if (inpc != null)
                {
                    switch (mode)
                    {
                        case BindMode.OneWay:
                            inpc.PropertyChanged -= SourcePropertyChanged;
                            break;
                        case BindMode.TwoWay:
                            inpc.PropertyChanged += SourcePropertyChanged;
                            Update(false);
                            break;
                        default:
                            break;
                    }
                }
                _bindMode = mode;
                return this;
            }

            public IDataConverter DataConverter { get; set; }

            public Type SourcePropertyType { get; set; }
            public Type TargetPropertyType { get; set; }

            public object Parameter { get; set; }

            public Func<object, EventArgs, bool> EventFilter;

            public string SourceEvent { get; set; }
        }

        public class WeakCollectionBinding : WeakBinding
        {
            public WeakCollectionBinding(object source, object bindTarget, string sourceProperty, string targetProperty)
                : base(source, bindTarget, sourceProperty, targetProperty)
            {
            }

            protected override void OnEventOccured(object sender, EventArgs args)
            {
                NotifyCollectionChangedEventArgs notifyEventArgs = args as NotifyCollectionChangedEventArgs;
                if (notifyEventArgs == null)
                {
                    throw new NotSupportedException("Currently WeakCollectionBinding only support INotifyCollectionChanged");
                }

                IList list = EmitEngine.GetProperty(Source, SourceProperty) as IList;
                if (list == null)
                {
                    throw new NotSupportedException("Currently WeakCollectionBinding only support IList source");
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
                            list.Add(Generator(notifyEventArgs.NewItems[0]));
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
                    case NotifyCollectionChangedAction.Replace://todo
                    case NotifyCollectionChangedAction.Move:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            public IDataGenerator DataGenerator { get; set; }

            public Func<object, object> Generator;

            public object Parameter { get; set; }
        }

        public class WeakCommandBinding : WeakBinding
        {
            private readonly ICommand _command;
            private readonly IList<string> _enablesProperties = new List<string>();
            private readonly IList<string> _watchProperties = new List<string>();
            private readonly MethodInfo _canExecuteChanged;

            public WeakCommandBinding(object source, object bindTarget, string sourceProperty, string targetProperty)
                : base(source, bindTarget, sourceProperty, targetProperty)
            {
                _command = EmitEngine.GetProperty(source, sourceProperty) as ICommand;
                if (_command == null)
                {
                    throw new ArgumentException("The WeakCommandBinding only support ICommand");
                }
                _command.CanExecuteChanged += CommandCanExecuteChanged;

                var inpc = source as INotifyPropertyChanged;
                if (inpc != null)
                {
                    inpc.PropertyChanged += SourcePropertyChanged;
                }

                _canExecuteChanged = _command.GetType().GetMethod("RaiseCanExecuteChanged", BindingFlags.Instance | BindingFlags.Public);
                if (_canExecuteChanged == null)
                {
                    throw new ArgumentException("The Command does not implement RaiseCanExecuteChanged to raise CanExecuteChanged event");
                }
            }

            void SourcePropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (_watchProperties.Contains(e.PropertyName))
                {
                    RaiseCanExecuteChanged();
                }
            }

            private void RaiseCanExecuteChanged()
            {
                _canExecuteChanged.Invoke(_command, null);
            }

            private void CommandCanExecuteChanged(object sender, EventArgs e)
            {
                if (BindTarget.Value == null)
                {
                    return;
                }

                foreach (var property in _enablesProperties)
                {
                    EmitEngine.SetProperty(BindTarget.Value, property, _command.CanExecute(Source));
                }
            }

            protected override void OnEventOccured(object sender, EventArgs args)
            {
                if (BindTarget.Value == null)
                {
                    return;
                }

                if (_command.CanExecute(Source))
                {
                    _command.Execute(Source);
                }
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
        }
    }

    public enum BindMode
    {
        OneWay,
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

    public static class Extension
    {
        public static string GetName(this Expression expression)
        {
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
