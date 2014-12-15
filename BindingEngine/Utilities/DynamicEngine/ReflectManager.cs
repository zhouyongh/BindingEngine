// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReflectManager.cs" company="Illusion">
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
//   Defines the ReflectManager type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Illusion.Utility
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    ///     A concrete <see cref="DynamicManagerBase"/> used to dynamically create and access objects, 
    /// It uses <see cref="System.Reflection" /> to generate the access methods.
    /// </summary>
    /// <remarks>
    ///     reference from Manuel Abadia:
    ///     http://www.manuelabadia.com/blog/PermaLink,guid,dc72b235-1381-4c91-8706-e36216f49b94.aspx
    /// </remarks>
    /// <example>
    ///     public class TestObject
    ///     {
    ///         public string Name { get; set; }
    ///         public void UpdateName(string name)
    ///         {
    ///             Name = name;
    ///         }
    ///     }
    ///     1.Access Property
    ///     TestObject testObj = new TestObject();
    ///     testObj.Name = "1"; &lt;-- equals to:  --&gt; DynamicEngine.SetProperty(testObj, "Name", "1");
    ///     2.CreateInstance
    ///     TestObject testObj = new TestObject();   &lt;-- equals to:  --&gt; TestObject testObj =
    ///     DynamicEngine.CreateInstance&lt;TestObject&gt;();
    ///     3.Invoke Method
    ///     TestObject testObj = new TestObject();
    ///     testObj.UpdateName("new"); &lt;-- equals to:  --&gt; DynamicEngine.InvokeMethod(testObj, "UpdateName", "new");
    /// </example>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
    public class ReflectManager : DynamicManagerBase
    {
        #region Fields

        /// <summary>
        ///     The event invokers.
        /// </summary>
        protected Dictionary<string, MethodInfo> eventMethodInfos =
            new Dictionary<string, MethodInfo>();

        #endregion

        #region Public Methods and Operators

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
        public override object CreateInstance(Type type, params object[] parameters)
        {
            if (parameters != null && parameters.Any())
            {
                return Activator.CreateInstance(type, parameters);
            }

            return Activator.CreateInstance(type);
        }

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
        public override T CreateInstance<T>(params object[] parameters)
        {
            Type type = typeof(T);
            return (T)this.CreateInstance(type, parameters);
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
        public override object GetField(object instance, string fieldName)
        {
            FieldInfo fieldInfo = this.GetFieldInfo(instance, fieldName);
            return fieldInfo.GetValue(instance);
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
        public override object GetField(object instance, FieldInfo field)
        {
            return field.GetValue(instance);
        }

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
        public override void SetField(object instance, string fieldName, object value)
        {
            FieldInfo fieldInfo = this.GetFieldInfo(instance, fieldName);
            fieldInfo.SetValue(instance, value);
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
        public override object GetProperty(object instance, string propertyName)
        {
            PropertyInfo propertyInfo = this.GetPropertyInfo(instance, propertyName);
            if (!propertyInfo.CanRead)
            {
                throw new MemberAccessException("The property " + propertyName + " has no accessible getter.");
            }

            return propertyInfo.GetGetMethod(true).Invoke(instance, null);
        }

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
        public override void SetProperty(object instance, string propertyName, object value)
        {
            PropertyInfo propertyInfo = this.GetPropertyInfo(instance, propertyName);
            if (!propertyInfo.CanWrite)
            {
                throw new MemberAccessException("The property " + propertyName + " has no accessible setter.");
            }

            propertyInfo.GetSetMethod(true).Invoke(instance, new[] { value });
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
        ///     The <see cref="object"/>.
        /// </returns>
        public override object GetIndexProperty(object instance, object index)
        {
            return this.InvokeMethod(instance, "get_Item", new[] { index });
        }

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
        public override void SetIndexProperty(object instance, object index, object value)
        {
            this.InvokeMethod(instance, "set_Item", new[] { index, value });
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
        ///     The <see cref="object"/>.
        /// </returns>
        public override object InvokeMethod(object instance, MethodInfo method, object[] parameters)
        {
            var type = instance.GetType();
            if (method.DeclaringType == null || !method.DeclaringType.IsConvertableFrom(type))
            {
                // Extension type, add this instance to the first parameter.
                parameters = parameters != null ? new[] { instance }.Concat(parameters).ToArray() : new[] { instance };
            }

            return method.Invoke(instance, parameters);
        }

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
        public override object InvokeMethod(Type type, string method, object[] parameters)
        {
            MethodInfo methodInfo = this.GetMethodInfo(type, method, parameters);
            return methodInfo.Invoke(null, parameters);
        }

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
        public override object InvokeMethod(object instance, string methodName, object[] parameters)
        {
            var methodInfo = this.GetMethodInfo(instance.GetType(), methodName, parameters);
            return this.InvokeMethod(instance, methodInfo, parameters);
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
        ///     The args.
        /// </param>
        public override void RaiseEvent(object instance, string eventName, EventArgs args)
        {
            var methodParameters = new[] { instance, args };

            FieldInfo fieldInfo = this.GetFieldInfo(
                instance, 
                eventName, 
                BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            object field = this.GetField(instance, fieldInfo);
            if (field != null)
            {
                // If no observer register the event, the event field will be null.
                this.GetEventMethodInfo(instance.GetType(), eventName).Invoke(field, methodParameters);
            }
        }

        /// <summary>
        ///     Clear the cache.
        /// </summary>
        public override void ClearCache()
        {
            base.ClearCache();

            eventMethodInfos.Clear();
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Gets the <see cref="MethodInfo"/> from the specific event name.
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
        private MethodInfo GetEventMethodInfo(Type type, string eventName)
        {
            string key = (type.FullName + "_" + eventName).Replace(".", "_");
            if (!this.eventMethodInfos.ContainsKey(key))
            {
                var eventInfo = this.GetEventInfo(type, eventName);

                MethodInfo methodInfo = eventInfo.EventHandlerType.GetMethod("Invoke");

                this.eventMethodInfos.Add(key, methodInfo);
            }

            return this.eventMethodInfos[key];
        }

        #endregion
    }
}