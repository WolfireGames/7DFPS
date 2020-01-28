using System.Collections.Generic;
using System.Linq;
using System;
using System.Reflection;
using UnityEngine;

namespace ExtentionUtil {
    public static class DictionaryExtensions {
        public static void Merge<TKey, TValue>(this Dictionary<TKey, TValue> dictionaryA, Dictionary<TKey, TValue> dictionaryB) {
            dictionaryB.ToList().ForEach(x => dictionaryA[x.Key] = x.Value);
        }
    }

    public static class TransformExtension {
        /// <summary>
        /// Uses recursion to find Transforms of Transforms. (Equivalent to Transform.Find if deep_search==false)
        /// </summary>
        public static Transform Find(this Transform transform, string name, bool deep_search) {
            if(!deep_search)
                return transform.Find(name);

            foreach (Transform child in transform) {
                // Are we looking for the current one?
                if(child.name.Equals(name))
                    return child;

                // See if our target is a child of the current one
                Transform grandchild_transform = child.Find(name, true);
                if(grandchild_transform)
                    return grandchild_transform;
            }

            return null;
        }

        // Interpolation methods
        public static void Lerp(this Transform transform, Transform a, Transform b, float t) {
            transform.localPosition = Vector3.Lerp(a.localPosition, b.localPosition, t);
            transform.localRotation = Quaternion.Slerp(a.localRotation, b.localRotation, t);
            transform.localScale = Vector3.Lerp(a.localScale, b.localScale, t);
        }

        public static void LerpPosition(this Transform transform, Transform a, Transform b, float t) {
            transform.localPosition = Vector3.Lerp(a.localPosition, b.localPosition, t);
        }
        public static void LerpPosition(this Transform transform, Vector3 a, Transform b, float t) {
            transform.localPosition = Vector3.Lerp(a, b.localPosition, t);
        }

        public static void LerpRotation(this Transform transform, Transform a, Transform b, float t) {
            transform.localRotation = Quaternion.Slerp(a.localRotation, b.localRotation, t);
        }
        public static void LerpRotation(this Transform transform, Quaternion a, Transform b, float t) {
            transform.localRotation = Quaternion.Slerp(a, b.localRotation, t);
        }

        public static void LerpScale(this Transform transform, Transform a, Transform b, float t) {
            transform.localScale = Vector3.Lerp(a.localScale, b.localScale, t);
        }
        public static void LerpScale(this Transform transform, Vector3 a, Transform b, float t) {
            transform.localScale = Vector3.Lerp(a, b.localScale, t);
        }
    }

    /// <summary> Get the attribute of a single aspect's constant field. (Ex: <T> from GunAspect.HAMMER) </summary>
    public static class GunAspectExtension {
        public static T GetAttribute<T>(this GunAspect aspect) {
            if(aspect.value.Length != 1) 
                throw new System.Exception("Tried to get fields of multiple aspects");
            
            int target = aspect.value[0];
            foreach(FieldInfo field in typeof(GunAspect).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)) {
                if(field.FieldType == typeof(int) && (int)field.GetRawConstantValue() == target) {
                    if(field.GetCustomAttribute(typeof(T)) is T attribute) {
                        return attribute;
                    }
                }
            }
            return default(T);
        }
    }

    // https://stackoverflow.com/questions/2362580/discovering-derived-types-using-reflection - Nate Barbettini
    public static class TypeExtensions {
        public static List<Type> GetAllDerivedTypes(this Type type, Type namespace_type) {
            return Assembly.GetAssembly(type).GetAllDerivedTypes(type, namespace_type.Namespace);
        }

        public static List<Type> GetAllDerivedTypes(this Type type) {
            return Assembly.GetAssembly(type).GetAllDerivedTypes(type);
        }

        public static List<Type> GetAllDerivedTypes(this Assembly assembly, Type type, string target_namespace = "") {
            return assembly
                .GetTypes()
                .Where(t => t != type && type.IsAssignableFrom(t) && (target_namespace == "" || t.Namespace == target_namespace))
                .ToList();
        }
    }
}
