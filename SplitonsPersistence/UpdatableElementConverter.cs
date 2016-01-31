using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SplitonsPersistence
{
    class UpdatableElementConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            // this converter can be applied to any type
            return objectType == typeof(UpdatableElement);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            // we currently support only writing of JSON
            JObject jsonObject = JObject.Load(reader);
            var element = new UpdatableElement();
            element.SerializedValue = JsonConvert.SerializeObject(jsonObject);
            element.id = jsonObject.GetValue("id").ToString();
            element.lastUpdated = long.Parse(jsonObject.GetValue("lastUpdated").ToString());
            return element;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var element = (UpdatableElement)value;
            var jsonObject = (JObject)JsonConvert.DeserializeObject(element.SerializedValue);

            //element.SerializedValue
            writer.WriteStartObject();
            foreach (var property in jsonObject.Properties())
            {
                writer.WritePropertyName(property.Name);

                if (UpdatableElement.LastUpdatedFieldName.Equals(property.Name))
                {
                    // let the serializer serialize the value itself
                    serializer.Serialize(writer, element.lastUpdated);
                }
                else
                {
                    // let the serializer serialize the value itself
                    serializer.Serialize(writer, property.Value);
                }
            }
            writer.WriteEndObject();
        }
    }
}