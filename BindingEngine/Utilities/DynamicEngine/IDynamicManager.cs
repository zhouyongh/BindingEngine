// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDynamicManager.cs" company="Illusion">
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
//   Defines the IDynamicManager type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Illusion.Utility
{
    using System;
    using System.Reflection;

    /// <summary>
    ///     Denotes the manager to dynamic create and access object.
    /// </summary>
    public interface IDynamicManager
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Gets the <see cref="MethodInfo"/> from the specific method.
        /// </summary>
        /// <param name="type">
        ///     The type.
        /// </param>
        /// <param name="methodName">
        ///     Name of the method.
        /// </param>
        /// <param name="parameters">
        ///     The parameters.
        /// </param>
        /// <param name="throwException">
        ///     if set to <c>true</c> [throw exception].
        /// </param>
        /// <returns>
        ///     The <see cref="MethodInfo"/>.
        /// </returns>
        MethodInfo GetMethodInfo(Type type, string methodName, object[] parameters, bool throwException);

        /// <summary>
        ///     Creates the instance.
        /// </summary>
        /// <param name="type">
        ///     The type.
        /// </param>
        /// <param name="parameters">
        ///     The parameters.
        /// </param>
        /// <returns>
        ///     The <see cref="object"/>.
        /// </returns>
        object CreateInstance(Type type, params object[] parameters);

        /// <summary>
        ///     Creates the instance.
        /// </summary>
        /// <typeparam name="T">
        ///     The created instance.
        /// </typeparam>
        /// <param name="parameters">
        ///     The parameters.
        /// </param>
        /// <returns>
        ///     The <see cref="T"/>.
        /// </returns>
        T CreateInstance<T>(params object[] parameters);

        /// <summary>
        ///     Gets the field value.
        /// </summary>
        /// <param name="instance">
        ///     The instance.
        /// </param>
        /// <param name="fieldName">
        ///     Name of the field.
        /// </param>
        /// <returns>
        ///     The <see cref="object"/>.
        /// </returns>
        object GetField(object instance, string fieldName);

        /// <summary>
        ///     Gets the field value.
        /// </summary>
        /// <param name="instance">
        ///     The instance.
        /// </param>
        /// <param name="field">
        ///     The field.
        /// </param>
        /// <returns>
        ///     The <see cref="object"/>.
        /// </returns>
        object GetField(object instance, FieldInfo field);

        /// <summary>
        ///     Sets the field value.
        /// </summary>
        /// <param name="instance">
        ///     The instance.
        /// </param>
        /// <param name="fieldName">
        ///     Name of the field.
        /// </param>
        /// <param name="value">
        ///     The value.
        /// </param>
        void SetField(object instance, string fieldName, object value);

        /// <summary>
        ///     Gets the property value.
        /// </summary>
        /// <param name="instance">
        ///     The instance.
        /// </param>
        /// <param name="propertyName">
        ///     Name of the property.
        /// </param>
        /// <returns>
        ///     The <see cref="object"/>.
        /// </returns>
        object GetProperty(object instance, string propertyName);

        /// <summary>
        ///     Sets the property value.
        /// </summary>
        /// <param name="instance">
        ///     The instance.
        /// </param>
        /// <param name="propertyName">
        ///     Name of the property.
        /// </param>
        /// <param name="value">
        ///     The value.
        /// </param>
        void SetProperty(object instance, string propertyName, object value);

        /// <summary>
        ///     Gets the index property value.
        /// </summary>
        /// <param name="instance">
        ///     The instance.
        /// </param>
        /// <param name="index">
        ///     The index.
        /// </param>
        /// <returns>
        ///     The <see cref="object"/>.
        /// </returns>
        object GetIndexProperty(object instance, object index);

        /// <summary>
        ///     Sets the index property value.
        /// </summary>
        /// <param name="instance">
        ///     The instance.
        /// </param>
        /// <param name="index">
        ///     The index.
        /// </param>
        /// <param name="value">
        ///     The value.
        /// </param>
        void SetIndexProperty(object instance, object index, object value);

        /// <summary>
        ///     Invokes the method with the specific parameters.
        /// </summary>
        /// <param name="instance">
        ///     The instance.
        /// </param>
        /// <param name="method">
        ///     The method
        /// </param>
        /// <param name="parameters">
        ///     The parameters.
        /// </param>
        /// <returns>
        ///     The <see cref="object"/>.
        /// </returns>
        object InvokeMethod(object instance, string method, object[] parameters);

        /// <summary>
        ///     Invokes the method with the specific parameters.
        /// </summary>
        /// <param name="instance">
        ///     The instance.
        /// </param>
        /// <param name="method">
        ///     The method
        /// </param>
        /// <param name="parameters">
        ///     The parameters.
        /// </param>
        /// <returns>
        ///     The <see cref="object"/>.
        /// </returns>
        object InvokeMethod(object instance, MethodInfo method, object[] parameters);

        /// <summary>
        ///     Invokes the method with the specific parameters.
        /// </summary>
        /// <param name="type">
        ///     The type.
        /// </param>
        /// <param name="method">
        ///     The method
        /// </param>
        /// <param name="parameters">
        ///     The parameters.
        /// </param>
        /// <returns>
        ///     The <see cref="object"/>.
        /// </returns>
        object InvokeMethod(Type type, string method, object[] parameters);

        /// <summary>
        ///     Raises the event.
        /// </summary>
        /// <param name="instance">
        ///     The instance.
        /// </param>
        /// <param name="eventName">
        ///     Name of the event.
        /// </param>
        /// <param name="args">
        ///     The args.
        /// </param>
        void RaiseEvent(object instance, string eventName, EventArgs args);

        /// <summary>
        ///     Clears all cache dynamic methods.
        /// </summary>
        void ClearCache();

        /// <summary>
        ///     Registers the type of the extension.
        /// </summary>
        /// <param name="type">
        ///     The type.
        /// </param>
        /// <param name="extensionType">
        ///     Type of the extension.
        /// </param>
        void RegisterExtensionType(Type type, Type extensionType);

        /// <summary>
        ///     UnRegister the extension type.
        /// </summary>
        /// <param name="type">
        ///     The type.
        /// </param>
        /// <param name="extensionType">
        ///     Type of the extension.
        /// </param>
        void UnRegisterExtensionType(Type type, Type extensionType);

        #endregion
    }
}