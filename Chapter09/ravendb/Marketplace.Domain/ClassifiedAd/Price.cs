using System;
using Marketplace.Domain.Shared;

namespace Marketplace.Domain.ClassifiedAd
{
    public class Price : Money
    {
        private Price(decimal amount, string currencyCode, ICurrencyLookup currencyLookup)
            : base(amount, currencyCode, currencyLookup)
        {
            if (amount < 0)
                throw new ArgumentException(
                    "Price cannot be negative",
                    nameof(amount));
        }

        internal Price(decimal amount, string currencyCode)
            : base(amount, new Currency {CurrencyCode = currencyCode})
        {
        }

        // Satisfy the serialization requirements
        protected Price()
        {
        }

        public new static Price FromDecimal(decimal amount, string currency,
            ICurrencyLookup currencyLookup)
        {
            return new Price(amount, currency, currencyLookup);
        }
    }
}