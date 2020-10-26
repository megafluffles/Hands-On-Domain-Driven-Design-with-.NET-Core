using System;
using Marketplace.Domain.Shared;
using Marketplace.Framework;

namespace Marketplace.Domain.UserProfile
{
    public class DisplayName : Value<DisplayName>
    {
        internal DisplayName(string displayName)
        {
            Value = displayName;
        }

        // Satisfy the serialization requirements
        protected DisplayName()
        {
        }

        public string Value { get; }

        public static DisplayName FromString(
            string displayName,
            CheckTextForProfanity hasProfanity)
        {
            if (displayName.IsEmpty())
                throw new ArgumentNullException(nameof(FullName));

            if (hasProfanity(displayName))
                throw new DomainExceptions.ProfanityFound(displayName);

            return new DisplayName(displayName);
        }

        public static implicit operator string(DisplayName displayName)
        {
            return displayName.Value;
        }
    }
}