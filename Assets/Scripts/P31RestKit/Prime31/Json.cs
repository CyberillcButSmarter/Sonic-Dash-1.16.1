using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace Prime31
{
	public class Json
	{
		internal class Serializer
		{
			private StringBuilder _builder;

			private Serializer()
			{
				_builder = new StringBuilder();
			}

			public static string serialize(object obj)
			{
				Serializer serializer = new Serializer();
				serializer.serializeObject(obj);
				return serializer._builder.ToString();
			}

			private void serializeObject(object value)
			{
				if (value == null)
				{
					_builder.Append("null");
					return;
				}
				if (value is string)
				{
					serializeString((string)value);
					return;
				}
				if (value is IList)
				{
					serializeIList((IList)value);
					return;
				}
				if (value is Dictionary<string, object>)
				{
					serializeDictionary((Dictionary<string, object>)value);
					return;
				}
				if (value is IDictionary)
				{
					serializeIDictionary((IDictionary)value);
					return;
				}
				if (value is bool)
				{
					_builder.Append(value.ToString().ToLower());
					return;
				}
				if (value.GetType().IsPrimitive)
				{
					_builder.Append(value);
					return;
				}
				if (value is DateTime)
				{
					DateTime value2 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
					double totalMilliseconds = ((DateTime)value).Subtract(value2).TotalMilliseconds;
					serializeString(Convert.ToString(totalMilliseconds, CultureInfo.InvariantCulture));
					return;
				}
				try
				{
					serializeClass(value);
				}
				catch (Exception ex)
				{
					Utils.logObject(string.Format("failed to serialize {0} with error: {1}", value, ex.Message));
				}
			}

			private void serializeIList(IList anArray)
			{
				_builder.Append("[");
				bool flag = true;
				for (int i = 0; i < anArray.Count; i++)
				{
					object value = anArray[i];
					if (!flag)
					{
						_builder.Append(", ");
					}
					serializeObject(value);
					flag = false;
				}
				_builder.Append("]");
			}

			private void serializeIDictionary(IDictionary dict)
			{
				_builder.Append("{");
				bool flag = true;
				foreach (object key in dict.Keys)
				{
					if (!flag)
					{
						_builder.Append(", ");
					}
					serializeString(key.ToString());
					_builder.Append(":");
					serializeObject(dict[key]);
					flag = false;
				}
				_builder.Append("}");
			}

			private void serializeDictionary(Dictionary<string, object> dict)
			{
				_builder.Append("{");
				bool flag = true;
				Dictionary<string, object>.KeyCollection keys = dict.Keys;
				foreach (string item in keys)
				{
					if (!flag)
					{
						_builder.Append(", ");
					}
					serializeString(item.ToString());
					_builder.Append(":");
					serializeObject(dict[item]);
					flag = false;
				}
				_builder.Append("}");
			}

			private void serializeString(string str)
			{
				_builder.Append("\"");
				char[] array = str.ToCharArray();
				foreach (char c in array)
				{
					switch (c)
					{
					case '"':
						_builder.Append("\\\"");
						continue;
					case '\\':
						_builder.Append("\\\\");
						continue;
					case '\b':
						_builder.Append("\\b");
						continue;
					case '\f':
						_builder.Append("\\f");
						continue;
					case '\n':
						_builder.Append("\\n");
						continue;
					case '\r':
						_builder.Append("\\r");
						continue;
					case '\t':
						_builder.Append("\\t");
						continue;
					}
					int num = Convert.ToInt32(c, CultureInfo.InvariantCulture);
					if (num >= 32 && num <= 126)
					{
						_builder.Append(c);
					}
					else
					{
						_builder.Append("\\u" + Convert.ToString(num, 16).PadLeft(4, '0'));
					}
				}
				_builder.Append("\"");
			}

			private void serializeClass(object value)
			{
				_builder.Append("{");
				bool flag = true;
				FieldInfo[] fields = value.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				foreach (FieldInfo fieldInfo in fields)
				{
					if (!fieldInfo.IsPrivate || !fieldInfo.Name.Contains("k__BackingField"))
					{
						if (!flag)
						{
							_builder.Append(", ");
						}
						serializeString(fieldInfo.Name);
						_builder.Append(":");
						serializeObject(fieldInfo.GetValue(value));
						flag = false;
					}
				}
				PropertyInfo[] properties = value.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				foreach (PropertyInfo propertyInfo in properties)
				{
					if (!flag)
					{
						_builder.Append(", ");
					}
					serializeString(propertyInfo.Name);
					_builder.Append(":");
					serializeObject(propertyInfo.GetValue(value, null));
					flag = false;
				}
				_builder.Append("}");
			}
		}

		public static bool useSimpleJson = true;

		public static string encode(object obj)
		{
			string text = ((!useSimpleJson) ? Serializer.serialize(obj) : SimpleJson.encode(obj));
			if (text == null)
			{
				Utils.logObject("Something went wrong serializing the object. We got a null return. Here is the object we tried to deserialize: ");
				Utils.logObject(obj);
			}
			return text;
		}
	}
}
