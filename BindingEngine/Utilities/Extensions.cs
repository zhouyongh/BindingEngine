// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Extensions.cs" company="Illusion">
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
//   The expression extension.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Illusion.Utility
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    /// <summary>
    ///     The expression extension.
    /// </summary>
    public static class ExpressionExtension
    {
        #region Constants

        /// <summary>
        ///     The index mark.
        /// </summary>
        public const string IndexMark = "[";

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetName(this Expression expression)
        {
            if (expression == null)
            {
                return null;
            }

            return GetName(expression, null).FirstRight(".");
        }

        #endregion

        #region Methods

        /// <summary>
        /// The get name.
        /// </summary>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <param name="formatter">
        /// The formatter.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string GetName(Expression expression, string formatter)
        {
            if (expression == null)
            {
                return null;
            }

            string result = null;

            if (expression is LambdaExpression)
            {
                result = GetName(((LambdaExpression)expression).Body, null);
            }
            else if (expression is UnaryExpression)
            {
                result = GetName(((UnaryExpression)expression).Operand, null);
            }
            else if (expression is MethodCallExpression)
            {
                var methodCallExpression = (MethodCallExpression)expression;
                if (methodCallExpression.Arguments.Count == 1)
                {
                    // Indexer
                    result = string.Format(
                        "{0}[{1}]",
                        GetName(methodCallExpression.Object, null),
                        GetName(methodCallExpression.Arguments[0], null));
                }
                else
                {
                    result = string.Format(
                        "{0}.{1}",
                        GetName(methodCallExpression.Object, null),
                        GetName(methodCallExpression.Arguments.Last(), null));
                }
            }
            else if (expression is ConstantExpression)
            {
                object value = ((ConstantExpression)expression).Value;
                if (value is MethodInfo)
                {
                    result = ((MethodInfo)value).Name;
                }
                else
                {
                    result = value.ToString();
                }
            }
            else if (expression is MemberExpression)
            {
                var memberExpression = (MemberExpression)expression;
                if (memberExpression.ToString().StartsWith("value("))
                {
                    // Need to compile
                    var compileValue = Expression.Lambda(memberExpression).Compile().DynamicInvoke();
                    if (compileValue != null)
                    {
                        result = compileValue.ToString();
                    }

                    result = string.Format("{0}.{1}", result ?? "item", memberExpression.Member.Name);
                }
                else
                {
                    result = GetName(memberExpression.Expression, "{0}." + memberExpression.Member.Name);
                }
            }
            else
            {
                result = expression.ToString();
            }

            if (formatter != null)
            {
                return string.Format(formatter, result);
            }

            return result;
        }

        #endregion
    }

    /// <summary>
    ///     The string extension.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public static class StringExtension
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The first right.
        /// </summary>
        /// <param name="str">
        ///     The string.
        /// </param>
        /// <param name="content">
        ///     The content.
        /// </param>
        /// <returns>
        ///     The <see cref="string"/>.
        /// </returns>
        public static string FirstRight(this string str, string content)
        {
            if (str == null)
            {
                return null;
            }

            if (!str.Contains(content))
            {
                return string.Empty;
            }

            int index = str.IndexOf(content, StringComparison.Ordinal);
            return index < str.Length - 1
                       ? str.Substring(index + content.Length, str.Length - index - content.Length)
                       : string.Empty;
        }

        /// <summary>
        /// The last left.
        /// </summary>
        /// <param name="str">
        /// The string.
        /// </param>
        /// <param name="content">
        /// The content.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string LastLeft(this string str, string content)
        {
            if (str == null)
            {
                return null;
            }

            if (!str.Contains(content))
            {
                return string.Empty;
            }

            int index = str.LastIndexOf(content, StringComparison.Ordinal);
            return str.Substring(0, index);
        }

        /// <summary>
        /// The right.
        /// </summary>
        /// <param name="str">
        /// The string.
        /// </param>
        /// <param name="content">
        /// The content.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string Right(this string str, string content)
        {
            if (str == null)
            {
                return null;
            }

            if (!str.Contains(content))
            {
                return string.Empty;
            }

            int index = str.LastIndexOf(content, StringComparison.Ordinal);
            return index < str.Length - 1
                       ? str.Substring(index + content.Length, str.Length - index - content.Length)
                       : string.Empty;
        }

        #endregion
    }

    /// <summary>
    ///     The object extension.
    /// </summary>
    public static class ObjectExtension
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The convert to.
        /// </summary>
        /// <param name="value">
        ///     The value.
        /// </param>
        /// <param name="type">
        ///     The type.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public static object ConvertTo(this object value, Type type)
        {
            if (type == null)
            {
                return value;
            }

            if (value == null || ReferenceEquals(value, string.Empty))
            {
                return type.DefaultValue();
            }

            if (type.IsInstanceOfType(value))
            {
                return value;
            }

            if (type == typeof(string))
            {
                return value.ToString();
            }

            Type t = Nullable.GetUnderlyingType(type) ?? type;
            return Convert.ChangeType(value, t, null);
        }

        /// <summary>
        ///     The extend to string method without exception.
        /// </summary>
        /// <param name="value">
        ///     The value.
        /// </param>
        /// <returns>
        ///     The <see cref="string"/>.
        /// </returns>
        public static string ToStringWithoutException(this object value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            return value.ToString();
        }

        #endregion
    }

    /// <summary>
    ///     The type extension.
    /// </summary>
    public static class TypeExtension
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The default value.
        /// </summary>
        /// <param name="type">
        ///     The type.
        /// </param>
        /// <returns>
        ///     The <see cref="object"/>.
        /// </returns>
        public static object DefaultValue(this Type type)
        {
            if (type == null)
            {
                return null;
            }

            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }

        /// <summary>
        ///     Determines whether the specific type is this type or baseClass or interface assign from.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="subType">The subType.</param>
        /// <returns>
        ///     <c>true</c> if it is; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsConvertableFrom(this Type type, Type subType)
        {
            return type == subType || subType.IsSubclassOf(type) || type.IsAssignableFrom(subType);
        }

        /// <summary>
        ///     Determines whether the specific type is convert able from the value.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        ///     <c>true</c> if it is; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsConvertableFrom(this Type type, object value)
        {
            try
            {
                Convert.ChangeType(value, type, null);
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        ///     Determines whether the specified type contains event.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="eventName">Name of the event.</param>
        /// <returns><c>true</c> if the specified type contains event; otherwise, <c>false</c>.</returns>
        public static bool ContainsEvent(this Type type, string eventName)
        {
            if (type == null)
            {
                return false;
            }

            return type.GetEvent(eventName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public) != null;
        }

        #endregion
    }

    /// <summary>
    ///     The weak reference extension.
    /// </summary>
    public static class WeakReferenceExtension
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Gets the target.
        /// </summary>
        /// <typeparam name="T">
        ///     The target to return.
        /// </typeparam>
        /// <param name="reference">
        ///     The reference.
        /// </param>
        /// <returns>
        ///     The <see cref="T"/>.
        /// </returns>
        public static T GetTarget<T>(this WeakReference reference) where T : class
        {
            if (reference == null)
            {
                return null;
            }

            return reference.Target as T;
        }

        /// <summary>
        ///     Determines whether the specified reference is not null and alive.
        /// </summary>
        /// <param name="reference">
        ///     The reference.
        /// </param>
        /// <returns>
        ///     <c>true</c> if [is not null and alive] [the specified reference]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNotNullAndAlive(this WeakReference reference)
        {
            return reference != null && reference.IsAlive;
        }

        #endregion
    }

    /// <summary>
    ///     The bind context extension.
    /// </summary>
    public static class BindContextExtension
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Tries the get value.
        /// </summary>
        /// <param name="context">
        ///     The source.
        /// </param>
        /// <returns>
        ///     The <see cref="Object"/>.
        /// </returns>
        public static object TryGetValue(this BindContext context)
        {
            return context == null ? null : context.Value;
        }

        #endregion
    }
}