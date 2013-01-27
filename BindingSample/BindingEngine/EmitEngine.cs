using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;

namespace BindingSample
{
    /// <summary>
    ///  A dynamic emit engine used to dynamically access and create objects.
    /// <remarks>
    ///  It uses <see cref="System.Reflection.Emit"/> to generate the access methods, holds all dynamic methods in its cache for next use, it's thread safe.
    /// </remarks>
    /// <example>
    ///  public class TestObject
    ///  {
    ///		public string Name { get; set; }
    ///     public void UpdateName(string name)
    ///     {
    ///         Name = name;
    ///     }
    ///	 }
    /// 
    /// 1.Access Property
    ///  TestObject testObj = new TestObject();
    ///  testObj.Name = "1"; &lt;-- equals to:  --&gt; EmitEngine.SetProperty(testObj, "Name", "1");
    ///  
    /// 2.CreateInstance
    ///  TestObject testObj = new TestObject();   &lt;-- equals to:  --&gt; TestObject testObj = EmitEngine.CreateInstance&lt;TestObject&gt;();
    /// 
    /// 3.Invoke Method
    ///  TestObject testObj = new TestObject();
    ///  testObj.UpdateName("new"); &lt;-- equals to:  --&gt; EmitEngine.InvokeMethod(testObj, "UpdateName", "new");
    /// </example>
    /// <remarks>
    /// reference from Manuel Abadia: http://www.manuelabadia.com/blog/PermaLink,guid,dc72b235-1381-4c91-8706-e36216f49b94.aspx
    /// </remarks>
    /// </summary>
    public class EmitEngine
    {
        #region Field

        private static readonly ConcurrentDictionary<string, DynamicMethod> fieldSetters = new ConcurrentDictionary<string, DynamicMethod>();
        private static readonly ConcurrentDictionary<string, DynamicMethod> fieldGetters = new ConcurrentDictionary<string, DynamicMethod>();
        private static readonly ConcurrentDictionary<string, DynamicMethod> propertyGetters = new ConcurrentDictionary<string, DynamicMethod>();
        private static readonly ConcurrentDictionary<string, DynamicMethod> propertySetter = new ConcurrentDictionary<string, DynamicMethod>();
        private static readonly ConcurrentDictionary<string, DynamicMethod> methodInvokers = new ConcurrentDictionary<string, DynamicMethod>();
        private static readonly ConcurrentDictionary<string, DynamicMethod> instanceCreators = new ConcurrentDictionary<string, DynamicMethod>();

        #endregion

        #region Public Method

        /// <summary>
        /// Gets the field.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns></returns>
        public static object GetField(object instance, string fieldName)
        {
            return FieldGetterMethod(instance, fieldName).Invoke(null, new[] { instance });
        }

        /// <summary>
        /// Gets the field.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="field">The field.</param>
        /// <returns></returns>
        public static object GetField(object instance, FieldInfo field)
        {
            return FieldGetterMethod(instance, field).Invoke(null, new[] { instance });
        }

        /// <summary>
        /// Sets the field.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="value">The value.</param>
        public static void SetField(object instance, string fieldName, object value)
        {
            FieldSetterMethod(instance, fieldName).Invoke(null, new[] { instance, value });
        }

        /// <summary>
        /// Sets the property.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        public static void SetProperty(object instance, string propertyName, object value)
        {
            PropertySetterMethod(instance, propertyName).Invoke(null, new[] { instance, value });
        }

        /// <summary>
        /// Gets the property.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public static object GetProperty(object instance, string propertyName)
        {
            return PropertyGetterMethod(instance, propertyName).Invoke(null, new[] { instance });
        }

        /// <summary>
        /// Invokes the method.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public static object InvokeMethod(object instance, string methodName, IEnumerable<object> parameters)
        {
            var methodParameters = new[] { instance, parameters != null ? parameters.ToArray() : null };
            return MethodInvokeMethod(instance, methodName).Invoke(null, methodParameters);
        }

        /// <summary>
        /// Invokes the method.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="returnType">Type of the return.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public static object InvokeMethod(object instance, string methodName, out Type returnType, IEnumerable<object> parameters)
        {
            returnType = instance.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public).ReturnType;
            return InvokeMethod(instance, methodName, parameters);
        }

        /// <summary>
        /// Clears all cache dynamic methods.
        /// </summary>
        public static void ClearCache()
        {
            fieldSetters.Clear();
            fieldGetters.Clear();
            propertyGetters.Clear();
            propertySetter.Clear();
            instanceCreators.Clear();
        }

        /// <summary>
        /// Creates the instance.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public static object CreateInstance(Type type, params object[] parameters)
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

            var method = CreateInstanceMethod(type, valueTypes);
            if (method != null)
            {
                return method.Invoke(null, new object[] { parameters });
            }
            // This type is stupid Struct that does not have constructors.
            return Activator.CreateInstance(type);
        }

        /// <summary>
        /// Creates the instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public static T CreateInstance<T>(params object[] parameters)
        {
            Type type = typeof(T);
            return (T)CreateInstance(type, parameters);
        }

        /// <summary>
        /// Gets the property delegate.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="propName">Name of the prop.</param>
        /// <returns></returns>
        public static Func<T, TResult> GetPropertyDelegate<T, TResult>(string propName)
        {
            Type type = typeof(T);
            var method = PropertyGetterMethod(type, propName);
            return (Func<T, TResult>)method.CreateDelegate(typeof(Func<T, TResult>));
        }

        /// <summary>
        /// Creates the default value.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static object CreateDefaultValue(Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the field getter method.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns></returns>
        private static DynamicMethod FieldGetterMethod(object instance, string fieldName)
        {
            var t = instance.GetType();
            var key = (t.FullName + "_" + fieldName).Replace(".", "_");
            if (!fieldGetters.ContainsKey(key))
            {
                var fieldInfo = t.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public);
                if (fieldInfo == null)
                    throw new ArgumentException("There is no publicly accessible " + fieldName + " field found in " + t.FullName);

                return FieldGetterMethod(instance, fieldInfo);
            }
            return fieldGetters[key];
        }

        /// <summary>
        /// Creates the instance method.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="ptypes">The ptypes.</param>
        /// <returns></returns>
        private static DynamicMethod CreateInstanceMethod(Type type, Type[] ptypes)
        {
            string typesKey = (ptypes == null || ptypes.Length == 0) ? string.Empty : string.Concat((IEnumerable<Type>)ptypes);
            var key = (type.FullName + "_" + typesKey).Replace(".", "_");

            if (!instanceCreators.ContainsKey(key))
            {
                DynamicMethod dm = new DynamicMethod(key, typeof(object), new Type[] { typeof(object[]) }, typeof(EmitEngine).Module, true);
                ILGenerator il = dm.GetILGenerator();
                ConstructorInfo cons = type.GetConstructor(ptypes);
                if (cons == null)
                {
                    return null; // The type is stupid struct and does not have constructors
                }

                il.Emit(OpCodes.Nop);
                for (int i = 0; i < ptypes.Length; i++)
                {
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Ldc_I4, i);
                    il.Emit(OpCodes.Ldelem_Ref);
                    if (ptypes[i].IsValueType)
                    {
                        il.Emit(OpCodes.Unbox_Any, ptypes[i]);
                    }
                    else
                    {
                        il.Emit(OpCodes.Castclass, ptypes[i]);
                    }
                }
                il.Emit(OpCodes.Newobj, cons);
                il.Emit(OpCodes.Ret);

                instanceCreators.TryAdd(key, dm);
            }

            return instanceCreators[key];
        }

        /// <summary>
        /// Gets the field getter method.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="fieldInfo">The field info.</param>
        /// <returns></returns>
        private static DynamicMethod FieldGetterMethod(object instance, FieldInfo fieldInfo)
        {
            if (fieldInfo == null)
                throw new ArgumentException("There argument fieldInfo is null");

            var t = instance.GetType();
            var key = (t.FullName + "_" + fieldInfo.Name).Replace(".", "_");
            if (!fieldGetters.ContainsKey(key))
            {
                var getter = new DynamicMethod("__get_field_" + key, fieldInfo.FieldType, new[] { t }, typeof(EmitEngine), true);

                var getterIl = getter.GetILGenerator();
                getterIl.Emit(OpCodes.Ldarg_0);
                getterIl.Emit(OpCodes.Ldfld, fieldInfo);
                getterIl.Emit(OpCodes.Ret);

                fieldGetters.TryAdd(key, getter);
            }
            return fieldGetters[key];
        }


        /// <summary>
        /// Gets the field setter method.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns></returns>
        private static DynamicMethod FieldSetterMethod(object instance, string fieldName)
        {
            var t = instance.GetType();

            var key = (t.FullName + "_" + fieldName).Replace(".", "_");
            if (!fieldSetters.ContainsKey(key))
            {
                var fieldInfo = t.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public);
                if (fieldInfo == null)
                    throw new ArgumentException("There is no publicly accessible " + fieldName + " field found in " + t.FullName);

                var setter = new DynamicMethod("__set_field_" + key, null, new[] { t, fieldInfo.FieldType }, typeof(EmitEngine), true);

                var setterIL = setter.GetILGenerator();
                setterIL.Emit(OpCodes.Ldarg_0);
                setterIL.Emit(OpCodes.Ldarg_1);
                setterIL.Emit(OpCodes.Stfld, fieldInfo);
                setterIL.Emit(OpCodes.Ret);

                fieldSetters.TryAdd(key, setter);
            }
            return fieldSetters[key];
        }

        /// <summary>
        /// Gets the property getter method.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        private static DynamicMethod PropertyGetterMethod(object instance, string propertyName)
        {
            var t = instance.GetType();
            var key = (t.FullName + "_" + propertyName).Replace(".", "_");
            if (!propertyGetters.ContainsKey(key))
            {
                var propertyInfo = t.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                if (propertyInfo == null)
                    throw new ArgumentException("There is no publicly accessible " + propertyName + " property found in " + t.FullName);

                if (!propertyInfo.CanRead)
                    throw new ArgumentException("The property " + propertyName + " has no publicly accessible getter.");

                var getter = new DynamicMethod("__get_property_" + key, propertyInfo.PropertyType, new[] { t }, typeof(EmitEngine), true);

                var getterIl = getter.GetILGenerator();

                getterIl.Emit(OpCodes.Ldarg_0);
                getterIl.Emit(OpCodes.Callvirt, propertyInfo.GetGetMethod());
                getterIl.Emit(OpCodes.Ret);

                propertyGetters.TryAdd(key, getter);
            }
            return propertyGetters[key];
        }

        /// <summary>
        /// Gets the property setter method.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        private static DynamicMethod PropertySetterMethod(object instance, string propertyName)
        {
            var t = instance.GetType();
            var key = (t.FullName + "_" + propertyName).Replace(".", "_");
            if (!propertySetter.ContainsKey(key))
            {
                var propertyInfo = t.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
                if (propertyInfo == null)
                    throw new ArgumentException("There is no publicly accessible " + propertyName + " property found in " + t.FullName);

                if (!propertyInfo.CanWrite)
                    throw new ArgumentException("The property " + propertyName + " has no publicly accessible setter.");

                var setter = new DynamicMethod("__set_property_" + key, null, new[] { t, propertyInfo.PropertyType }, typeof(EmitEngine), true);

                var setterIl = setter.GetILGenerator();

                setterIl.Emit(OpCodes.Ldarg_0);
                setterIl.Emit(OpCodes.Ldarg_1);
                setterIl.Emit(OpCodes.Callvirt, propertyInfo.GetSetMethod());
                setterIl.Emit(OpCodes.Ret);

                propertySetter.TryAdd(key, setter);
            }
            return propertySetter[key];
        }

        /// <summary>
        /// Gets the method invoke method.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <returns></returns>
        private static DynamicMethod MethodInvokeMethod(object instance, string methodName)
        {
            var t = instance.GetType();
            var key = (t.FullName + "_" + methodName).Replace(".", "_");
            if (!methodInvokers.ContainsKey(key))
            {
                var methodInfo = t.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);
                if (methodInfo == null)
                    throw new ArgumentException("There is no publicly accessible " + methodName + " method found in " + t.FullName);

                DynamicMethod dynamicMethod = new DynamicMethod(string.Empty, typeof(object), new Type[] { typeof(object), typeof(object[]) }, methodInfo.DeclaringType.Module);

                ILGenerator ilGenerator = dynamicMethod.GetILGenerator();

                ParameterInfo[] parameters = methodInfo.GetParameters();

                Type[] paramTypes = new Type[parameters.Length];

                // copies the parameter types to an array
                for (int i = 0; i < paramTypes.Length; i++)
                {
                    if (parameters[i].ParameterType.IsByRef)
                        paramTypes[i] = parameters[i].ParameterType.GetElementType();
                    else
                        paramTypes[i] = parameters[i].ParameterType;
                }

                LocalBuilder[] locals = new LocalBuilder[paramTypes.Length];

                // generates a local variable for each parameter
                for (int i = 0; i < paramTypes.Length; i++)
                {
                    locals[i] = ilGenerator.DeclareLocal(paramTypes[i], true);
                }

                // creates code to copy the parameters to the local variables
                for (int i = 0; i < paramTypes.Length; i++)
                {
                    ilGenerator.Emit(OpCodes.Ldarg_1);
                    EmitFastInt(ilGenerator, i);
                    ilGenerator.Emit(OpCodes.Ldelem_Ref);
                    EmitCastToReference(ilGenerator, paramTypes[i]);
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
                    if (parameters[i].ParameterType.IsByRef)
                        ilGenerator.Emit(OpCodes.Ldloca_S, locals[i]);
                    else
                        ilGenerator.Emit(OpCodes.Ldloc, locals[i]);
                }

                // calls the method
                if (!methodInfo.IsStatic)
                {
                    ilGenerator.EmitCall(OpCodes.Callvirt, methodInfo, null);
                }
                else
                {
                    ilGenerator.EmitCall(OpCodes.Call, methodInfo, null);
                }

                // creates code for handling the return value
                if (methodInfo.ReturnType == typeof(void))
                {
                    ilGenerator.Emit(OpCodes.Ldnull);
                }
                else
                {
                    EmitBoxIfNeeded(ilGenerator, methodInfo.ReturnType);
                }

                // iterates through the parameters updating the parameters passed by ref
                for (int i = 0; i < paramTypes.Length; i++)
                {
                    if (parameters[i].ParameterType.IsByRef)
                    {
                        ilGenerator.Emit(OpCodes.Ldarg_1);
                        EmitFastInt(ilGenerator, i);
                        ilGenerator.Emit(OpCodes.Ldloc, locals[i]);
                        if (locals[i].LocalType.IsValueType)
                            ilGenerator.Emit(OpCodes.Box, locals[i].LocalType);
                        ilGenerator.Emit(OpCodes.Stelem_Ref);
                    }
                }

                // returns the value to the caller
                ilGenerator.Emit(OpCodes.Ret);

                methodInvokers.TryAdd(key, dynamicMethod);
            }
            return methodInvokers[key];
        }

        /// <summary>Emits the cast to a reference, unboxing if needed.</summary>
        /// <param name="ilGenerator"></param>
        /// <param name="type">The type to cast.</param>
        private static void EmitCastToReference(ILGenerator ilGenerator, Type type)
        {
            if (type.IsValueType)
            {
                ilGenerator.Emit(OpCodes.Unbox_Any, type);
            }
            else
            {
                ilGenerator.Emit(OpCodes.Castclass, type);
            }
        }

        /// <summary>Boxes a type if needed.</summary>
        /// <param name="ilGenerator">The MSIL generator.</param>
        /// <param name="type">The type.</param>
        private static void EmitBoxIfNeeded(ILGenerator ilGenerator, Type type)
        {
            if (type.IsValueType)
            {
                ilGenerator.Emit(OpCodes.Box, type);
            }
        }

        /// <summary>
        /// Emits code to save an integer to the evaluation stack.
        /// </summary>
        /// <param name="ilGenerator">The il generator.</param>
        /// <param name="value">The value to push.</param>
        private static void EmitFastInt(ILGenerator ilGenerator, int value)
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
                ilGenerator.Emit(OpCodes.Ldc_I4_S, (SByte)value);
            }
            else
            {
                ilGenerator.Emit(OpCodes.Ldc_I4, value);
            }
        }

        #endregion
    }
}
