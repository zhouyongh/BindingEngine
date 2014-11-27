// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WeakCollectionBinding.cs" company="Illusion">
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
//   The concrete <see cref="WeakBinding"/> that provides the binding for collection.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Illusion.Utility
{
    using System;
    using System.Collections;

    /// <summary>
    ///     The concrete <see cref="WeakBinding"/> that provides the binding for collection.
    /// </summary>
    public class WeakCollectionBinding : WeakPropertyBinding
    {
        #region Static Fields

        /// <summary>
        ///     The name of collection changed event.
        /// </summary>
        public static readonly string CollectionChangedEventName = "CollectionChanged";

        /// <summary>
        ///     Get the WeakCollectionChangedEventArgs.
        /// </summary>
        public static Func<EventArgs, WeakCollectionChangedEventArgs> GetCollectionChangedEventArgs = o =>
        {
            var propertyInfo = o.GetType().GetProperty("Action");
            if (propertyInfo == null)
            {
                return null;
            }

            var arg = new WeakCollectionChangedEventArgs();
            DynamicEngine.SetProperty(arg, "Action", Enum.Parse(typeof(WeakCollectionAction), DynamicEngine.GetProperty(o, "Action").ToString(), true));
            DynamicEngine.SetProperty(arg, "NewItems", DynamicEngine.GetProperty(o, "NewItems"));
            DynamicEngine.SetProperty(arg, "OldItems", DynamicEngine.GetProperty(o, "OldItems"));
            DynamicEngine.SetProperty(arg, "NewStartingIndex", DynamicEngine.GetProperty(o, "NewStartingIndex"));
            DynamicEngine.SetProperty(arg, "OldStartingIndex", DynamicEngine.GetProperty(o, "OldStartingIndex"));
            return arg;
        };

        #endregion

        #region Fields

        /// <summary>
        ///     The flag indicates the collection is changing.
        /// </summary>
        private bool isCollectionChanging;

        /// <summary>
        ///     The target data generator
        /// </summary>
        private IDataGenerator targetDataGenerator;

        /// <summary>
        ///     The source data generator.
        /// </summary>
        private IDataGenerator sourceDataGenerator;

        /// <summary>
        ///     The target generator
        /// </summary>
        private Func<object, object, object> targetGenerator;

        /// <summary>
        ///     The source generator.
        /// </summary>
        private Func<object, object, object> sourceGenerator;

        /// <summary>
        ///     The target handler
        /// </summary>
        private ICollectionHandler targetHandler;

        /// <summary>
        ///     The source handler
        /// </summary>
        private ICollectionHandler sourceHandler;

        /// <summary>
        ///     The target data parameter
        /// </summary>
        private object targetDataParameter;

        /// <summary>
        ///     The source data parameter
        /// </summary>
        private object sourceDataParameter;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="WeakCollectionBinding"/> class.
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
        public WeakCollectionBinding(object target, string targetProperty, object source, string sourceProperty)
            : base(target, targetProperty, source, sourceProperty)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the target data generator.
        /// </summary>
        /// <value>The target data generator.</value>
        public IDataGenerator TargetDataGenerator
        {
            get
            {
                return this.targetDataGenerator;
            }

            set
            {
                this.targetDataGenerator = value;
                this.Refresh();
            }
        }

        /// <summary>
        ///     Gets or sets the source data generator.
        /// </summary>
        /// <value>The source data generator.</value>
        public IDataGenerator SourceDataGenerator
        {
            get
            {
                return this.sourceDataGenerator;
            }

            set
            {
                this.sourceDataGenerator = value;
                this.Refresh();
            }
        }

        /// <summary>
        ///     Gets or sets the target data parameter.
        /// </summary>
        /// <value>The target data parameter.</value>
        public object TargetDataParameter
        {
            get
            {
                return this.targetDataParameter;
            }

            set
            {
                this.targetDataParameter = value;
                this.Refresh();
            }
        }

        /// <summary>
        ///     Gets or sets the source data parameter.
        /// </summary>
        /// <value>The source data parameter.</value>
        public object SourceDataParameter
        {
            get
            {
                return this.sourceDataParameter;
            }

            set
            {
                this.sourceDataParameter = value;
                this.Refresh();
            }
        }

        /// <summary>
        ///     Gets or sets the target generator.
        /// </summary>
        /// <value>The target generator.</value>
        public Func<object, object, object> TargetGenerator
        {
            get
            {
                return this.targetGenerator;
            }

            set
            {
                this.targetGenerator = value;
                this.Refresh();
            }
        }

        /// <summary>
        ///     Gets or sets the source generator.
        /// </summary>
        /// <value>The source generator.</value>
        public Func<object, object, object> SourceGenerator
        {
            get
            {
                return this.sourceGenerator;
            }

            set
            {
                this.sourceGenerator = value;
                this.Refresh();
            }
        }

        /// <summary>
        ///     Gets or sets the target handler.
        /// </summary>
        /// <value>The target handler.</value>
        public ICollectionHandler TargetHandler
        {
            get
            {
                return this.targetHandler;
            }

            set
            {
                this.targetHandler = value;
                this.Refresh();
            }
        }

        /// <summary>
        ///     Gets or sets the source handler.
        /// </summary>
        /// <value>The source handler.</value>
        public ICollectionHandler SourceHandler
        {
            get
            {
                return this.sourceHandler;
            }

            set
            {
                this.sourceHandler = value;
                this.Refresh();
            }
        }

        #endregion

        #region Public Methods and Operators

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

        /// <summary>
        ///     Sets the target collection handler.
        /// </summary>
        /// <param name="handler">
        ///     The handler.
        /// </param>
        /// <returns>
        ///     The <see cref="WeakCollectionBinding"/>.
        /// </returns>
        public WeakCollectionBinding SetTargetCollectionHandler(ICollectionHandler handler)
        {
            this.TargetHandler = handler;
            return this;
        }

        /// <summary>
        ///     Sets the target data generator.
        /// </summary>
        /// <param name="generator">
        ///     The generator.
        /// </param>
        /// <returns>
        ///     The <see cref="WeakCollectionBinding"/>.
        /// </returns>
        public WeakCollectionBinding SetTargetDataGenerator(Func<object, object, object> generator)
        {
            this.TargetGenerator = generator;
            return this;
        }

        /// <summary>
        ///     Sets the target data generator.
        /// </summary>
        /// <param name="generator">
        ///     The generator.
        /// </param>
        /// <returns>
        ///     The <see cref="WeakCollectionBinding"/>.
        /// </returns>
        public WeakCollectionBinding SetTargetDataGenerator(IDataGenerator generator)
        {
            this.TargetDataGenerator = generator;
            return this;
        }

        /// <summary>
        ///     Sets the target data parameter.
        /// </summary>
        /// <param name="parameter">
        ///     The parameter.
        /// </param>
        /// <returns>
        ///     The <see cref="WeakCollectionBinding"/>.
        /// </returns>
        public WeakCollectionBinding SetTargetDataParameter(object parameter)
        {
            this.TargetDataParameter = parameter;
            return this;
        }

        /// <summary>
        ///     Sets the source collection handler.
        /// </summary>
        /// <param name="handler">
        ///     The handler.
        /// </param>
        /// <returns>
        ///     The <see cref="WeakCollectionBinding"/>.
        /// </returns>
        public WeakCollectionBinding SetSourceCollectionHandler(ICollectionHandler handler)
        {
            this.SourceHandler = handler;
            return this;
        }

        /// <summary>
        ///     Sets the source data generator.
        /// </summary>
        /// <param name="generator">
        ///     The generator.
        /// </param>
        /// <returns>
        ///     The <see cref="WeakCollectionBinding"/>.
        /// </returns>
        public WeakCollectionBinding SetSourceDataGenerator(Func<object, object, object> generator)
        {
            this.SourceGenerator = generator;
            return this;
        }

        /// <summary>
        ///     Sets the source data generator.
        /// </summary>
        /// <param name="generator">
        ///     The generator.
        /// </param>
        /// <returns>
        ///     The <see cref="WeakCollectionBinding"/>.
        /// </returns>
        public WeakCollectionBinding SetSourceDataGenerator(IDataGenerator generator)
        {
            this.SourceDataGenerator = generator;
            return this;
        }

        /// <summary>
        ///     Sets the source data parameter.
        /// </summary>
        /// <param name="parameter">
        ///     The parameter.
        /// </param>
        /// <returns>
        ///     The <see cref="WeakCollectionBinding"/>.
        /// </returns>
        public WeakCollectionBinding SetSourceDataParameter(object parameter)
        {
            this.SourceDataParameter = parameter;
            return this;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Does the conventions.
        /// </summary>
        protected override void DoConventions()
        {
            this.AddConvention(BindMode.OneWay, Utility.BindTarget.Source, CollectionChangedEventName);
            this.AddConvention(BindMode.OneWayToSource, Utility.BindTarget.Target, CollectionChangedEventName);
        }

        /// <summary>
        ///     Sources to target.
        /// </summary>
        protected override void SourceToTarget()
        {
            this.UpdateCollection();
        }

        /// <summary>
        ///     Targets to source.
        /// </summary>
        protected override void TargetToSource()
        {
            this.UpdateCollection(false);
        }

        /// <summary>
        ///     Updates the source value.
        /// </summary>
        /// <param name="args">
        ///     The <see cref="EventArgs"/> instance containing the event data.
        /// </param>
        protected override void UpdateSourceValue(EventArgs args)
        {
            var e = GetCollectionChangedEventArgs(args);
            if (e == null)
            {
                base.UpdateSourceValue(args);
            }
            else if (this.IsBindMode(BindMode.OneWayToSource))
            {
                this.HandleCollectionChanged(e, false);
            }
        }

        /// <summary>
        ///     Updates the target value.
        /// </summary>
        /// <param name="args">
        ///     The <see cref="EventArgs"/> instance containing the event data.
        /// </param>
        protected override void UpdateTargetValue(EventArgs args)
        {
            var e = GetCollectionChangedEventArgs(args);
            if (e == null)
            {
                base.UpdateTargetValue(args);
            }
            else if (this.IsBindMode(BindMode.OneWay))
            {
                this.HandleCollectionChanged(e);
            }
        }

        /// <summary>
        ///     Adds the item.
        /// </summary>
        /// <param name="index">
        ///     The index.
        /// </param>
        /// <param name="item">
        ///     The item.
        /// </param>
        /// <param name="sourceToTarget">
        ///     if set to <c>true</c> from source to target.
        /// </param>
        private void AddItem(int index, object item, bool sourceToTarget = true)
        {
            ICollectionHandler handler = sourceToTarget ? this.TargetHandler : this.SourceHandler;
            BindSource bindSource = sourceToTarget ? this.BindTarget : this.BindSource;
            var list = bindSource.Value as IList;

            bool handled = false;
            if (handler != null)
            {
                handled = handler.AddItem(index, item, bindSource.Source, bindSource.Value);
            }

            if (!handled && list != null)
            {
                IDataGenerator dataGenerator = sourceToTarget ? this.TargetDataGenerator : this.SourceDataGenerator;
                object parameter = sourceToTarget ? this.TargetDataParameter : this.SourceDataParameter;
                Func<object, object, object> generator = sourceToTarget ? this.TargetGenerator : this.SourceGenerator;

                if (dataGenerator != null)
                {
                    list.Insert(index, dataGenerator.Generate(item, parameter));
                }
                else if (generator != null)
                {
                    list.Insert(index, generator(item, parameter));
                }
                else
                {
                    list.Insert(index, item);
                }
            }
        }

        /// <summary>
        ///     Clears the items.
        /// </summary>
        /// <param name="sourceToTarget">
        ///     if set to <c>true</c> from source to target.
        /// </param>
        private void ClearItems(bool sourceToTarget = true)
        {
            BindSource bindSource = sourceToTarget ? this.BindTarget : this.BindSource;
            ICollectionHandler handler = sourceToTarget ? this.TargetHandler : this.SourceHandler;
            var list = bindSource.Value as IList;

            bool handled = false;
            if (handler != null)
            {
                handled = handler.Clear(bindSource.Source, bindSource.Value);
            }

            if (!handled && list != null)
            {
                list.Clear();
            }
        }

        /// <summary>
        ///     Handles the collection changed.
        /// </summary>
        /// <param name="e">
        ///     The <see cref="EventArgs"/> instance containing the event data.
        /// </param>
        /// <param name="sourceToTarget">
        ///     if set to <c>true</c> from source to target.
        /// </param>
        private void HandleCollectionChanged(EventArgs e, bool sourceToTarget = true)
        {
            if (this.isCollectionChanging)
            {
                return;
            }

            var args = GetCollectionChangedEventArgs(e);

            try
            {
                this.isCollectionChanging = true;

                switch (args.Action)
                {
                    case WeakCollectionAction.Add:
                        this.AddItem(args.NewStartingIndex, args.NewItems[0], sourceToTarget);
                        break;
                    case WeakCollectionAction.Remove:
                        this.RemoveItem(args.OldStartingIndex, args.OldItems[0], sourceToTarget);
                        break;
                    case WeakCollectionAction.Reset:
                        this.ClearItems(sourceToTarget);
                        break;
                }
            }
            finally
            {
                this.isCollectionChanging = false;
            }
        }

        /// <summary>
        ///     Removes the item.
        /// </summary>
        /// <param name="index">
        ///     The index.
        /// </param>
        /// <param name="item">
        ///     The item.
        /// </param>
        /// <param name="sourceToTarget">
        ///     if set to <c>true</c> from source to target.
        /// </param>
        private void RemoveItem(int index, object item, bool sourceToTarget = true)
        {
            bool handled = false;
            BindSource bindSource = sourceToTarget ? this.BindTarget : this.BindSource;
            ICollectionHandler handler = sourceToTarget ? this.TargetHandler : this.SourceHandler;
            var list = bindSource.Value as IList;

            if (handler != null)
            {
                handled = handler.RemoveItem(index, item, bindSource.Source, bindSource.Value);
            }

            if (!handled && list != null)
            {
                list.RemoveAt(index);
            }
        }

        /// <summary>
        ///     Updates the collection.
        /// </summary>
        /// <param name="sourceToTarget">
        ///     if set to <c>true</c> from source to target.
        /// </param>
        private void UpdateCollection(bool sourceToTarget = true)
        {
            var target = sourceToTarget ? this.BindSource : this.BindTarget;

            // 1. Clear
            this.ClearItems(sourceToTarget);

            // 2. Regenerate
            var targets = target.Value as IEnumerable;
            if (targets == null)
            {
                return;
            }

            int index = 0;
            foreach (object item in targets)
            {
                this.AddItem(index, item, sourceToTarget);
                index++;
            }
        }

        #endregion
    }

    /// <summary>
    ///     The weak collection changed event args.
    /// </summary>
    public class WeakCollectionChangedEventArgs : EventArgs
    {
        /// <summary>
        ///     Gets or sets the action.
        /// </summary>
        public WeakCollectionAction Action { get; set; }

        /// <summary>
        ///     Gets or sets the new items.
        /// </summary>
        public IList NewItems { get; set; }

        /// <summary>
        ///     Gets or sets the new starting index.
        /// </summary>
        public int NewStartingIndex { get; set; }

        /// <summary>
        ///     Gets or sets the old items.
        /// </summary>
        public IList OldItems { get; set; }

        /// <summary>
        ///     Gets or sets the old start index.
        /// </summary>
        public int OldStartingIndex { get; set; }
    }

    /// <summary>
    ///     Denotes the action of collection changed.
    /// </summary>
    public enum WeakCollectionAction
    {
        /// <summary>
        ///     Add item.
        /// </summary>
        Add,

        /// <summary>
        ///     Remove item.
        /// </summary>
        Remove,

        /// <summary>
        ///     Replace item.
        /// </summary>
        Replace,

        /// <summary>
        ///     Clear item.
        /// </summary>
        Reset
    }
}