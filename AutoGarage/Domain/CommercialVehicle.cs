using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoGarage.Domain;

public class CommercialVehicle : Vehicle
{
    public int TowingWeight { get; private set; }
    public override string LicensePlate
    {
        get { return base.LicensePlate; }
        set
        {
            if (!IsValidLicensePlate(value))
            {
                throw new ArgumentException(
                    "License plate for commercial vehicles must be 8 characters long, contain 2 hyphens, and start with 'V'.");
            }
            base.LicensePlate = value;
        }
    }

    public CommercialVehicle(int id, int ownerId, string description, string licensePlate, int towingWeight)
        : base(id, ownerId, description, licensePlate)
    {
        TowingWeight = towingWeight;
    }

    public override bool IsValidLicensePlate(string licensePlate)
    {
        if (!base.IsValidLicensePlate(licensePlate))
        {
            return false;
        }

        if (char.ToUpperInvariant(licensePlate[0]) != 'V')
        {
            return false;
        }

        return true;
    }
}
