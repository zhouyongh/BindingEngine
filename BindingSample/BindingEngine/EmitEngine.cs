using System;
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
	/// 1.Access Property
	///  public class TestObject
	///  {
	///		public string Name { get; set; }
	///	 }
	///	 
	///  TestObject testObj = new TestObject();
	///  testObj.Name = "1"; &lt;-- equals to:  --&gt; DynamicAccessEngine.SetProperty(testObj, "Name", "1");
	///  
	/// 2.CreateInstance
    ///  TestObject testObj = new TestObject();   &lt;-- equals to:  --&gt; TestObject testObj = DynamicAccessEngine.CreateInstance<TestObject>();
	/// </example>
	/// </summary>
	public class EmitEngine
	{
		#region Field

		private static readonly Dictionary<string, DynamicMethod> fieldSetters = new Dictionary<string, DynamicMethod>();
		private static readonly Dictionary<string, DynamicMethod> fieldGetters = new Dictionary<string, DynamicMethod>();
		private static readonly Dictionary<string, DynamicMethod> propertyGetters = new Dictionary<string, DynamicMethod>();
		private static readonly Dictionary<string, DynamicMethod> propertySetter = new Dictionary<string, DynamicMethod>();
		private static readonly Dictionary<string, DynamicMethod> instanceCreators = new Dictionary<string, DynamicMethod>();
		private static readonly object fgSync = new object();
		private static readonly object fsSync = new object();
		private static readonly object pgSync = new object();
		private static readonly object psSync = new object();
		private static readonly object isSync = new object();

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
			    else
			    {
				    valueTypes = new Type[parameters.Length];
				    for (int i = 0; i < parameters.Length; i++)
				    {
					    valueTypes[i] = parameters[i].GetType();
				    }			        
			    }
			}

			var method = CreateInstanceMethod(type, valueTypes);
			if (method != null)
			{
				return method.Invoke(null, new object[] { parameters });
			} 
			else // This type is stupid Struct that does not have constructors.
			{
				return Activator.CreateInstance(type);
			}
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
				lock (fgSync)
				{
					var fieldInfo = t.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public);
					if (fieldInfo == null)
						throw new ArgumentException("There is no publicly accessible " + fieldName + " field found in " + t.FullName);

					return FieldGetterMethod(instance, fieldInfo);
				}
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
				lock (isSync)
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

					instanceCreators.Add(key, dm);
				}
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
				lock (fgSync)
				{
					var getter = new DynamicMethod("__get_field_" + key, fieldInfo.FieldType, new[] { t }, typeof(EmitEngine), true);

					var getterIL = getter.GetILGenerator();
					getterIL.Emit(OpCodes.Ldarg_0);
					getterIL.Emit(OpCodes.Ldfld, fieldInfo);
					getterIL.Emit(OpCodes.Ret);

					fieldGetters.Add(key, getter);
				}
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
				lock (fsSync)
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

					fieldSetters.Add(key, setter);
				}
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
				lock (pgSync)
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

					propertyGetters.Add(key, getter);
				}
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
				lock (psSync)
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

					propertySetter.Add(key, setter);
				}
			}
			return propertySetter[key];
		}

		#endregion
	}
}
