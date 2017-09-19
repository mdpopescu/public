using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using WindowsFormsApp1.Models;

namespace WindowsFormsApp1
{
    public static class Extensions
    {
        public static IObservable<T> SafeGet<T>(this IReadOnlyDictionary<string, IObservable<T>> dict, string key) =>
            dict.ContainsKey(key) ? dict[key] : Observable.Empty<T>();

        public static IObservable<LabeledValue> Intercept(this object target, params string[] events)
        {
            return events
                .Select(eventName => Observable.FromEventPattern(target, eventName).Select(it => new LabeledValue(eventName, target)))
                .Merge();
        }

        /// <summary>Unwraps the values from a stream of labeled values, but only for those matching a given label.</summary>
        /// <typeparam name="T">The type of the extracted value.</typeparam>
        /// <param name="labeledValues">The labeled values.</param>
        /// <param name="inputLabel">The label for the values to be extracted.</param>
        /// <returns>The values that match the given label, unwrapped.</returns>
        public static IObservable<T> Extract<T>(this IObservable<LabeledValue> labeledValues, string inputLabel)
        {
            return labeledValues
                .Where(it => it.Label == inputLabel)
                .Select(it => (T) it.Value);
        }

        public static IObservable<LabeledValue> Transform<T>(
            this IObservable<LabeledValue> labeledValues,
            string inputLabel,
            string outputLabel,
            Func<T, object> func)
        {
            return labeledValues
                .Extract<T>(inputLabel)
                .Select(it => new LabeledValue(outputLabel, func(it)));
        }

        /// <summary>Replaces one label with another.</summary>
        /// <param name="labeledValues">The labeled values.</param>
        /// <param name="inputLabel">The input label.</param>
        /// <param name="outputLabel">The output label.</param>
        /// <returns>A stream of labeled values.</returns>
        public static IObservable<LabeledValue> Relabel(
            this IObservable<LabeledValue> labeledValues,
            string inputLabel,
            string outputLabel)
        {
            return labeledValues
                .Extract<object>(inputLabel)
                .Select(it => new LabeledValue(outputLabel, it));
        }

        /// <summary>Combines the specified dictionaries.</summary>
        /// <param name="dictionaries">The dictionaries.</param>
        /// <returns>A single dictionary combining all inputs.</returns>
        public static IReadOnlyDictionary<string, IObservable<LabeledValue>> Combine(
            params IReadOnlyDictionary<string, IObservable<LabeledValue>>[] dictionaries)
        {
            return dictionaries
                .SelectMany(dict => dict)
                .GroupBy(pair => pair.Key, pair => pair.Value)
                .ToDictionary(group => group.Key, group => group.Merge());
        }

        /// <summary>Executes an action if the given object is not <c>null</c>.</summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="action">The action.</param>
        public static void IfNotNull<T>(this T obj, Action<T> action)
        {
            if (obj != null)
                action(obj);
        }
    }
}