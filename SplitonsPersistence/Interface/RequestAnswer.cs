using System.Collections.Generic;

namespace SplitonsPersistence.Interface
{
    // ReSharper disable InconsistentNaming because ofserialisation Json.
    public struct RequestMessage
    {
        public int id { get; set; }
        public SplitonSynchRequest request { get; set; }
    }
    public struct AnwserMessage
    {
        public int id { get; set; }
        public SplitonSynchRequest answer { get; set; }
    }

    public struct SplitonSynchRequest
    {
        public string projectId { get; set; }
        public long lastUpdated { get; set; }
        public List<UpdatableElement> toUpdate { get; set; }
    }
    // ReSharper restore InconsistentNaming
}
