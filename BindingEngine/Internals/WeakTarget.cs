// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WeakTarget.cs" company="Illusion">
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
//   Represents the binding entity of a target, it holds all the <see cref="WeakBinding"/> used for the specific bind target.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Illusion.Utility
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    ///     Represents the binding entity of a target, it holds all the <see cref="WeakBinding"/> used for the specific bind target.
    /// </summary>
    internal class WeakTarget : WeakReference
    {
        #region Fields

        /// <summary>
        ///     The bindings.
        /// </summary>
        private readonly Dictionary<WeakEntry, WeakBinding> bindings = new Dictionary<WeakEntry, WeakBinding>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="WeakTarget"/> class.
        /// </summary>
        /// <param name="source">
        ///     The source.
        /// </param>
        public WeakTarget(object source)
            : base(source)
        {
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Get the binding.
        /// </summary>
        /// <typeparam name="T">
        ///     The concrete <see cref="WeakBinding"/> to return.
        /// </typeparam>        
        /// <param name="targetProperty">
        ///     The target property.
        /// </param>
        /// <param name="source">
        ///     The source.
        /// </param>
        /// <param name="sourceProperty">
        ///     The source property.
        /// </param>
        /// <returns>
        ///     The <see cref="T"/>.
        /// </returns>
        public T GetBinding<T>(string targetProperty, object source, string sourceProperty)
            where T : WeakBinding
        {
            var entry = new WeakEntry(targetProperty, source, sourceProperty);
            if (this.bindings.ContainsKey(entry))
            {
                return this.bindings[entry] as T;
            }

            return null;
        }

        /// <summary>
        ///     Clears the binding.
        /// </summary>
        /// <param name="targetProperty">
        ///     The target property.
        /// </param>
        /// <param name="source">
        ///     The source.
        /// </param>
        /// <param name="sourceProperty">
        ///     The source property.
        /// </param>
        public void ClearBinding(string targetProperty, object source, string sourceProperty)
        {
            var entry = new WeakEntry(targetProperty, source, sourceProperty);
            if (this.bindings.ContainsKey(entry))
            {
                this.bindings[entry].Clear();
                this.bindings.Remove(entry);
            }
        }

        /// <summary>
        ///     Clears the bindings.
        /// </summary>
        /// <param name="targetProperty">
        ///     The target property.
        /// </param>
        public void ClearBindings(string targetProperty)
        {
            var removeBindings =
                this.bindings.Where(item => string.Equals(item.Value.TargetProperty, targetProperty))
                             .ToArray();

            foreach (var binding in removeBindings)
            {
                binding.Value.Clear();
                this.bindings.Remove(binding.Key);
            }
        }

        /// <summary>
        ///     Clears the bindings.
        /// </summary>
        public void ClearBindings()
        {
            foreach (WeakBinding binding in this.bindings.Values)
            {
                binding.Clear();
            }

            this.bindings.Clear();
        }

        /// <summary>
        ///     Sets the binding.
        /// </summary>
        /// <typeparam name="T">
        ///     The concrete <see cref="WeakBinding"/> to return.
        /// </typeparam>
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
        /// <exception cref="System.NotSupportedException">
        ///     The new binding is not compatible with the existing binding.
        /// </exception>
        public T SetBinding<T>(string targetProperty, object source, string sourceProperty, bool activate = true)
            where T : WeakBinding
        {
            var entry = new WeakEntry(targetProperty, source, sourceProperty);
            WeakBinding binding;
            if (this.bindings.ContainsKey(entry))
            {
                binding = this.bindings[entry];
                if (typeof(T) != binding.GetType())
                {
                    throw new NotSupportedException(
                        string.Format(
                            "The new {0} is not compatible with the existing {1}, ", 
                            typeof(T), 
                            binding.GetType())
                        + "please clear the binding and rebind or update WeakTarget.SetBinding method to clear the existing binding first");
                }

                binding.Update(targetProperty, source, sourceProperty);
            }
            else
            {
                binding = DynamicEngine.CreateInstance<T>(this.Target, targetProperty, source, sourceProperty);
                binding.Initialize<T>(activate);

                this.bindings.Add(entry, binding);
            }

            return binding as T;
        }

        #endregion
    }
}