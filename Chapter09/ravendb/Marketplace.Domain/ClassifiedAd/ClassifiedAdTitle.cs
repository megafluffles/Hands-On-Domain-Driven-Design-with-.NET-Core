using System;
using System.Text.RegularExpressions;
using Marketplace.Framework;

namespace Marketplace.Domain.ClassifiedAd
{
    public class ClassifiedAdTitle : Value<ClassifiedAdTitle>
    {
        internal ClassifiedAdTitle(string value)
        {
            Value = value;
        }

        // Satisfy the serialization requirements 
        protected ClassifiedAdTitle()
        {
        }

        public string Value { get; private set; }

        public static ClassifiedAdTitle FromString(string title)
        {
            CheckValidity(title);
            return new ClassifiedAdTitle(title);
        }

        public static ClassifiedAdTitle FromHtml(string htmlTitle)
        {
            var supportedTagsReplaced = htmlTitle
                .Replace("<i>", "*")
                .Replace("</i>", "*")
                .Replace("<b>", "**")
                .Replace("</b>", "**");

            var value = Regex.Replace(supportedTagsReplaced, "<.*?>", string.Empty);
            CheckValidity(value);

            return new ClassifiedAdTitle(value);
        }

        public static implicit operator string(ClassifiedAdTitle title)
        {
            return title.Value;
        }

        private static void CheckValidity(string value)
        {
            if (value.Length > 100)
                throw new ArgumentOutOfRangeException(
                    "Title cannot be longer that 100 characters",
                    nameof(value));
        }
    }
}