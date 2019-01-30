using System;
using Newtonsoft.Json;

namespace Newcats.JobManager.Common.NetCore.Util.Json
{
    public static class JsonExtensions
    {
        #region Serialize
        /// <summary>
        /// Serializes the specified object to a JSON string.
        /// </summary>
        /// <param name="value">The object to serialize.</param>
        /// <returns>A JSON string representation of the object.</returns>
        public static string ToJson(this object value)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(value);
        }

        /// <summary>
        /// Serializes the specified object to a JSON string using formatting.
        /// </summary>
        /// <param name="value">The object to serialize.</param>
        /// <param name="formatting">Indicates how the output should be formatted.</param>
        /// <returns>
        /// A JSON string representation of the object.
        /// </returns>
        public static string ToJson(this object value, Formatting formatting)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(value, formatting);
        }

        /// <summary>
        /// Serializes the specified object to a JSON string using a collection of <see cref="T:Newtonsoft.Json.JsonConverter" />.
        /// </summary>
        /// <param name="value">The object to serialize.</param>
        /// <param name="converters">A collection of converters used while serializing.</param>
        /// <returns>A JSON string representation of the object.</returns>
        public static string ToJson(this object value, params JsonConverter[] converters)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(value, converters);
        }

        /// <summary>
        /// Serializes the specified object to a JSON string using formatting and a collection of <see cref="T:Newtonsoft.Json.JsonConverter" />.
        /// </summary>
        /// <param name="value">The object to serialize.</param>
        /// <param name="formatting">Indicates how the output should be formatted.</param>
        /// <param name="converters">A collection of converters used while serializing.</param>
        /// <returns>A JSON string representation of the object.</returns>
        public static string ToJson(this object value, Formatting formatting, params JsonConverter[] converters)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(value, formatting, converters);
        }

        /// <summary>
        /// Serializes the specified object to a JSON string using <see cref="T:Newtonsoft.Json.JsonSerializerSettings" />.
        /// </summary>
        /// <param name="value">The object to serialize.</param>
        /// <param name="settings">The <see cref="T:Newtonsoft.Json.JsonSerializerSettings" /> used to serialize the object.
        /// If this is <c>null</c>, default serialization settings will be used.</param>
        /// <returns>
        /// A JSON string representation of the object.
        /// </returns>
        public static string ToJson(this object value, JsonSerializerSettings settings)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(value, settings);
        }

        /// <summary>
        /// Serializes the specified object to a JSON string using a type, formatting and <see cref="T:Newtonsoft.Json.JsonSerializerSettings" />.
        /// </summary>
        /// <param name="value">The object to serialize.</param>
        /// <param name="settings">The <see cref="T:Newtonsoft.Json.JsonSerializerSettings" /> used to serialize the object.
        /// If this is <c>null</c>, default serialization settings will be used.</param>
        /// <param name="type">
        /// The type of the value being serialized.
        /// This parameter is used when <see cref="P:Newtonsoft.Json.JsonSerializer.TypeNameHandling" /> is <see cref="F:Newtonsoft.Json.TypeNameHandling.Auto" /> to write out the type name if the type of the value does not match.
        /// Specifying the type is optional.
        /// </param>
        /// <returns>
        /// A JSON string representation of the object.
        /// </returns>
        public static string ToJson(this object value, Type type, JsonSerializerSettings settings)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(value, type, settings);
        }

        /// <summary>
        /// Serializes the specified object to a JSON string using formatting and <see cref="T:Newtonsoft.Json.JsonSerializerSettings" />.
        /// </summary>
        /// <param name="value">The object to serialize.</param>
        /// <param name="formatting">Indicates how the output should be formatted.</param>
        /// <param name="settings">The <see cref="T:Newtonsoft.Json.JsonSerializerSettings" /> used to serialize the object.
        /// If this is <c>null</c>, default serialization settings will be used.</param>
        /// <returns>
        /// A JSON string representation of the object.
        /// </returns>
        public static string ToJson(this object value, Formatting formatting, JsonSerializerSettings settings)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(value, formatting, settings);
        }

        /// <summary>
        /// Serializes the specified object to a JSON string using a type, formatting and <see cref="T:Newtonsoft.Json.JsonSerializerSettings" />.
        /// </summary>
        /// <param name="value">The object to serialize.</param>
        /// <param name="formatting">Indicates how the output should be formatted.</param>
        /// <param name="settings">The <see cref="T:Newtonsoft.Json.JsonSerializerSettings" /> used to serialize the object.
        /// If this is <c>null</c>, default serialization settings will be used.</param>
        /// <param name="type">
        /// The type of the value being serialized.
        /// This parameter is used when <see cref="P:Newtonsoft.Json.JsonSerializer.TypeNameHandling" /> is <see cref="F:Newtonsoft.Json.TypeNameHandling.Auto" /> to write out the type name if the type of the value does not match.
        /// Specifying the type is optional.
        /// </param>
        /// <returns>
        /// A JSON string representation of the object.
        /// </returns>
        public static string ToJson(this object value, Type type, Formatting formatting, JsonSerializerSettings settings)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(value, type, formatting, settings);
        }
        #endregion

        #region Deserialize
        /// <summary>
        /// Deserializes the JSON to a .NET object.
        /// </summary>
        /// <param name="value">The JSON to deserialize.</param>
        /// <returns>The deserialized object from the JSON string.</returns>
        public static object DeserializeJson(this string value)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject(value);
        }

        /// <summary>
        /// Deserializes the JSON to a .NET object using <see cref="T:Newtonsoft.Json.JsonSerializerSettings" />.
        /// </summary>
        /// <param name="value">The JSON to deserialize.</param>
        /// <param name="settings">
        /// The <see cref="T:Newtonsoft.Json.JsonSerializerSettings" /> used to deserialize the object.
        /// If this is <c>null</c>, default serialization settings will be used.
        /// </param>
        /// <returns>The deserialized object from the JSON string.</returns>
        public static object DeserializeJson(this string value, JsonSerializerSettings settings)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject(value, settings);
        }

        /// <summary>
        /// Deserializes the JSON to the specified .NET type.
        /// </summary>
        /// <param name="value">The JSON to deserialize.</param>
        /// <param name="type">The <see cref="T:System.Type" /> of object being deserialized.</param>
        /// <returns>The deserialized object from the JSON string.</returns>
        public static object DeserializeJson(this string value, Type type)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject(value, type);
        }

        /// <summary>
        /// Deserializes the JSON to the specified .NET type.
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
        /// <param name="value">The JSON to deserialize.</param>
        /// <returns>The deserialized object from the JSON string.</returns>
        public static T DeserializeJson<T>(this string value)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(value);
        }

        /// <summary>
        /// Deserializes the JSON to the given anonymous type.
        /// </summary>
        /// <typeparam name="T">
        /// The anonymous type to deserialize to. This can't be specified
        /// traditionally and must be inferred from the anonymous type passed
        /// as a parameter.
        /// </typeparam>
        /// <param name="value">The JSON to deserialize.</param>
        /// <param name="anonymousTypeObject">The anonymous type object.</param>
        /// <returns>The deserialized anonymous type from the JSON string.</returns>
        public static T DeserializeJson<T>(this string value, T anonymousTypeObject)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeAnonymousType<T>(value, anonymousTypeObject);
        }

        /// <summary>
        /// Deserializes the JSON to the given anonymous type using <see cref="T:Newtonsoft.Json.JsonSerializerSettings" />.
        /// </summary>
        /// <typeparam name="T">
        /// The anonymous type to deserialize to. This can't be specified
        /// traditionally and must be inferred from the anonymous type passed
        /// as a parameter.
        /// </typeparam>
        /// <param name="value">The JSON to deserialize.</param>
        /// <param name="anonymousTypeObject">The anonymous type object.</param>
        /// <param name="settings">
        /// The <see cref="T:Newtonsoft.Json.JsonSerializerSettings" /> used to deserialize the object.
        /// If this is <c>null</c>, default serialization settings will be used.
        /// </param>
        /// <returns>The deserialized anonymous type from the JSON string.</returns>
        public static T DeserializeJson<T>(this string value, T anonymousTypeObject, JsonSerializerSettings settings)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeAnonymousType<T>(value, anonymousTypeObject, settings);
        }

        /// <summary>
        /// Deserializes the JSON to the specified .NET type using a collection of <see cref="T:Newtonsoft.Json.JsonConverter" />.
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
        /// <param name="value">The JSON to deserialize.</param>
        /// <param name="converters">Converters to use while deserializing.</param>
        /// <returns>The deserialized object from the JSON string.</returns>
        public static T DeserializeJson<T>(this string value, params JsonConverter[] converters)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(value, converters);
        }

        /// <summary>
        /// Deserializes the JSON to the specified .NET type using <see cref="T:Newtonsoft.Json.JsonSerializerSettings" />.
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
        /// <param name="value">The object to deserialize.</param>
        /// <param name="settings">
        /// The <see cref="T:Newtonsoft.Json.JsonSerializerSettings" /> used to deserialize the object.
        /// If this is <c>null</c>, default serialization settings will be used.
        /// </param>
        /// <returns>The deserialized object from the JSON string.</returns>
        public static T DeserializeJson<T>(this string value, JsonSerializerSettings settings)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(value, settings);
        }

        /// <summary>
        /// Deserializes the JSON to the specified .NET type using a collection of <see cref="T:Newtonsoft.Json.JsonConverter" />.
        /// </summary>
        /// <param name="value">The JSON to deserialize.</param>
        /// <param name="type">The type of the object to deserialize.</param>
        /// <param name="converters">Converters to use while deserializing.</param>
        /// <returns>The deserialized object from the JSON string.</returns>
        public static object DeserializeJson(this string value, Type type, params JsonConverter[] converters)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject(value, type, converters);
        }

        /// <summary>
        /// Deserializes the JSON to the specified .NET type using <see cref="T:Newtonsoft.Json.JsonSerializerSettings" />.
        /// </summary>
        /// <param name="value">The JSON to deserialize.</param>
        /// <param name="type">The type of the object to deserialize to.</param>
        /// <param name="settings">
        /// The <see cref="T:Newtonsoft.Json.JsonSerializerSettings" /> used to deserialize the object.
        /// If this is <c>null</c>, default serialization settings will be used.
        /// </param>
        /// <returns>The deserialized object from the JSON string.</returns>
        public static object DeserializeJson(this string value, Type type, JsonSerializerSettings settings)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject(value, type, settings);
        }
        #endregion
    }
}