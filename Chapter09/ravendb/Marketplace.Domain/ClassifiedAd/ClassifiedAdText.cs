using Marketplace.Framework;

namespace Marketplace.Domain.ClassifiedAd
{
    public class ClassifiedAdText : Value<ClassifiedAdText>
    {
        internal ClassifiedAdText(string text)
        {
            Value = text;
        }

        // Satisfy the serialization requirements 
        protected ClassifiedAdText()
        {
        }

        public string Value { get; }

        public static ClassifiedAdText FromString(string text)
        {
            return new ClassifiedAdText(text);
        }

        public static implicit operator string(ClassifiedAdText text)
        {
            return text.Value;
        }
    }
}