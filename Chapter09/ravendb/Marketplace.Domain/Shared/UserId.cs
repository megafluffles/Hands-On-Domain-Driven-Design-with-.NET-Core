using System;
using Marketplace.Framework;

namespace Marketplace.Domain.Shared
{
    public class UserId : Value<UserId>
    {
        public UserId(Guid value)
        {
            if (value == default)
                throw new ArgumentNullException(nameof(value), "User id cannot be empty");

            Value = value;
        }

        public Guid Value { get; }

        public static implicit operator Guid(UserId self)
        {
            return self.Value;
        }
    }
}