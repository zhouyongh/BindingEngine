// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BindingManager.cs" company="Illusion">
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
//   Manage all the binding operations, and holds all the binding in field. 
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Illusion.Utility
{
    using System;
    using System.Linq.Expressions;

    /// <summary>
    ///     Manage all the binding operations, and holds all the binding in field. 
    /// </summary>
    public class BindingManager
    {
        #region Static Fields

        /// <summary>
        ///     The indexer left mark.
        /// </summary>
        public static string IndexerLeftMark = "[";

        /// <summary>
        ///     The indexer name.
        /// </summary>
        public static string IndexerName = "Item[]";

        /// <summary>
        ///     The indexer right mark.
        /// </summary>
        public static string IndexerRightMark = "]";

        #endregion

        #region Fields

        /// <summary>
        ///     All the weak targets.
        /// </summary>
        private readonly WeakKeyDictionary<object, WeakTarget> weakTargets = new WeakKeyDictionary<object, WeakTarget>();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Clears all bindings.
        /// </summary>
        public void ClearAllBindings()
        {
            foreach (WeakTarget weakSource in this.weakTargets.Values)
            {
                weakSource.ClearBindings();
            }

            this.weakTargets.Clear();
        }

        /// <summary>
        ///     Clears the binding.
        /// </summary>
        /// <param name="target">
        ///     The target.
        /// </param>
        public void ClearBinding(object target)
        {
            WeakTarget weakTarget = this.GetWeakTarget(target);
            weakTarget.ClearBindings();

            this.weakTargets.Remove(weakTarget.GetHashCode());
        }

        /// <summary>
        ///     Clears the binding.
        /// </summary>
        /// <typeparam name="TTarget">
        ///     The type of the target.
        /// </typeparam>
        /// <param name="target">
        ///     The target.
        /// </param>
        /// <param name="targetExpression">
        ///     The target expression.
        /// </param>
        public void ClearBinding<TTarget>(
            TTarget target,
            Expression<Func<TTarget, object>> targetExpression)
        {
            this.ClearBinding(target, targetExpression.GetName());
        }

        /// <summary>
        ///     Clears the binding.
        /// </summary>
        /// <param name="target">
        ///     The target.
        /// </param>
        /// <param name="targetProperty">
        ///     The target property.
        /// </param>
        public void ClearBinding(object target, string targetProperty)
        {
            if (!this.weakTargets.ContainsKey(target))
            {
                return;
            }

            var weakSource = this.weakTargets[target];
            weakSource.ClearBindings(targetProperty);
        }

        /// <summary>
        ///     Clears the binding.
        /// </summary>
        /// <typeparam name="TTarget">
        ///     The type of the target.
        /// </typeparam>
        /// <typeparam name="TSource">
        ///     The type of the source.
        /// </typeparam>
        /// <param name="target">
        ///     The target.
        /// </param>
        /// <param name="targetExpression">
        ///     The target expression.
        /// </param>
        /// <param name="source">
        ///     The source.
        /// </param>
        /// <param name="sourceExpression">
        ///     The source property.
        /// </param>
        public void ClearBinding<TTarget, TSource>(
            TTarget target,
            Expression<Func<TTarget, object>> targetExpression,
            TSource source,
            Expression<Func<TSource, object>> sourceExpression)
        {
            this.ClearBinding(target, targetExpression.GetName(), source, sourceExpression.GetName());
        }

        /// <summary>
        ///     Clears the binding.
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
        public void ClearBinding(object target, string targetProperty, object source, string sourceProperty)
        {
            WeakTarget weakTarget = this.GetWeakTarget(target);
            weakTarget.ClearBinding(targetProperty, source, sourceProperty);
        }

        /// <summary>
        ///     Gets the binding.
        /// </summary>
        /// <typeparam name="TTarget">
        ///     The type of the target.
        /// </typeparam>
        /// <typeparam name="TSource">
        ///     The type of the source.
        /// </typeparam>
        /// <typeparam name="T">
        ///     The concrete <see cref="WeakBinding"/> to return.
        /// </typeparam>
        /// <param name="target">
        ///     The target.
        /// </param>
        /// <param name="targetExpression">
        ///     The target expression.
        /// </param>
        /// <param name="source">
        ///     The source.
        /// </param>
        /// <param name="sourceExpression">
        ///     The source expression.
        /// </param>
        /// <param name="activate">
        ///     if set to <c>true</c> activate.
        /// </param>
        /// <returns>
        ///     The <see cref="T"/>.
        /// </returns>
        public T GetBinding<TTarget, TSource, T>(
            TTarget target,
            Expression<Func<TTarget, object>> targetExpression,
            TSource source,
            Expression<Func<TSource, object>> sourceExpression,
            bool activate = true) where T : WeakBinding
        {
            return this.GetBinding<T>(target, targetExpression.GetName(), source, sourceExpression.GetName(), activate);
        }

        /// <summary>
        ///     Gets the binding.
        /// </summary>
        /// <typeparam name="T">
        ///     The concrete <see cref="WeakBinding"/> to return.
        /// </typeparam>
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
        /// <param name="activate">
        ///     if set to <c>true</c> activate.
        /// </param>
        /// <returns>
        ///     The <see cref="T"/>.
        /// </returns>
        public T GetBinding<T>(object target, string targetProperty, object source, string sourceProperty, bool activate = true) where T : WeakBinding
        {
            WeakTarget weakTarget = this.GetWeakTarget(target);
            return weakTarget.GetBinding<T>(targetProperty, source, sourceProperty);
        }

        /// <summary>
        ///     Sets the binding.
        /// </summary>
        /// <typeparam name="TTarget">
        ///     The type of the target.
        /// </typeparam>
        /// <typeparam name="TSource">
        ///     The type of the source.
        /// </typeparam>
        /// <typeparam name="T">
        ///     The concrete <see cref="WeakBinding"/> to return.
        /// </typeparam>
        /// <param name="target">
        ///     The target.
        /// </param>
        /// <param name="targetExpression">
        ///     The target expression.
        /// </param>
        /// <param name="source">
        ///     The source.
        /// </param>
        /// <param name="sourceExpression">
        ///     The source expression.
        /// </param>
        /// <param name="activate">
        ///     if set to <c>true</c> activate.
        /// </param>
        /// <returns>
        ///     The <see cref="T"/>.
        /// </returns>
        public T SetBinding<TTarget, TSource, T>(
            TTarget target,
            Expression<Func<TTarget, object>> targetExpression,
            TSource source,
            Expression<Func<TSource, object>> sourceExpression,
            bool activate = true) where T : WeakBinding
        {
            return this.SetBinding<T>(target, targetExpression.GetName(), source, sourceExpression.GetName(), activate);
        }

        /// <summary>
        ///     Sets the binding.
        /// </summary>
        /// <typeparam name="T">
        ///     The concrete <see cref="WeakBinding"/> to return.
        /// </typeparam>
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
        /// <param name="activate">
        ///     if set to <c>true</c> activate.
        /// </param>
        /// <returns>
        ///     The <see cref="T"/>.
        /// </returns>
        public T SetBinding<T>(object target, string targetProperty, object source, string sourceProperty, bool activate = true) where T : WeakBinding
        {
            WeakTarget weakTarget = this.GetWeakTarget(target);
            return weakTarget.SetBinding<T>(targetProperty, source, sourceProperty, activate);
        }

        /// <summary>
        ///     Sets the collection binding.
        /// </summary>
        /// <typeparam name="TTarget">
        ///     The type of the source.
        /// </typeparam>
        /// <typeparam name="TSource">
        ///     The type of the target.
        /// </typeparam>
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
        /// <param name="activate">
        ///     if set to <c>true</c> activate.
        /// </param>
        /// <returns>
        ///     The <see cref="WeakCollectionBinding"/>.
        /// </returns>
        public WeakCollectionBinding SetCollectionBinding<TTarget, TSource>(
            TTarget target,
            Expression<Func<TTarget, object>> targetProperty,
            TSource source,
            Expression<Func<TSource, object>> sourceProperty,
            bool activate = true)
        {
            return this.SetCollectionBinding(target, targetProperty.GetName(), source, sourceProperty.GetName(), activate);
        }

        /// <summary>
        ///     Sets the collection binding.
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
        /// <param name="activate">
        ///     if set to <c>true</c> activate.
        /// </param>
        /// <returns>
        ///     The <see cref="WeakCollectionBinding"/>.
        /// </returns>
        public WeakCollectionBinding SetCollectionBinding(object target, string targetProperty, object source, string sourceProperty, bool activate = true)
        {
            WeakTarget weakTarget = this.GetWeakTarget(target);
            return weakTarget.SetBinding<WeakCollectionBinding>(targetProperty, source, sourceProperty, activate);
        }

        /// <summary>
        ///     Sets the command binding.
        /// </summary>
        /// <typeparam name="TTarget">
        ///     The type of the source.
        /// </typeparam>
        /// <typeparam name="TSource">
        ///     The type of the target.
        /// </typeparam>
        /// <param name="target">
        ///     The target.
        /// </param>
        /// <param name="targetExpression">
        ///     The target expression.
        /// </param>
        /// <param name="source">
        ///     The source.
        /// </param>
        /// <param name="sourceExpression">
        ///     The source property.
        /// </param>
        /// <param name="activate">
        ///     if set to <c>true</c> activate.
        /// </param>
        /// <returns>
        ///     The <see cref="WeakCommandBinding"/>.
        /// </returns>
        public WeakCommandBinding SetCommandBinding<TTarget, TSource>(
            TTarget target,
            Expression<Func<TTarget, object>> targetExpression,
            TSource source,
            Expression<Func<TSource, object>> sourceExpression,
            bool activate = true)
        {
            return this.SetCommandBinding(target, targetExpression.GetName(), source, sourceExpression.GetName(), activate);
        }

        /// <summary>
        ///     Sets the command binding.
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
        /// <param name="activate">
        ///     if set to <c>true</c> activate.
        /// </param>
        /// <returns>
        ///     The <see cref="WeakCommandBinding"/>.
        /// </returns>
        public WeakCommandBinding SetCommandBinding(object target, string targetProperty, object source, string sourceProperty, bool activate = true)
        {
            WeakTarget weakTarget = this.GetWeakTarget(target);
            return weakTarget.SetBinding<WeakCommandBinding>(targetProperty, source, sourceProperty, activate);
        }

        /// <summary>
        ///     Sets the method binding.
        /// </summary>
        /// <typeparam name="TTarget">
        ///     The type of the target.
        /// </typeparam>
        /// <typeparam name="TSource">
        ///     The type of the source.
        /// </typeparam>
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
        /// <param name="activate">
        ///     if set to <c>true</c> activate.
        /// </param>
        /// <returns>
        ///     The <see cref="WeakMethodBinding"/>.
        /// </returns>
        public WeakMethodBinding SetMethodBinding<TTarget, TSource>(
            TTarget target,
            Expression<Func<TTarget, object>> targetProperty,
            TSource source,
            Expression<Func<TSource, object>> sourceProperty,
            bool activate = true)
        {
            return this.SetMethodBinding(target, targetProperty.GetName(), source, sourceProperty.GetName());
        }

        /// <summary>
        ///     Sets the method binding.
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
        /// <param name="activate">
        ///     if set to <c>true</c> activate.
        /// </param>
        /// <returns>
        ///     The <see cref="WeakMethodBinding"/>.
        /// </returns>
        public WeakMethodBinding SetMethodBinding(object target, string targetProperty, object source, string sourceProperty, bool activate = true)
        {
            WeakTarget weakTarget = this.GetWeakTarget(target);
            return weakTarget.SetBinding<WeakMethodBinding>(targetProperty, source, sourceProperty, activate);
        }

        /// <summary>
        ///     Sets the notify binding.
        /// </summary>
        /// <typeparam name="TTarget">
        ///     The type of the source.
        /// </typeparam>
        /// <typeparam name="TSource">
        ///     The type of the target.
        /// </typeparam>
        /// <param name="target">
        ///     The target.
        /// </param>
        /// <param name="targetExpression">
        ///     The target expression.
        /// </param>
        /// <param name="source">
        ///     The source.
        /// </param>
        /// <param name="sourceExpression">
        ///     The source expression.
        /// </param>
        /// <param name="activate">
        ///     The activate.
        /// </param>
        /// <returns>
        ///     The <see cref="WeakNotifyBinding"/>.
        /// </returns>
        public WeakNotifyBinding SetNotifyBinding<TTarget, TSource>(
            TTarget target,
            Expression<Func<TTarget, object>> targetExpression,
            TSource source,
            Expression<Func<TSource, object>> sourceExpression,
            bool activate = true)
        {
            return this.SetNotifyBinding(target, targetExpression.GetName(), source, sourceExpression.GetName(), activate);
        }

        /// <summary>
        ///     Sets the notify binding.
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
        /// <param name="activate">
        ///     The activate.
        /// </param>
        /// <returns>
        ///     The <see cref="WeakNotifyBinding"/>.
        /// </returns>
        public WeakNotifyBinding SetNotifyBinding(object target, string targetProperty, object source, string sourceProperty, bool activate = true)
        {
            WeakTarget weakTarget = this.GetWeakTarget(target);
            return weakTarget.SetBinding<WeakNotifyBinding>(targetProperty, source, sourceProperty, activate);
        }

        /// <summary>
        ///     Sets the property binding.
        /// </summary>
        /// <typeparam name="TTarget">
        ///     The type of the target.
        /// </typeparam>
        /// <typeparam name="TSource">
        ///     The type of the source.
        /// </typeparam>
        /// <param name="target">
        ///     The target.
        /// </param>
        /// <param name="targetExpression">
        ///     The target expression.
        /// </param>
        /// <param name="source">
        ///     The source.
        /// </param>
        /// <param name="sourceExpression">
        ///     The source property.
        /// </param>
        /// <param name="activate">
        ///     The activate.
        /// </param>
        /// <returns>
        ///     The <see cref="WeakPropertyBinding"/>.
        /// </returns>
        public WeakPropertyBinding SetPropertyBinding<TTarget, TSource>(
            TTarget target,
            Expression<Func<TTarget, object>> targetExpression,
            TSource source,
            Expression<Func<TSource, object>> sourceExpression,
            bool activate = true)
        {
            return this.SetPropertyBinding(target, targetExpression.GetName(), source, sourceExpression.GetName(), activate);
        }

        /// <summary>
        ///     Sets the property binding.
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
        /// <param name="activate">
        ///     The activate.
        /// </param>
        /// <returns>
        ///     The <see cref="WeakPropertyBinding"/>.
        /// </returns>
        public WeakPropertyBinding SetPropertyBinding(object target, string targetProperty, object source, string sourceProperty, bool activate = true)
        {
            WeakTarget weakTarget = this.GetWeakTarget(target);
            return weakTarget.SetBinding<WeakPropertyBinding>(targetProperty, source, sourceProperty, activate);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Gets the <see cref="WeakTarget"/>.
        /// </summary>
        /// <param name="target">
        ///     The target.
        /// </param>
        /// <returns>
        ///     The <see cref="WeakTarget"/>.
        /// </returns>
        private WeakTarget GetWeakTarget(object target)
        {
            if (this.weakTargets.ContainsKey(target))
            {
                return this.weakTargets[target];
            }

            var weakTarget = new WeakTarget(target);
            this.weakTargets.Add(target, weakTarget);
            return weakTarget;
        }

        #endregion
    }
}