using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoGarage.Domain;
using AutoGarage.DTO;
using AutoGarage.Mappers;
using Microsoft.Data.SqlClient;

namespace AutoGarage.DAL;

public class CarOwnerRepository
{
    private readonly string _connectionString =
        "Server=localhost\\SQLEXPRESS;Database=AutoGarage;Trusted_Connection=True;TrustServerCertificate=True\r\n";
    private VehicleRepository _vehicleRepo = new VehicleRepository();

    public CarOwnerRepository()
    {
    }

    public void InitializeDatabase()
    {
        using var connection = new SqlConnection(_connectionString);
        try
        {
            connection.Open();
        }
        catch (DbException ex)
        {
            Console.WriteLine(
                "CarOwnerRepository - Failed to connect to database: " + ex.Message);
        }
    }

    public List<CarOwner> GetAllCarOwners()
    {
        var carOwners = new List<CarOwner>();
        var vehicles = _vehicleRepo.GetAllVehicles();
        using var connection = new SqlConnection(_connectionString);

        try
        {
            connection.Open();
            string sql = "SELECT OID, Name FROM Owners";
            using var command = new SqlCommand(sql, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                int id = (int)reader["OID"];
                string? rawName = reader["Name"] as string;
                string name = string.IsNullOrWhiteSpace(rawName) ? "Unknown Owner" : rawName;

                CarOwnerDTO ownerDTO = new CarOwnerDTO(id, name);

                List<Vehicle> ownerVehicles = vehicles
                    .Where(v => v.OwnerId == id)
                    .ToList();

                carOwners.Add(ownerDTO.ToDomain(ownerVehicles));
            }
        }
        catch (DbException ex)
        {
            Console.WriteLine(
                "CarOwnerRepository - An error occured while trying to get vehicles: " + ex.Message);
        }

        return carOwners;
    }

    public void AddCarOwner(string name)
    {
        using var connection = new SqlConnection(_connectionString);
        try
        {
            connection.Open();
            string sql = "INSERT INTO Owners (Name) VALUES (@Name)";
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Name", name);
            command.ExecuteNonQuery();
        }
        catch (DbException ex)
        {
            Console.WriteLine(
                "CarOwnerRepository - An error occured while trying to add a car owner: " + ex.Message);
        }
    }

    public bool CarOwnerExists(int ownerId)
    {
        using var connection = new SqlConnection(_connectionString);
        try
        {
            connection.Open();
            string sql = "SELECT COUNT(1) FROM Owners WHERE OID = @OID";
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@OID", ownerId);
            int count = (int)command.ExecuteScalar();
            return count > 0;
        }
        catch (DbException ex)
        {
            Console.WriteLine(
                "CarOwnerRepository - An error occured while trying to check if car owner exists: " + ex.Message);
            return false;
        }
    }
}
