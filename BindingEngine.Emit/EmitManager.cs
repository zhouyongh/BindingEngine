// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmitManager.cs" company="Illusion">
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
//   Defines the EmitManager type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Illusion.Utility
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;

    /// <summary>
    ///     Concrete <see cref="IDynamicManager" /> that use <see cref="System.Reflection.Emit" /> to create and access object.
    /// </summary>
    /// <remarks>
    ///     It hold the <see cref="System.Reflection.Emit.DynamicMethod" /> in the cache, use ClearCaches method to clear it.
    /// </remarks>
    public class EmitManager : DynamicManagerBase
    {
        #region Fields

        /// <summary>
        ///     The field getters.
        /// </summary>
        private readonly Dictionary<string, DynamicGetHandler> fieldGetters =
            new Dictionary<string, DynamicGetHandler>();

        /// <summary>
        ///     The field setters.
        /// </summary>
        private readonly Dictionary<string, DynamicSetHandler> fieldSetters =
            new Dictionary<string, DynamicSetHandler>();

        /// <summary>
        ///     The instance creators.
        /// </summary>
        private new Dictionary<string, DynamicCreateHandler> instanceCreators =
            new Dictionary<string, DynamicCreateHandler>();

        /// <summary>
        ///     The method invokers.
        /// </summary>
        private Dictionary<MethodInfo, DynamicInvokeHandler> methodInvokers =
            new Dictionary<MethodInfo, DynamicInvokeHandler>();

        /// <summary>
        ///     The property getters.
        /// </summary>
        private Dictionary<string, DynamicGetHandler> propertyGetters =
            new Dictionary<string, DynamicGetHandler>();

        /// <summary>
        ///     The property setters.
        /// </summary>
        private Dictionary<string, DynamicSetHandler> propertySetters =
            new Dictionary<string, DynamicSetHandler>();

        /// <summary>
        ///     The event invokers.
        /// </summary>
        private Dictionary<string, DynamicEventHandler> eventInvokers =
            new Dictionary<string, DynamicEventHandler>();

        #endregion

        #region Delegates

        /// <summary>
        ///     Represents the method that will get the value from the source.
        /// </summary>
        /// <param name="source">The source</param>
        /// <returns>The value</returns>
        private delegate object DynamicGetHandler(object source);

        /// <summary>
        ///     Represents the method that will set the value to the source.
        /// </summary>
        /// <param name="source">The source</param>
        /// <param name="value">The value</param>
        private delegate void DynamicSetHandler(object source, object value);

        /// <summary>
        ///     Represents the method that will create the source.
        /// </summary>
        /// <param name="parameters">The parameters</param>
        /// <returns>The value</returns>
        private delegate object DynamicCreateHandler(object[] parameters);

        /// <summary>
        ///     Represents the method that will invoke the method.
        /// </summary>
        /// <param name="target">The target</param>
        /// <param name="parameters">The parameters</param>
        /// <returns>The value</returns>
        private delegate object DynamicInvokeHandler(object target, object[] parameters);

        /// <summary>
        ///     Represents the method that will invoke the event.
        /// </summary>
        /// <param name="target">The target</param>
        /// <param name="args">The event args</param>
        private delegate void DynamicEventHandler(object target, EventArgs args);

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Clears all cache dynamic methods.
        /// </summary>
        public override void ClearCache()
        {
            base.ClearCache();

            this.fieldSetters.Clear();
            this.fieldGetters.Clear();
            this.propertyGetters.Clear();
            this.propertySetters.Clear();
            this.instanceCreators.Clear();
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
        public override object CreateInstance(Type type, params object[] parameters)
        {
            Type[] valueTypes;
            if (parameters == null)
            {
                valueTypes = new Type[0];
            }
            else
            {
                if (parameters.Any(item => item == null))
                {
                    return Activator.CreateInstance(type, parameters);
                }

                valueTypes = new Type[parameters.Length];
                for (int i = 0; i < parameters.Length; i++)
                {
                    valueTypes[i] = parameters[i].GetType();
                }
            }

            var handler = this.CreateInstanceMethod(type, valueTypes);
            if (handler != null)
            {
                return handler(parameters);
            }

            // This type is stupid Struct that does not have constructors.
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
        ///     Gets the field.
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
            return this.FieldGetterMethod(instance, fieldName)(instance);
        }

        /// <summary>
        ///     Gets the field.
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
            return this.FieldGetterMethod(instance, field)(instance);
        }

        /// <summary>
        ///     Gets the index property.
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
        ///     Gets the property.
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
            return this.PropertyGetterMethod(instance, propertyName)(instance);
        }

        /// <summary>
        ///     Invokes the method.
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

            return this.MethodInvokeMethod(type, method)(instance, parameters);
        }

        /// <summary>
        ///     Invokes the method.
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
            var methodInfo = this.GetMethodInfo(type, method, parameters);
            return this.MethodInvokeMethod(type, methodInfo)(null, parameters);
        }

        /// <summary>
        ///     Invokes the method.
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
            this.EventInvokeMethod(instance.GetType(), eventName)(instance, args);
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
        public override void SetField(object instance, string fieldName, object value)
        {
            this.FieldSetterMethod(instance, fieldName)(instance, value);
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
        public override void SetIndexProperty(object instance, object index, object value)
        {
            this.InvokeMethod(instance, "set_Item", new[] { index, value });
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
        public override void SetProperty(object instance, string propertyName, object value)
        {
            this.PropertySetterMethod(instance, propertyName)(instance, value);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Creates the instance method.
        /// </summary>
        /// <param name="type">
        ///     The type.
        /// </param>
        /// <param name="types">
        ///     The types.
        /// </param>
        /// <returns>
        ///     The <see cref="DynamicMethod"/>.
        /// </returns>
        private DynamicCreateHandler CreateInstanceMethod(Type type, Type[] types)
        {
            string typesKey = (types == null || types.Length == 0)
                                  ? string.Empty
                                  : string.Concat((IEnumerable<Type>)types);

            string key = (type.FullName + "_" + typesKey).Replace(".", "_");

            if (!this.instanceCreators.ContainsKey(key))
            {
                var dm = new DynamicMethod(
                    key,
                    typeof(object),
                    new[] { typeof(object[]) },
                    type,
                    true);
                ILGenerator il = dm.GetILGenerator();
                ConstructorInfo cons = type.GetConstructor(types);
                if (cons == null)
                {
                    return null; // The type is stupid struct and does not have constructors
                }

                ParameterInfo[] parameters = cons.GetParameters();

                il.Emit(OpCodes.Nop);
                for (int i = 0; i < parameters.Length; i++)
                {
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Ldc_I4, i);
                    il.Emit(OpCodes.Ldelem_Ref);
                    if (types != null)
                    {
                        il.Emit(
                            parameters[i].ParameterType.IsValueType ? OpCodes.Unbox_Any : OpCodes.Castclass,
                            types[i]);
                    }
                }

                il.Emit(OpCodes.Newobj, cons);
                il.Emit(OpCodes.Ret);

                this.instanceCreators.Add(key, (DynamicCreateHandler)dm.CreateDelegate(typeof(DynamicCreateHandler)));
            }

            return this.instanceCreators[key];
        }

        /// <summary>
        ///     Boxes a type if needed.
        /// </summary>
        /// <param name="ilGenerator">
        ///     The MSIL generator.
        /// </param>
        /// <param name="type">
        ///     The type.
        /// </param>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation",
            Justification = "Reviewed. Suppression is OK here.")]
        private void EmitBoxIfNeeded(ILGenerator ilGenerator, Type type)
        {
            if (type.IsValueType)
            {
                ilGenerator.Emit(OpCodes.Box, type);
            }
        }

        /// <summary>
        ///     Emits the cast to a reference, unbox if needed.
        /// </summary>
        /// <param name="ilGenerator">
        ///     The il generator.
        /// </param>
        /// <param name="type">
        ///     The type to cast.
        /// </param>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation",
            Justification = "Reviewed. Suppression is OK here.")]
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1614:ElementParameterDocumentationMustHaveText",
            Justification = "Reviewed. Suppression is OK here.")]
        private void EmitCastToReference(ILGenerator ilGenerator, Type type)
        {
            ilGenerator.Emit(type.IsValueType ? OpCodes.Unbox_Any : OpCodes.Castclass, type);
        }

        /// <summary>
        ///     Emits code to save an integer to the evaluation stack.
        /// </summary>
        /// <param name="ilGenerator">
        ///     The il generator.
        /// </param>
        /// <param name="value">
        ///     The value to push.
        /// </param>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation",
            Justification = "Reviewed. Suppression is OK here.")]
        private void EmitFastInt(ILGenerator ilGenerator, int value)
        {
            // for small integers, emit the proper opcode
            switch (value)
            {
                case -1:
                    ilGenerator.Emit(OpCodes.Ldc_I4_M1);
                    return;
                case 0:
                    ilGenerator.Emit(OpCodes.Ldc_I4_0);
                    return;
                case 1:
                    ilGenerator.Emit(OpCodes.Ldc_I4_1);
                    return;
                case 2:
                    ilGenerator.Emit(OpCodes.Ldc_I4_2);
                    return;
                case 3:
                    ilGenerator.Emit(OpCodes.Ldc_I4_3);
                    return;
                case 4:
                    ilGenerator.Emit(OpCodes.Ldc_I4_4);
                    return;
                case 5:
                    ilGenerator.Emit(OpCodes.Ldc_I4_5);
                    return;
                case 6:
                    ilGenerator.Emit(OpCodes.Ldc_I4_6);
                    return;
                case 7:
                    ilGenerator.Emit(OpCodes.Ldc_I4_7);
                    return;
                case 8:
                    ilGenerator.Emit(OpCodes.Ldc_I4_8);
                    return;
            }

            // for bigger values emit the short or long opcode
            if (value > -129 && value < 128)
            {
                ilGenerator.Emit(OpCodes.Ldc_I4_S, (sbyte)value);
            }
            else
            {
                ilGenerator.Emit(OpCodes.Ldc_I4, value);
            }
        }

        /// <summary>
        ///     Gets the event invoke method.
        /// </summary>
        /// <param name="type">
        ///     The type.
        /// </param>
        /// <param name="eventName">
        ///     The info of event.
        /// </param>
        /// <returns>
        ///     The <see cref="DynamicEventHandler"/>.
        /// </returns>
        /// <exception cref="System.MissingMemberException">
        ///     There is no accessible  + eventName +  method found in  + type.FullName
        /// </exception>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation",
            Justification = "Reviewed. Suppression is OK here.")]
        private DynamicEventHandler EventInvokeMethod(Type type, string eventName)
        {
            string key = (type.FullName + "_" + eventName).Replace(".", "_");

            if (!this.eventInvokers.ContainsKey(key))
            {
                var eventInfo = GetEventInfo(type, eventName);

                Type declaringType = eventInfo.DeclaringType ?? type;
                var dynamicMethod = new DynamicMethod(
                    "__event_invoke_" + key,
                    typeof(void),
                    new[] { typeof(object), typeof(EventArgs) },
                    declaringType.Module,
                    true);

                ILGenerator ilGenerator = dynamicMethod.GetILGenerator();
                Label label = ilGenerator.DefineLabel();
                FieldInfo eventField = type.GetField(
                    eventName,
                    BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (eventField == null)
                {
                    throw new MissingMemberException("The event does not have the background filed.");
                }

                ilGenerator.DeclareLocal(typeof(bool));

                ilGenerator.Emit(OpCodes.Nop);
                ilGenerator.Emit(OpCodes.Ldarg_0);
                ilGenerator.Emit(OpCodes.Ldfld, eventField);
                ilGenerator.Emit(OpCodes.Ldnull);
                ilGenerator.Emit(OpCodes.Ceq);
                ilGenerator.Emit(OpCodes.Stloc_0);
                ilGenerator.Emit(OpCodes.Ldloc_0);
                ilGenerator.Emit(OpCodes.Brtrue_S, label);

                ilGenerator.Emit(OpCodes.Nop);
                ilGenerator.Emit(OpCodes.Ldarg_0);
                ilGenerator.Emit(OpCodes.Ldfld, eventField);
                ilGenerator.Emit(OpCodes.Ldarg_0);
                ilGenerator.Emit(OpCodes.Ldarg_1);

                ilGenerator.Emit(OpCodes.Callvirt, eventInfo.EventHandlerType.GetMethod("Invoke"));
                ilGenerator.Emit(OpCodes.Nop);
                ilGenerator.Emit(OpCodes.Nop);

                ilGenerator.MarkLabel(label);
                ilGenerator.Emit(OpCodes.Ret);

                this.eventInvokers.Add(key, (DynamicEventHandler)dynamicMethod.CreateDelegate(typeof(DynamicEventHandler)));
            }

            return this.eventInvokers[key];
        }

        /// <summary>
        ///     Gets the field getter method.
        /// </summary>
        /// <param name="instance">
        ///     The instance.
        /// </param>
        /// <param name="fieldName">
        ///     Name of the field.
        /// </param>
        /// <returns>
        ///     The <see cref="DynamicMethod"/>.
        /// </returns>
        private DynamicGetHandler FieldGetterMethod(object instance, string fieldName)
        {
            Type t = instance.GetType();
            string key = (t.FullName + "_" + fieldName).Replace(".", "_");
            if (!this.fieldGetters.ContainsKey(key))
            {
                FieldInfo fieldInfo = t.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (fieldInfo == null)
                {
                    throw new MissingFieldException(
                        "There is no accessible " + fieldName + " field found in " + t.FullName);
                }

                return this.FieldGetterMethod(instance, fieldInfo);
            }

            return this.fieldGetters[key];
        }

        /// <summary>
        ///     Gets the field getter method.
        /// </summary>
        /// <param name="instance">
        ///     The instance.
        /// </param>
        /// <param name="fieldInfo">
        ///     The field info.
        /// </param>
        /// <returns>
        ///     The <see cref="DynamicMethod"/>.
        /// </returns>
        private DynamicGetHandler FieldGetterMethod(object instance, FieldInfo fieldInfo)
        {
            if (fieldInfo == null)
            {
                throw new ArgumentNullException("fieldInfo");
            }

            Type t = instance.GetType();
            string key = (t.FullName + "_" + fieldInfo.Name).Replace(".", "_");
            if (!this.fieldGetters.ContainsKey(key))
            {
                var getter = new DynamicMethod(
                    "__get_field_" + key,
                    typeof(object),
                    new[] { typeof(object) },
                    t,
                    true);

                ILGenerator getterIl = getter.GetILGenerator();
                getterIl.Emit(OpCodes.Ldarg_0);
                getterIl.Emit(OpCodes.Ldfld, fieldInfo);
                BoxIfNeeded(fieldInfo.FieldType, getterIl);
                getterIl.Emit(OpCodes.Ret);

                this.fieldGetters.Add(key, (DynamicGetHandler)getter.CreateDelegate(typeof(DynamicGetHandler)));
            }

            return this.fieldGetters[key];
        }

        /// <summary>
        ///     Gets the field setter method.
        /// </summary>
        /// <param name="instance">
        ///     The instance.
        /// </param>
        /// <param name="fieldName">
        ///     Name of the field.
        /// </param>
        /// <returns>
        ///     The <see cref="DynamicMethod"/>.
        /// </returns>
        private DynamicSetHandler FieldSetterMethod(object instance, string fieldName)
        {
            Type t = instance.GetType();

            string key = (t.FullName + "_" + fieldName).Replace(".", "_");
            if (!this.fieldSetters.ContainsKey(key))
            {
                var fieldInfo = t.GetField(
                    fieldName,
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (fieldInfo == null)
                {
                    throw new MissingFieldException(
                        "There is no accessible " + fieldName + " field found in " + t.FullName);
                }

                var setter = new DynamicMethod(
                    "__set_field_" + key,
                    typeof(void),
                    new[] { typeof(object), typeof(object) },
                    t,
                    true);

                ILGenerator setterIl = setter.GetILGenerator();
                setterIl.Emit(OpCodes.Ldarg_0);
                setterIl.Emit(OpCodes.Ldarg_1);
                UnboxIfNeeded(fieldInfo.FieldType, setterIl);
                setterIl.Emit(OpCodes.Stfld, fieldInfo);
                setterIl.Emit(OpCodes.Ret);

                this.fieldSetters.Add(key, (DynamicSetHandler)setter.CreateDelegate(typeof(DynamicSetHandler)));
            }

            return this.fieldSetters[key];
        }

        /// <summary>
        ///     Gets the method invoke method.
        /// </summary>
        /// <param name="type">
        ///     The type.
        /// </param>
        /// <param name="methodInfo">
        ///     The method info.
        /// </param>
        /// <returns>
        ///     The <see cref="DynamicInvokeHandler"/>.
        /// </returns>
        /// <exception cref="System.ArgumentException">
        ///     There is no accessible  + methodName +  method found in  + type.FullName
        /// </exception>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation",
            Justification = "Reviewed. Suppression is OK here.")]
        private DynamicInvokeHandler MethodInvokeMethod(Type type, MethodInfo methodInfo)
        {
            if (methodInfo == null)
            {
                throw new ArgumentNullException("methodInfo");
            }

            if (!this.methodInvokers.ContainsKey(methodInfo))
            {
                Type declaringType = methodInfo.DeclaringType ?? type;
                var dynamicMethod = new DynamicMethod(
                    "__invoke_method_" + methodInfo.Name,
                    typeof(object),
                    new[] { typeof(object), typeof(object[]) },
                    declaringType,
                    true);

                ILGenerator ilGenerator = dynamicMethod.GetILGenerator();

                ParameterInfo[] parameters = methodInfo.GetParameters();

                var paramTypes = new Type[parameters.Length];

                // copies the parameter types to an array
                for (int i = 0; i < paramTypes.Length; i++)
                {
                    if (parameters[i].ParameterType.IsByRef)
                    {
                        paramTypes[i] = parameters[i].ParameterType.GetElementType();
                    }
                    else
                    {
                        paramTypes[i] = parameters[i].ParameterType;
                    }
                }

                var locals = new LocalBuilder[paramTypes.Length];

                // generates a local variable for each parameter
                for (int i = 0; i < paramTypes.Length; i++)
                {
                    locals[i] = ilGenerator.DeclareLocal(paramTypes[i], true);
                }

                // creates code to copy the parameters to the local variables
                for (int i = 0; i < paramTypes.Length; i++)
                {
                    ilGenerator.Emit(OpCodes.Ldarg_1);
                    this.EmitFastInt(ilGenerator, i);
                    ilGenerator.Emit(OpCodes.Ldelem_Ref);
                    this.EmitCastToReference(ilGenerator, paramTypes[i]);
                    ilGenerator.Emit(OpCodes.Stloc, locals[i]);
                }

                if (!methodInfo.IsStatic)
                {
                    // loads the object into the stack
                    ilGenerator.Emit(OpCodes.Ldarg_0);
                }

                // loads the parameters copied to the local variables into the stack
                for (int i = 0; i < paramTypes.Length; i++)
                {
                    ilGenerator.Emit(parameters[i].ParameterType.IsByRef ? OpCodes.Ldloca_S : OpCodes.Ldloc, locals[i]);
                }

                // calls the method
                ilGenerator.EmitCall(methodInfo.IsStatic ? OpCodes.Call : OpCodes.Callvirt, methodInfo, null);

                // creates code for handling the return value
                if (methodInfo.ReturnType == typeof(void))
                {
                    ilGenerator.Emit(OpCodes.Ldnull);
                }
                else
                {
                    this.EmitBoxIfNeeded(ilGenerator, methodInfo.ReturnType);
                }

                // iterates through the parameters updating the parameters passed by ref
                for (int i = 0; i < paramTypes.Length; i++)
                {
                    if (parameters[i].ParameterType.IsByRef)
                    {
                        ilGenerator.Emit(OpCodes.Ldarg_1);
                        this.EmitFastInt(ilGenerator, i);
                        ilGenerator.Emit(OpCodes.Ldloc, locals[i]);
                        if (locals[i].LocalType.IsValueType)
                        {
                            ilGenerator.Emit(OpCodes.Box, locals[i].LocalType);
                        }

                        ilGenerator.Emit(OpCodes.Stelem_Ref);
                    }
                }

                // returns the value to the caller
                ilGenerator.Emit(OpCodes.Ret);

                this.methodInvokers.Add(methodInfo, (DynamicInvokeHandler)dynamicMethod.CreateDelegate(typeof(DynamicInvokeHandler)));
            }

            return this.methodInvokers[methodInfo];
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
        ///     The <see cref="DynamicMethod"/>.
        /// </returns>
        private DynamicGetHandler PropertyGetterMethod(object instance, string propertyName)
        {
            Type t = instance.GetType();
            return this.PropertyGetterMethod(propertyName, t);
        }

        /// <summary>
        ///     Properties the getter method.
        /// </summary>
        /// <param name="propertyName">
        ///     Name of the property.
        /// </param>
        /// <param name="t">
        ///     The t.
        /// </param>
        /// <returns>
        ///     The <see cref="DynamicMethod"/>.
        /// </returns>
        /// <exception cref="System.ArgumentException">
        ///     There is no publicly accessible  + propertyName +  property found in  +
        ///     t.FullName
        /// </exception>
        private DynamicGetHandler PropertyGetterMethod(string propertyName, Type t)
        {
            string key = (t.FullName + "_" + propertyName).Replace(".", "_");
            if (!this.propertyGetters.ContainsKey(key))
            {
                PropertyInfo propertyInfo = t.GetProperty(
                    propertyName,
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                if (propertyInfo == null)
                {
                    throw new MissingMemberException(
                        "There is no accessible " + propertyName + " property found in " + t.FullName);
                }

                if (!propertyInfo.CanRead)
                {
                    throw new MemberAccessException(
                        "The property " + propertyName + " has no accessible getter.");
                }

                var getter = new DynamicMethod(
                    "__get_property_" + key,
                    typeof(object),
                    new[] { typeof(object) },
                    t,
                    true);

                ILGenerator getterIl = getter.GetILGenerator();

                var getMethodInfo = propertyInfo.GetGetMethod(true);
                getterIl.Emit(OpCodes.Ldarg_0);
                getterIl.Emit(OpCodes.Callvirt, getMethodInfo);
                BoxIfNeeded(getMethodInfo.ReturnType, getterIl);
                getterIl.Emit(OpCodes.Ret);

                this.propertyGetters.Add(key, (DynamicGetHandler)getter.CreateDelegate(typeof(DynamicGetHandler)));
            }

            return this.propertyGetters[key];
        }

        /// <summary>
        ///     Gets the property setter method.
        /// </summary>
        /// <param name="instance">
        ///     The instance.
        /// </param>
        /// <param name="propertyName">
        ///     Name of the property.
        /// </param>
        /// <returns>
        ///     The <see cref="DynamicMethod"/>.
        /// </returns>
        private DynamicSetHandler PropertySetterMethod(object instance, string propertyName)
        {
            Type t = instance.GetType();
            string key = (t.FullName + "_" + propertyName).Replace(".", "_");
            if (!this.propertySetters.ContainsKey(key))
            {
                PropertyInfo propertyInfo = t.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
                if (propertyInfo == null)
                {
                    throw new MissingMemberException(
                        "There is no accessible " + propertyName + " property found in " + t.FullName);
                }

                if (!propertyInfo.CanWrite)
                {
                    throw new MemberAccessException(
                        "The property " + propertyName + " has no accessible setter.");
                }

                var setter = new DynamicMethod(
                    "__set_property_" + key,
                    typeof(void),
                    new[] { typeof(object), typeof(object) },
                    t,
                    true);

                ILGenerator setterIl = setter.GetILGenerator();

                MethodInfo setMethodInfo = propertyInfo.GetSetMethod(true);

                setterIl.Emit(OpCodes.Ldarg_0);
                setterIl.Emit(OpCodes.Ldarg_1);
                UnboxIfNeeded(setMethodInfo.GetParameters()[0].ParameterType, setterIl);
                setterIl.Emit(OpCodes.Callvirt, setMethodInfo);
                setterIl.Emit(OpCodes.Ret);

                this.propertySetters.Add(key, (DynamicSetHandler)setter.CreateDelegate(typeof(DynamicSetHandler)));
            }

            return this.propertySetters[key];
        }

        /// <summary>
        ///     Box the value if needed.
        /// </summary>
        /// <param name="type">The type</param>
        /// <param name="generator">The il generator</param>
        private void BoxIfNeeded(Type type, ILGenerator generator)
        {
            if (type.IsValueType)
            {
                generator.Emit(OpCodes.Box, type);
            }
        }

        /// <summary>
        ///     Unbox the value if needed.
        /// </summary>
        /// <param name="type">The type</param>
        /// <param name="generator">The generator</param>
        private void UnboxIfNeeded(Type type, ILGenerator generator)
        {
            if (type.IsValueType)
            {
                generator.Emit(OpCodes.Unbox_Any, type);
            }
        }

        #endregion
    }
}