using MyWay.Core.Common.Exceptions;

namespace MyWay.Core.Common.ValueObjects;

public sealed record ContactInfo
{
    public ContactInfo(string contactName, string phone, string? email = null)
    {
        if (string.IsNullOrWhiteSpace(contactName))
        {
            throw new DomainException("Contact name is required.");
        }

        if (string.IsNullOrWhiteSpace(phone))
        {
            throw new DomainException("Phone is required.");
        }

        ContactName = contactName.Trim();
        Phone = phone.Trim();
        Email = string.IsNullOrWhiteSpace(email) ? null : email.Trim();
    }

    public string ContactName { get; }

    public string Phone { get; }

    public string? Email { get; }
}
