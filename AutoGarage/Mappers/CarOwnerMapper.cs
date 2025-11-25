using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoGarage.DTO;
using AutoGarage.Domain;

namespace AutoGarage.Mappers;

public static class CarOwnerMapper
{
    public static CarOwnerDTO ToDTO(this CarOwner owner)
    {
        return new CarOwnerDTO(
            owner.Id,
            owner.Name
        );
    }

    public static List<VehicleDTO> VehiclesToDTOs(this CarOwner owner)
    {
        List<VehicleDTO> vehicleDTOs = new List<VehicleDTO>();
        foreach (var vehicle in owner.Vehicles)
        {
            vehicleDTOs.Add(vehicle.ToDTO());
        }
        return vehicleDTOs;
    }

    public static CarOwner ToDomain(this CarOwnerDTO ownerDTO, List<Vehicle> vehicles)
    {
        return new CarOwner(
            ownerDTO.Id,
            ownerDTO.Name,
            vehicles
        );
    }
}