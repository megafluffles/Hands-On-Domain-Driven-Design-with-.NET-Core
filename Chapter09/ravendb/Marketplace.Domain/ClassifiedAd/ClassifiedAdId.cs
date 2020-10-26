using System;
using Marketplace.Framework;

namespace Marketplace.Domain.ClassifiedAd
{
    public class ClassifiedAdId : Value<ClassifiedAdId>
    {
        public ClassifiedAdId(Guid value)
        {
            if (value == default)
                throw new ArgumentNullException(nameof(value), "Classified Ad id cannot be empty");

            Value = value;
        }

        public Guid Value { get; }

        public static implicit operator Guid(ClassifiedAdId self)
        {
            return self.Value;
        }

        public static implicit operator ClassifiedAdId(string value)
        {
            return new ClassifiedAdId(Guid.Parse(value));
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}