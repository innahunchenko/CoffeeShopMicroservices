﻿namespace Ordering.API.Domain.ValueObjects.AddressObjects
{
    public class Address
    {
        public string FirstName { get; private set; } = default!;
        public string LastName { get; private set; } = default!;
        public string? EmailAddress { get; private set; } = default!;
        public string AddressLine { get; private set; } = default!;
        public string Country { get; private set; } = default!;
        public string State { get; private set; } = default!;
        public string ZipCode { get; private set; } = default!;

        public static Address From(
            string firstName, string lastName, string? emailAddress,
            string addressLine, string country, string state, string zipCode)
        {
            return new Address
            {
                FirstName = firstName,
                LastName = lastName,
                EmailAddress = emailAddress,
                AddressLine = addressLine,
                Country = country,
                State = state,
                ZipCode = zipCode
            };
        }
    }
}
