// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicEngine.cs" company="Illusion">
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
//   A dynamic emit engine used to dynamically create and access objects.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Illusion.Utility
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;

    /// <summary>
    ///     A dynamic engine used to dynamically create and access objects, it actually the wrapper class for <see cref="IDynamicManager"/> to provide quick access.
    /// </summary>
    public static class DynamicEngine
    {
        #region Static Fields

        /// <summary>
        ///     The <see cref="IDynamicManager"/>.
        /// </summary>
        private static IDynamicManager dynamicManager = new ReflectManager();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Set the binding manager, the default value is EmitManager.
        /// </summary>
        /// <param name="manager">
        ///     The <see cref="IDynamicManager"/>.
        /// </param>
        public static void SetBindingManager(IDynamicManager manager)
        {
            dynamicManager = manager;
        }

        /// <summary>
        ///     Gets the method information with the specific parameters.
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
        ///     if set to <c>true</c> throw exception.
        /// </param>
        /// <returns>
        ///     The <see cref="MethodInfo"/>.
        /// </returns>
        public static MethodInfo GetMethodInfo(
            Type type,
            string methodName,
            object[] parameters,
            bool throwException = true)
        {
            return dynamicManager.GetMethodInfo(type, methodName, parameters, throwException);
        }

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
        public static object CreateInstance(Type type, params object[] parameters)
        {
            return dynamicManager.CreateInstance(type, parameters);
        }

        /// <summary>
        ///     Creates the instance.
        /// </summary>
        /// <typeparam name="T">
        ///     Instance type.
        /// </typeparam>
        /// <param name="parameters">
        ///     The parameters.
        /// </param>
        /// <returns>
        ///     The <see cref="T"/>.
        /// </returns>
        public static T CreateInstance<T>(params object[] parameters)
        {
            return dynamicManager.CreateInstance<T>(parameters);
        }

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
        public static object GetField(object instance, string fieldName)
        {
            return dynamicManager.GetField(instance, fieldName);
        }

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
        public static object GetField(object instance, FieldInfo field)
        {
            return dynamicManager.GetField(instance, field);
        }

        /// <summary>
        ///     Sets the field.
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
        public static void SetField(object instance, string fieldName, object value)
        {
            dynamicManager.SetField(instance, fieldName, value);
        }

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
        public static object GetProperty(object instance, string propertyName)
        {
            return dynamicManager.GetProperty(instance, propertyName);
        }

        /// <summary>
        ///     Sets the property.
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
        public static void SetProperty(object instance, string propertyName, object value)
        {
            dynamicManager.SetProperty(instance, propertyName, value);
        }

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
        /// The <see cref="object"/>.
        /// </returns>
        public static object GetIndexProperty(object instance, object index)
        {
            return dynamicManager.GetIndexProperty(instance, index);
        }

        /// <summary>
        ///     Sets the index property.
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
        public static void SetIndexProperty(object instance, object index, object value)
        {
            dynamicManager.SetIndexProperty(instance, index, value);
        }

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
        /// The <see cref="object"/>.
        /// </returns>
        public static object InvokeMethod(object instance, MethodInfo method, object[] parameters)
        {
            return dynamicManager.InvokeMethod(instance, method, parameters);
        }

        /// <summary>
        ///     Invokes the method with the specific parameters.
        /// </summary>
        /// <param name="instance">
        ///     The instance.
        /// </param>
        /// <param name="methodName">
        ///     Name of the method.
        /// </param>
        /// <param name="parameters">
        ///     The parameters.
        /// </param>
        /// <returns>
        ///     The <see cref="object"/>.
        /// </returns>
        public static object InvokeMethod(object instance, string methodName, object[] parameters)
        {
            return dynamicManager.InvokeMethod(instance, methodName, parameters);
        }

        /// <summary>
        ///     Invokes the method with the specific parameters.
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
        /// <returns>
        ///     The <see cref="object"/>.
        /// </returns>
        public static object InvokeMethod(Type type, string methodName, object[] parameters)
        {
            return dynamicManager.InvokeMethod(type, methodName, parameters);
        }

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
        ///     The <see cref="EventArgs"/> instance containing the event data.
        /// </param>
        public static void RaiseEvent(object instance, string eventName, EventArgs args)
        {
            dynamicManager.RaiseEvent(instance, eventName, args);
        }

        /// <summary>
        ///     Clears all cache dynamic methods.
        /// </summary>
        public static void ClearCache()
        {
            dynamicManager.ClearCache();
        }

        /// <summary>
        ///     Registers the type of the extension.
        /// </summary>
        /// <param name="type">
        ///     The type.
        /// </param>
        /// <param name="extensionType">
        ///     Type of the extension.
        /// </param>
        public static void RegisterExtensionType(Type type, Type extensionType)
        {
            dynamicManager.RegisterExtensionType(type, extensionType);
        }

        /// <summary>
        ///     UnRegister the extension type.
        /// </summary>
        /// <param name="type">
        ///     The type.
        /// </param>
        /// <param name="extensionType">
        ///     Type of the extension.
        /// </param>
        public static void UnRegisterExtensionType(Type type, Type extensionType)
        {
            dynamicManager.UnRegisterExtensionType(type, extensionType);
        }

        #endregion
    }
}