// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WeakKeyDictionary.cs" company="Illusion">
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
//   The weak key dictionary.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Illusion.Utility
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    ///     The weak key dictionary.
    /// </summary>
    /// <typeparam name="TKey">
    ///     The type of the t key.
    /// </typeparam>
    /// <typeparam name="TValue">
    ///     The type of the t value.
    /// </typeparam>
    public class WeakKeyDictionary<TKey, TValue>
        where TKey : class
    {
        #region Fields

        /// <summary>
        ///     The keys.
        /// </summary>
        private readonly List<WeakReference> keys = new List<WeakReference>();

        /// <summary>
        ///     The values.
        /// </summary>
        private readonly List<TValue> values = new List<TValue>();

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the count.
        /// </summary>
        /// <value>The count.</value>
        public int Count
        {
            get
            {
                return this.keys.Count;
            }
        }

        /// <summary>
        ///     Gets the keys.
        /// </summary>
        /// <value>The keys.</value>
        public IList Keys
        {
            get
            {
                return this.keys;
            }
        }

        /// <summary>
        ///     Gets the values.
        /// </summary>
        /// <value>The values.</value>
        public IList<TValue> Values
        {
            get
            {
                return this.values;
            }
        }

        #endregion

        #region Public Indexers

        /// <summary>
        ///     Gets or sets the <see cref="TValue"/> with the specified key.
        /// </summary>
        /// <param name="key">
        ///     The key.
        /// </param>
        /// <returns>
        ///     The value.
        /// </returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">
        ///     The key is not found.
        /// </exception>
        public TValue this[TKey key]
        {
            get
            {
                int index = this.FindEntry(key);
                if (index >= 0)
                {
                    return this.values[index];
                }

                throw new KeyNotFoundException();
            }

            set
            {
                this.Insert(key, value, false);
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Adds the specified key.
        /// </summary>
        /// <param name="key">
        ///     The key.
        /// </param>
        /// <param name="value">
        ///     The value.
        /// </param>
        public void Add(TKey key, TValue value)
        {
            this.Insert(key, value, true);
        }

        /// <summary>
        ///     Clears this instance.
        /// </summary>
        public void Clear()
        {
            this.keys.Clear();
            this.values.Clear();
        }

        /// <summary>
        ///     Determines whether the specified key contains key.
        /// </summary>
        /// <param name="key">
        ///     The key.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the specified key contains key; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsKey(TKey key)
        {
            return this.FindEntry(key) >= 0;
        }

        /// <summary>
        ///     Determines whether the specified value contains value.
        /// </summary>
        /// <param name="value">
        ///  The value.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the specified value contains value; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsValue(TValue value)
        {
            return this.values.IndexOf(value) >= 0;
        }

        /// <summary>
        ///     Removes the specified key.
        /// </summary>
        /// <param name="key">
        ///     The key.
        /// </param>
        /// <returns>
        ///     <c>true</c> if XXXX, <c>false</c> otherwise.
        /// </returns>
        /// <exception cref="System.ArgumentException">
        ///     The key is null.
        /// </exception>
        public bool Remove(TKey key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            int index = this.FindEntry(key);
            if (index >= 0)
            {
                this.keys.RemoveAt(index);

                // Dispose the value
                var dispose = this.values[index] as IDisposable;
                if (dispose != null)
                {
                    dispose.Dispose();
                }

                this.values.RemoveAt(index);
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Tries the get value.
        /// </summary>
        /// <param name="key">
        ///     The key.
        /// </param>
        /// <param name="value">
        ///     The value.
        /// </param>
        /// <returns>
        ///     <c>true</c> if XXXX, <c>false</c> otherwise.
        /// </returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            int index = this.FindEntry(key);
            if (index >= 0)
            {
                value = this.values[index];
                return true;
            }

            value = default(TValue);
            return false;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Finds the entry with key.
        /// </summary>
        /// <param name="key">
        ///     The key.
        /// </param>
        /// <returns>
        ///     The <see cref="Int32"/>.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1126:PrefixCallsCorrectly", 
            Justification = "Reviewed. Suppression is OK here.")]
        private int FindEntry(TKey key)
        {
            int result = -1;
            for (int i = this.Count - 1; i >= 0; i--)
            {
                WeakReference currentKey = this.keys[i];
                var target = (TKey)currentKey.Target;
                if (target == null)
                {
                    this.keys.RemoveAt(i);
                    this.values.RemoveAt(i);
                    result--;
                }
                else
                {
                    if (Equals(target, key))
                    {
                        result = i;
                    }
                }
            }

            return result < 0 ? -1 : result;
        }

        /// <summary>
        ///     Inserts the value with the specified key.
        /// </summary>
        /// <param name="key">
        ///     The key.
        /// </param>
        /// <param name="value">
        ///     The value.
        /// </param>
        /// <param name="add">
        ///     If set to <c>true</c>, add it.
        /// </param>
        /// <exception cref="System.ArgumentException">
        ///     The key is null.
        /// </exception>
        private void Insert(TKey key, TValue value, bool add)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            int index = this.FindEntry(key);
            if (index >= 0)
            {
                if (add)
                {
                    throw new ArgumentException("Duplicate key");
                }

                this.values[index] = value;
                return;
            }

            this.keys.Add(new WeakReference(key));
            this.values.Add(value);
        }

        #endregion
    }
}