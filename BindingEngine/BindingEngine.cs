// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BindingEngine.cs" company="Illusion">
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
//   The binding engine.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Illusion.Utility
{
    using System;
    using System.Linq.Expressions;

    /// <summary>
    ///     A binding engine that provide custom binding operation, it actually the wrapper class for <see cref="BindingManager"/> to provide quick operation.
    /// </summary>
    public static class BindingEngine
    {
        #region Static Fields

        /// <summary>
        ///     The binding manager.
        /// </summary>
        private static readonly BindingManager bindingManager = new BindingManager();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Clears all bindings.
        /// </summary>
        public static void ClearAllBindings()
        {
            bindingManager.ClearAllBindings();
        }

        /// <summary>
        ///     Clears the binding.
        /// </summary>
        /// <param name="target">
        ///     The target.
        /// </param>
        public static void ClearBinding(object target)
        {
            bindingManager.ClearBinding(target);
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
        public static void ClearBinding<TTarget>(
            TTarget target,
            Expression<Func<TTarget, object>> targetExpression)
        {
            bindingManager.ClearBinding(target, targetExpression);
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
        public static void ClearBinding(object target, string targetProperty)
        {
            bindingManager.ClearBinding(target, targetProperty);
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
        public static void ClearBinding<TTarget, TSource>(
            TTarget target,
            Expression<Func<TTarget, object>> targetExpression,
            TSource source,
            Expression<Func<TSource, object>> sourceExpression)
        {
            bindingManager.ClearBinding(target, targetExpression, source, sourceExpression);
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
        public static void ClearBinding(object target, string targetProperty, object source, string sourceProperty)
        {
            bindingManager.ClearBinding(target, targetProperty, source, sourceProperty);
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
        public static T SetBinding<TTarget, TSource, T>(
            TTarget target,
            Expression<Func<TTarget, object>> targetExpression,
            TSource source,
            Expression<Func<TSource, object>> sourceExpression,
            bool activate = true) where T : WeakBinding
        {
            return bindingManager.SetBinding<TTarget, TSource, T>(
                target,
                targetExpression,
                source,
                sourceExpression,
                activate);
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
        public static T SetBinding<T>(object target, string targetProperty, object source, string sourceProperty, bool activate = true) where T : WeakBinding
        {
            return bindingManager.SetBinding<T>(target, targetProperty, source, sourceProperty, activate);
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
        public static WeakCollectionBinding SetCollectionBinding<TTarget, TSource>(
            TTarget target,
            Expression<Func<TTarget, object>> targetProperty,
            TSource source,
            Expression<Func<TSource, object>> sourceProperty,
            bool activate = true)
        {
            return bindingManager.SetCollectionBinding(target, targetProperty, source, sourceProperty, activate);
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
        public static WeakCollectionBinding SetCollectionBinding(object target, string targetProperty, object source, string sourceProperty, bool activate = true)
        {
            return bindingManager.SetCollectionBinding(target, targetProperty, source, sourceProperty, activate);
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
        public static WeakCommandBinding SetCommandBinding<TTarget, TSource>(
            TTarget target,
            Expression<Func<TTarget, object>> targetExpression,
            TSource source,
            Expression<Func<TSource, object>> sourceExpression,
            bool activate = true)
        {
            return bindingManager.SetCommandBinding(target, targetExpression, source, sourceExpression, activate);
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
        public static WeakCommandBinding SetCommandBinding(object target, string targetProperty, object source, string sourceProperty, bool activate = true)
        {
            return bindingManager.SetCommandBinding(target, targetProperty, source, sourceProperty, activate);
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
        /// <param name="targetExpression">
        ///     The target property.
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
        ///     The <see cref="WeakMethodBinding"/>.
        /// </returns>
        public static WeakMethodBinding SetMethodBinding<TTarget, TSource>(
            TTarget target,
            Expression<Func<TTarget, object>> targetExpression,
            TSource source,
            Expression<Func<TSource, object>> sourceExpression,
            bool activate = true)
        {
            return bindingManager.SetMethodBinding(target, targetExpression, source, sourceExpression, activate);
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
        public static WeakMethodBinding SetMethodBinding(object target, string targetProperty, object source, string sourceProperty, bool activate = true)
        {
            return bindingManager.SetMethodBinding(target, targetProperty, source, sourceProperty, activate);
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
        public static WeakNotifyBinding SetNotifyBinding<TTarget, TSource>(
            TTarget target,
            Expression<Func<TTarget, object>> targetExpression,
            TSource source,
            Expression<Func<TSource, object>> sourceExpression,
            bool activate = true)
        {
            return bindingManager.SetNotifyBinding(target, targetExpression, source, sourceExpression);
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
        public static WeakNotifyBinding SetNotifyBinding(object target, string targetProperty, object source, string sourceProperty, bool activate = true)
        {
            return bindingManager.SetNotifyBinding(target, targetProperty, source, sourceProperty, activate);
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
        public static WeakPropertyBinding SetPropertyBinding<TTarget, TSource>(
            TTarget target,
            Expression<Func<TTarget, object>> targetExpression,
            TSource source,
            Expression<Func<TSource, object>> sourceExpression,
            bool activate = true)
        {
            return bindingManager.SetPropertyBinding(target, targetExpression, source, sourceExpression, activate);
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
        public static WeakPropertyBinding SetPropertyBinding(object target, string targetProperty, object source, string sourceProperty, bool activate = true)
        {
            return bindingManager.SetPropertyBinding(target, targetProperty, source, sourceProperty, activate);
        }

        #endregion
    }
}