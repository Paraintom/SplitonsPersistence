using System;
using System.Linq;

namespace SplitonsPersistence
{
    // ReSharper disable InconsistentNaming because ofserialisation Json.
    public interface ITransaction
    {
        string id { get; set; }
        long lastUpdated { get; set; }
    }

    public struct Transaction : ITransaction
    {
       

        public string id { get; set; }
        public long lastUpdated { get; set; }
        public string from { get; set; }
        public string[] to { get; set; }
        public string comment { get; set; }
        public float amount { get; set; }
        public string currency { get; set; }
        public bool deleted { get; set; }

        public override string ToString()
        {
            return string.Format("Id: {0}, LastUpdated: {1}, From: {2}, To: {3}, Comment: {4}, Amount: {5}, Currency: {6}",
                id, lastUpdated, from, to.Length, comment, amount, currency);
        }

        #region Equality members
        public bool Equals(Transaction other)
        {
            return string.Equals(id, other.id) && lastUpdated == other.lastUpdated 
                && string.Equals(@from, other.@from) && string.Equals(comment, other.comment) 
                && amount.Equals(other.amount) && string.Equals(currency, other.currency) 
                && deleted.Equals(other.deleted)
                && to.ToList().All(o => other.to.Contains(o));
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Transaction && Equals((Transaction)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (id != null ? id.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ lastUpdated.GetHashCode();
                hashCode = (hashCode * 397) ^ (@from != null ? @from.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (to != null ? to.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (comment != null ? comment.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ amount.GetHashCode();
                hashCode = (hashCode * 397) ^ (currency != null ? currency.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ deleted.GetHashCode();
                return hashCode;
            }
        } 
        #endregion
    }
    // ReSharper restore InconsistentNaming
}
