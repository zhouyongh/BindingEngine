//--------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicManagerBase.cs" company="Illusion">
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
//   Base dynamic manager that support creating and accessing the object dynamically.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Illusion.Utility
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    ///     Base dynamic manager that support creating and accessing the object dynamically.
    /// </summary>
    public abstract class DynamicManagerBase : IDynamicManager
    {
        /// <summary>
        ///     The extension types.
        /// </summary>
        protected Dictionary<Type, IList<Type>> extensionTypes =
            new Dictionary<Type, IList<Type>>();

        /// <summary>
        ///     The instance creators.
        /// </summary>
        protected Dictionary<string, Delegate> instanceCreators =
            new Dictionary<string, Delegate>();

        /// <summary>
        ///     The fields.
        /// </summary>
        protected Dictionary<string, FieldInfo> fields = new Dictionary<string, FieldInfo>();

        /// <summary>
        ///     The properties.
        /// </summary>
        protected Dictionary<string, PropertyInfo> properties =
            new Dictionary<string, PropertyInfo>();

        /// <summary>
        ///     The method invokers.
        /// </summary>
        protected Dictionary<string, MethodInfo> methodInfos =
            new Dictionary<string, MethodInfo>();

        /// <summary>
        ///     The event info.
        /// </summary>
        protected Dictionary<string, EventInfo> eventInfos =
            new Dictionary<string, EventInfo>();

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
        ///     if set to <c>true</c> throw exception.
        /// </param>
        /// <returns>
        ///     The <see cref="MethodInfo"/>.
        /// </returns>
        /// <exception cref="MissingMemberException">
        ///     There is no accessible  + methodName +  event found in  +
        ///     type.FullName
        /// </exception>
        public virtual MethodInfo GetMethodInfo(
            Type type,
            string methodName,
            object[] parameters,
            bool throwException = true)
        {
            parameters = parameters ?? new object[0];

            string key = (type.FullName + "_" + methodName + "_" + parameters.Length).Replace(".", "_");
            if (!this.methodInfos.ContainsKey(key))
            {
                var methodInfo = GetTypeMethodInfo(type, methodName, parameters, false);

                if (methodInfo == null && this.extensionTypes.ContainsKey(type))
                {
                    foreach (var extensionType in this.extensionTypes[type])
                    {
                        methodInfo = GetTypeMethodInfo(extensionType, methodName, parameters, true);
                        if (methodInfo != null)
                        {
                            break;
                        }
                    }
                }

                if (methodInfo == null && throwException)
                {
                    throw new MissingMemberException(
                        "There is no accessible " + methodName + " method found in " + type.FullName);
                }

                this.methodInfos.Add(key, methodInfo);
                return methodInfo;
            }

            return this.methodInfos[key];
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
        public abstract object CreateInstance(Type type, params object[] parameters);

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
        public abstract T CreateInstance<T>(params object[] parameters);

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
        public abstract object GetField(object instance, string fieldName);

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
        public abstract object GetField(object instance, FieldInfo field);

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
        public abstract void SetField(object instance, string fieldName, object value);

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
        public abstract object GetProperty(object instance, string propertyName);

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
        public abstract void SetProperty(object instance, string propertyName, object value);

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
        public abstract object GetIndexProperty(object instance, object index);

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
        public abstract void SetIndexProperty(object instance, object index, object value);

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
        public abstract object InvokeMethod(object instance, MethodInfo method, object[] parameters);

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
        public abstract object InvokeMethod(Type type, string method, object[] parameters);

        /// <summary>
        ///     Invokes the method  with the specific parameters.
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
        public abstract object InvokeMethod(object instance, string methodName, object[] parameters);

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
        public abstract void RaiseEvent(object instance, string eventName, EventArgs args);

        /// <summary>
        ///     Clears all cache info.
        /// </summary>
        public virtual void ClearCache()
        {
            this.fields.Clear();
            this.properties.Clear();
            this.instanceCreators.Clear();
            this.methodInfos.Clear();
            this.eventInfos.Clear();
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
        public void RegisterExtensionType(Type type, Type extensionType)
        {
            if (type == extensionType)
            {
                return;
            }

            if (this.extensionTypes.ContainsKey(type))
            {
                IList<Type> list = this.extensionTypes[type];
                if (!list.Contains(extensionType))
                {
                    list.Add(extensionType);
                }
            }
            else
            {
                var types = new List<Type> { extensionType };
                this.extensionTypes.Add(type, types);
            }
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
        public void UnRegisterExtensionType(Type type, Type extensionType)
        {
            if (this.extensionTypes.ContainsKey(type))
            {
                IList<Type> list = this.extensionTypes[type];
                if (list.Contains(extensionType))
                {
                    list.Remove(extensionType);
                }

                if (list.Count == 0)
                {
                    this.extensionTypes.Remove(type);
                }

                var keys = this.methodInfos.Where(keypair => keypair.Value.DeclaringType == extensionType)
                    .Select(item => item.Key)
                    .ToArray();

                foreach (var key in keys)
                {
                    this.methodInfos.Remove(key);
                }
            }
        }

        /// <summary>
        ///     Gets the <see cref="EventInfo"/> from the specific event name.
        /// </summary>
        /// <param name="type">
        ///     The type.
        /// </param>
        /// <param name="eventName">
        ///     Name of the event.
        /// </param>
        /// <returns>
        ///     The <see cref="MethodInfo"/>.
        /// </returns>
        /// <exception cref="System.MissingMemberException">
        ///     There is no accessible  + eventName +  event found in  +
        ///     type.FullName
        /// </exception>
        protected EventInfo GetEventInfo(Type type, string eventName)
        {
            string key = (type.FullName + "_" + eventName).Replace(".", "_");
            if (!this.eventInfos.ContainsKey(key))
            {
                EventInfo eventInfo = type.GetEvent(eventName);
                if (eventInfo == null)
                {
                    throw new MissingMemberException(
                        "There is no accessible " + eventName + " event found in " + type.FullName);
                }

                this.eventInfos.Add(key, eventInfo);
            }

            return this.eventInfos[key];
        }

        /// <summary>
        ///     Gets the <see cref="FieldInfo"/>.
        /// </summary>
        /// <param name="instance">
        ///     The instance.
        /// </param>
        /// <param name="fieldName">
        ///     The field name.
        /// </param>
        /// <param name="flags">
        ///     The flags.
        /// </param>
        /// <returns>
        ///     The <see cref="FieldInfo"/>.
        /// </returns>
        /// <exception cref="MissingMemberException">
        ///     Can not find the field.
        /// </exception>
        protected FieldInfo GetFieldInfo(
            object instance, 
            string fieldName,
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        {
            Type t = instance.GetType();
            string key = (t.FullName + "_" + fieldName).Replace(".", "_");
            if (!this.fields.ContainsKey(key))
            {
                FieldInfo fieldInfo = t.GetField(fieldName, flags);
                if (fieldInfo == null)
                {
                    throw new MissingMemberException(
                        "There is no publicly accessible " + fieldName + " field found in " + t.FullName);
                }

                this.fields.Add(key, fieldInfo);
                return fieldInfo;
            }

            return this.fields[key];
        }

        /// <summary>
        ///     Gets the property getter method.
        /// </summary>
        /// <param name="instance">
        ///     The instance.
        /// </param>
        /// <param name="propertyName">
        ///     Name of the property.
        /// </param>
        /// <returns>
        ///     The <see cref="PropertyInfo"/>.
        /// </returns>
        protected PropertyInfo GetPropertyInfo(object instance, string propertyName)
        {
            Type t = instance.GetType();
            return this.GetPropertyInfo(propertyName, t);
        }

        /// <summary>
        ///     Get the match <see cref="MethodInfo"/> if exists.
        /// </summary>
        /// <param name="type">
        ///     The type.
        /// </param>
        /// <param name="methodName">
        ///     The method name.
        /// </param>
        /// <param name="parameters">
        ///     The parameters.
        /// </param>
        /// <param name="isExtensionMethod">
        ///     Whether the method is extension method, the extension method has one more parameter reference to this.
        /// </param>
        /// <returns>
        ///     The <see cref="MethodInfo"/>.
        /// </returns>
        private static MethodInfo GetTypeMethodInfo(Type type, string methodName, object[] parameters, bool isExtensionMethod)
        {
            var extensionIndex = isExtensionMethod ? 1 : 0;

            var methods =
                type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(
                        item =>
                        (item.Name.Equals(methodName) || item.Name.Right(".").Equals(methodName))
                        && item.GetParameters().Length == parameters.Length + extensionIndex)
                    .ToArray();

            MethodInfo methodInfo = null;

            var types = parameters.Select(parameter => parameter != null ? parameter.GetType() : typeof(object)).ToArray();

            if (methods.Length > 0)
            {
                foreach (var info in methods)
                {
                    if (CheckMethodParameters(types, info, isExtensionMethod))
                    {
                        methodInfo = info;
                        break;
                    }
                }
            }

            return methodInfo;
        }

        /// <summary>
        ///     Check whether the specific parameter types is equals to the <see cref="MethodInfo"/> parameter types.
        /// </summary>
        /// <param name="parameterTypes">
        ///     The specific parameters.
        /// </param>
        /// <param name="methodInfo">
        ///     The compare <see cref="MethodInfo"/>.
        /// </param>
        /// <param name="isExtensionMethod">
        ///     Specify whether the <param name="methodInfo"/> is extension method.
        /// </param>
        /// <returns>
        ///     The result.
        /// </returns>
        private static bool CheckMethodParameters(Type[] parameterTypes, MethodInfo methodInfo, bool isExtensionMethod)
        {
            var paras = methodInfo.GetParameters().Select(item => item.ParameterType).ToArray();
            var extensionIndex = isExtensionMethod ? 1 : 0;

            for (int index = 0; index < parameterTypes.Length; index++)
            {
                var parameterType = parameterTypes[index];
                var paraType = paras[index + extensionIndex];

                if (paraType.IsByRef)
                {
                    parameterType = parameterType.MakeByRefType();
                }

                if (parameterType != paraType && !paraType.IsAssignableFrom(parameterType))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        ///     Gets the property getter method.
        /// </summary>
        /// <param name="propertyName">
        ///     Name of the property.
        /// </param>
        /// <param name="t">
        ///     The t.
        /// </param>
        /// <returns>
        ///     The <see cref="PropertyInfo"/>.
        /// </returns>
        /// <exception cref="System.ArgumentException">
        ///     There is no publicly accessible  + propertyName +  property found in  +
        ///     t.FullName
        /// </exception>
        private PropertyInfo GetPropertyInfo(string propertyName, Type t)
        {
            string key = (t.FullName + "_" + propertyName).Replace(".", "_");
            if (!this.properties.ContainsKey(key))
            {
                PropertyInfo propertyInfo = t.GetProperty(
                    propertyName, 
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                if (propertyInfo == null)
                {
                    throw new MissingMemberException(
                        "There is no publicly accessible " + propertyName + " property found in " + t.FullName);
                }

                this.properties.Add(key, propertyInfo);
            }

            return this.properties[key];
        }
    }
}