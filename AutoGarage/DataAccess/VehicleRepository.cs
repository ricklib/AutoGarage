using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoGarage.Domain;
using AutoGarage.DTO;
using AutoGarage.Mappers;
using Microsoft.Data.SqlClient;

namespace AutoGarage.DataAccess;

public class VehicleRepository
{
    private readonly string _connectionString =
        "Server=localhost\\SQLEXPRESS;Database=AutoGarage;Trusted_Connection=True;TrustServerCertificate=True\r\n";

    public VehicleRepository()
    {
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        using var connection = new SqlConnection(_connectionString);
        try
        {
            connection.Open();
            Console.WriteLine(
                "VehicleRepository - Successfully connected to database.");
        }
        catch (SqlException ex)
        {
            Console.WriteLine(
                "VehicleRepository - Failed to connect to database: " + ex.Message);
        }
    }

    public List<Vehicle> GetAllVehicles()
    {
        var vehicles = new List<Vehicle>();

        using var connection = new SqlConnection(_connectionString);
        try
        {
            connection.Open();

            string sql = "SELECT VID, Description, LicensePlate, TowingWeight, Commercial, OID FROM Vehicles";
            using var command = new SqlCommand(sql, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                int id = (int)reader["VID"];
                string? description = reader["Description"] as string;
                string? licensePlate = reader["LicensePlate"] as string;
                int? towingWeight = reader["TowingWeight"] != DBNull.Value ? (int?)reader["TowingWeight"] : null;
                bool commercial = (bool)reader["Commercial"];
                int ownerId = (int)reader["OID"];

                VehicleDTO vehicleDTO = new VehicleDTO(
                    id,
                    ownerId,
                    description,
                    licensePlate,
                    towingWeight,
                    commercial
                );

                vehicles.Add(vehicleDTO.ToDomain());
            }
        }
        catch (SqlException ex)
        {
            Console.WriteLine(
                "VehicleRepository - An error occured while trying to get vehicles: " + ex.Message);
        }

        return vehicles;
    }

    public void AddVehicle(Vehicle vehicle)
    {
        using var connection = new SqlConnection(_connectionString);
        try
        {
            connection.Open();
            string sql = "INSERT INTO Vehicles (Description, LicensePlate, TowingWeight, Commercial, OID) " +
                         "VALUES (@Description, @LicensePlate, @TowingWeight, @Commercial, @OID)";
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Description", vehicle.Description);
            command.Parameters.AddWithValue("@LicensePlate", vehicle.LicensePlate);
            if (vehicle is CommercialVehicle commercialVehicle)
            {
                command.Parameters.AddWithValue("@TowingWeight", commercialVehicle.TowingWeight);
                command.Parameters.AddWithValue("@Commercial", true);
            }
            else
            {
                command.Parameters.AddWithValue("@TowingWeight", DBNull.Value);
                command.Parameters.AddWithValue("@Commercial", false);
            }
            command.Parameters.AddWithValue("@OID", vehicle.OwnerId);
            command.ExecuteNonQuery();
        }
        catch (SqlException ex)
        {
            Console.WriteLine("VehicleRepository - An error occured while trying to add a vehicle: " + ex.Message);
        }
    }

    public bool UpdateVehicleLicensePlate(int vehicleId, string newLicensePlate)
    {
        using var connection = new SqlConnection(_connectionString);
        try
        {
            connection.Open();
            string sql = "UPDATE Vehicles SET LicensePlate = @LicensePlate WHERE VID = @VID";
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@LicensePlate", newLicensePlate);
            command.Parameters.AddWithValue("@VID", vehicleId);
            int rowsAffected = command.ExecuteNonQuery();
            return rowsAffected > 0;
        }
        catch (SqlException ex)
        {
            Console.WriteLine("VehicleRepository - An error occured while trying to update license plate: " + ex.Message);
            return false;
        }
    }

    public bool DeleteVehicle(int vehicleId)
    {
        using var connection = new SqlConnection(_connectionString);
        try
        {
            connection.Open();
            string sql = "DELETE FROM Vehicles WHERE VID = @VID";
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@VID", vehicleId);
            int rowsAffected = command.ExecuteNonQuery();
            return rowsAffected > 0;
        }
        catch (SqlException ex)
        {
            Console.WriteLine("VehicleRepository - An error occured while trying to delete vehicle: " + ex.Message);
            return false;
        }
    }
    
    public bool VehicleExists(int vehicleId)
    {
        using var connection = new SqlConnection(_connectionString);
        try
        {
            connection.Open();
            string sql = "SELECT COUNT(1) FROM Vehicles WHERE VID = @VID";
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@VID", vehicleId);
            int count = (int)command.ExecuteScalar();
            return count > 0;
        }
        catch (SqlException ex)
        {
            Console.WriteLine(
                "VehicleRepository - An error occured while trying to check vehicle existence: " + ex.Message);
            return false;
        }
    }

    public Vehicle GetVehicleById(int vehicleId)
    {
        using var connection = new SqlConnection(_connectionString);
        try
        {
            connection.Open();
            string sql = "SELECT * FROM Vehicles WHERE VID = @VID";
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@VID", vehicleId);
            using var reader = command.ExecuteReader();

            if (!reader.Read())
            {
                throw new ArgumentException($"Vehicle with id {vehicleId} was not found.");
            }

            int id = (int)reader["VID"];
            string? description = reader["Description"] as string;
            string? licensePlate = reader["LicensePlate"] as string;
            int? towingWeight = reader["TowingWeight"] != DBNull.Value ? (int?)reader["TowingWeight"] : null;
            bool commercial = (bool)reader["Commercial"];
            int ownerId = (int)reader["OID"];

            var vehicle = new VehicleDTO(
                id,
                ownerId,
                description,
                licensePlate,
                towingWeight,
                commercial
            ).ToDomain();

            return vehicle;
        }
        catch (SqlException ex)
        {
            Console.WriteLine(
                "VehicleRepository - An error occured while tring to get vehicle by id: " + ex.Message);
            throw;
        }
    }
}
