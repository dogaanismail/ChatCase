using System;

namespace ChatCase.Core.Entities
{
    /// <summary>
    /// IEntity interface
    /// </summary>
    public interface IEntity
    {
    }

    /// <summary>
    /// IEntity interface by generic TKey
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public interface IEntity<out TKey> : IEntity where TKey : IEquatable<TKey>
    {
        public TKey Id { get; }
        DateTime CreatedAt { get; set; }
    }
}
