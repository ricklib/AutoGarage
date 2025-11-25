using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoGarage.DTO;

public class VehicleDTO
{
    public int Id { get; private set; }
    public int OwnerId { get; set; }
    public string? Description { get; private set; }
    public string LicensePlate { get; private set; }
    public int? TowingWeight { get; private set; }
    public bool Commercial { get; private set; }

    public VehicleDTO(int id, int ownerId, string? description, string licensePlate, int? towingWeight, bool commercial)
    {
        Id = id;
        OwnerId = ownerId;
        Description = description;
        LicensePlate = licensePlate;
        TowingWeight = towingWeight;
        Commercial = commercial;
    }
}
