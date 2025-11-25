using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoGarage.Domain;

public class Vehicle
{
    public int Id { get; private set; }
    public int OwnerId { get; private set; }
    private string? _description;
    public string? Description
    {
        get { return _description; }
        set
        {
            if (value != null && value.Length > 500)
            {
                throw new ArgumentException("Description cannot be longer than 500 characters.");
            }
            _description = value;
        }
    }

    private string _licensePlate;
    public virtual string LicensePlate
    {
        get { return _licensePlate; }
        set
        {
            if (!IsValidLicensePlate(value))
            {
                throw new ArgumentException(
                    "License plate must be 8 characters long and contain 2 hyphens.");
            }

            _licensePlate = value;
        }
    }

    public Vehicle(int id, int ownerId, string description, string licensePlate)
    {
        if (!IsValidLicensePlate(licensePlate))
        {
            throw new ArgumentException(
                "License plate must be 8 characters long and contain 2 hyphens.");
        }

        Id = id;
        OwnerId = ownerId;
        Description = description;
        _licensePlate = licensePlate;
    }

    public virtual bool IsValidLicensePlate(string licensePlate)
    {
        if (licensePlate.Length != 8)
        {
            return false;
        }
        int hyphenCount = 0;
        for (int i = 0; i < licensePlate.Length; i++)
        {
            if (licensePlate[i] == '-')
            {
                hyphenCount++;
            }
        }
        return hyphenCount == 2;
    }
}
