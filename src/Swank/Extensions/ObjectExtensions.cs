using System;
using System.Linq;
using System.Web.Script.Serialization;

namespace FubuMVC.Swank.Extensions
{
    public static class ObjectExtensions
    {
        public class OtherwiseOptions<TResult>
        {
            private readonly object _value;
            private readonly Func<TResult> _returnThis;

            public OtherwiseOptions(object value, Func<TResult> returnThis)
            {
                _value = value;
                _returnThis = returnThis;
            }

            public TResult OtherwiseDefault()
            {
                return Otherwise(default(TResult));
            }

            public TResult Otherwise(params TResult[] values)
            {
                return _value != null ? _returnThis() : values.FirstOrDefault(x => x != null);
            }

            public OtherwiseOptions<TNextResult> WhenNotNull<TNextResult>(Func<TResult, TNextResult> returnThis)
            {
                return OtherwiseDefault().WhenNotNull(returnThis);
            }
        }

        public static OtherwiseOptions<TResult> WhenNotNull<TSource, TResult>(this TSource value, Func<TSource, TResult> returnThis)
        {
            return new OtherwiseOptions<TResult>(value, () => returnThis(value));
        }

        public static T DeserializeJson<T>(this string json)
        {
            return new JavaScriptSerializer().Deserialize<T>(json);
        }

        public static string SerializeJson<T>(this T source)
        {
            return new JavaScriptSerializer().Serialize(source);
        }

        public static string ToSampleValueString(this object value, Configuration configuration)
        {
            var type = value.GetType();
            if (type == typeof(Decimal)) return ((Decimal)value).ToString(configuration.SampleRealFormat);
            if (type == typeof(Decimal?)) return ((Decimal?)value).Value.ToString(configuration.SampleRealFormat);
            if (type == typeof(Double)) return ((Double)value).ToString(configuration.SampleRealFormat);
            if (type == typeof(Double?)) return ((Double?)value).Value.ToString(configuration.SampleRealFormat);
            if (type == typeof(Single)) return ((Single)value).ToString(configuration.SampleRealFormat);
            if (type == typeof(Single?)) return ((Single?)value).Value.ToString(configuration.SampleRealFormat);

            if (type == typeof(Byte)) return ((Byte)value).ToString(configuration.SampleIntegerFormat);
            if (type == typeof(Byte?)) return ((Byte?)value).Value.ToString(configuration.SampleIntegerFormat);
            if (type == typeof(SByte)) return ((SByte)value).ToString(configuration.SampleIntegerFormat);
            if (type == typeof(SByte?)) return ((SByte?)value).Value.ToString(configuration.SampleIntegerFormat);
            if (type == typeof(Int16)) return ((Int16)value).ToString(configuration.SampleIntegerFormat);
            if (type == typeof(Int16?)) return ((Int16?)value).Value.ToString(configuration.SampleIntegerFormat);
            if (type == typeof(UInt16)) return ((UInt16)value).ToString(configuration.SampleIntegerFormat);
            if (type == typeof(UInt16?)) return ((UInt16?)value).Value.ToString(configuration.SampleIntegerFormat);
            if (type == typeof(Int32)) return ((Int32)value).ToString(configuration.SampleIntegerFormat);
            if (type == typeof(Int32?)) return ((Int32?)value).Value.ToString(configuration.SampleIntegerFormat);
            if (type == typeof(UInt32)) return ((UInt32)value).ToString(configuration.SampleIntegerFormat);
            if (type == typeof(UInt32?)) return ((UInt32?)value).Value.ToString(configuration.SampleIntegerFormat);
            if (type == typeof(Int64)) return ((Int64)value).ToString(configuration.SampleIntegerFormat);
            if (type == typeof(Int64?)) return ((Int64?)value).Value.ToString(configuration.SampleIntegerFormat);
            if (type == typeof(UInt64)) return ((UInt64)value).ToString(configuration.SampleIntegerFormat);
            if (type == typeof(UInt64?)) return ((UInt64?)value).Value.ToString(configuration.SampleIntegerFormat);

            if (type == typeof(DateTime)) return ((DateTime)value).ToString(configuration.SampleDateTimeFormat);
            if (type == typeof(DateTime?)) return ((DateTime?)value).Value.ToString(configuration.SampleDateTimeFormat);

            if (type == typeof(TimeSpan)) return ((TimeSpan)value).ToString(configuration.SampleTimeSpanFormat);
            if (type == typeof(TimeSpan?)) return ((TimeSpan?)value).Value.ToString(configuration.SampleTimeSpanFormat);

            if (type == typeof(Guid)) return ((Guid)value).ToString(configuration.SampleGuidFormat);
            if (type == typeof(Guid?)) return ((Guid?)value).Value.ToString(configuration.SampleGuidFormat);

            if (type.IsEnum) return configuration.EnumFormat == EnumFormat.AsString ? value.ToString() : ((int)value).ToString();

            return value.ToString();
        }
    }
}