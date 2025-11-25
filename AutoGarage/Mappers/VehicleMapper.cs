using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoGarage.DTO;
using AutoGarage.Domain;

namespace AutoGarage.Mappers;

public static class VehicleMapper
{
    public static VehicleDTO ToDTO(this Vehicle vehicle)
    {
        if (vehicle is CommercialVehicle commercialVehicle)
        {
            return new VehicleDTO(
                commercialVehicle.Id,
                commercialVehicle.OwnerId,
                commercialVehicle.Description,
                commercialVehicle.LicensePlate,
                commercialVehicle.TowingWeight,
                true
            );
        }
        else
        {
            return new VehicleDTO(
                vehicle.Id,
                vehicle.OwnerId,
                vehicle.Description,
                vehicle.LicensePlate,
                null,
                false
            );
        }
    }

    public static Vehicle ToDomain(this VehicleDTO vehicleDTO)
    {
        if (vehicleDTO.Commercial)
        {
            if (vehicleDTO.TowingWeight == null)
            {
                throw new ArgumentException("TowingWeight can't be null for commercial vehicles.");
            }
            return new CommercialVehicle(
                vehicleDTO.Id,
                vehicleDTO.OwnerId,
                vehicleDTO.Description ?? string.Empty,
                vehicleDTO.LicensePlate,
                vehicleDTO.TowingWeight.Value
            );
        }
        else
        {
            return new Vehicle(
                vehicleDTO.Id,
                vehicleDTO.OwnerId,
                vehicleDTO.Description ?? string.Empty,
                vehicleDTO.LicensePlate
            );
        }
    }
}
