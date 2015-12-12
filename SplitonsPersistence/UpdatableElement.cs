using Newtonsoft.Json;

namespace SplitonsPersistence
{
    // ReSharper disable InconsistentNaming because ofserialisation Json.
    [JsonConverter(typeof (UpdatableElementConverter))]
    public struct UpdatableElement
    {
        public string SerializedValue { get; set; }
        public string id { get; set; }
        public long lastUpdated { get; set; }
    }
    // ReSharper restore InconsistentNaming
}