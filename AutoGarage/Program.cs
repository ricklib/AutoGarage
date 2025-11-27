using System;
using System.Transactions;
using AutoGarage.DataAccess;
using AutoGarage.Domain;

internal class Program
{
    private static readonly VehicleRepository vehicleRepo = new VehicleRepository();
    private static readonly CarOwnerRepository ownerRepo = new CarOwnerRepository();

    private static void Main(string[] args)
    {
        Console.WriteLine("GARAGE MANAGEMENT SYSTEM");

        bool keepUsing = true;
        while (keepUsing)
        {
            Console.WriteLine("\n" + """
                Choose an option.
                --------------------------------
                [1] List all owners and vehicles
                [2] Add car owner
                [3] Add vehicle
                [4] Edit vehicle license plate
                [5] Delete vehicle
                [6] Exit program
                """);
            char input = Console.ReadKey().KeyChar;
            Console.Clear();

            switch (input)
            {
                case '1':
                    ListOwnersAndVehicles();
                    break;
                case '2':
                    AddCarOwner();
                    break;
                case '3':
                    AddVehicle();
                    break;
                case '4':
                    EditVehicleLicensePlate();
                    break;
                case '5':
                    DeleteVehicle();
                    break;
                case '6':
                    keepUsing = false;
                    break;
                default:
                    Console.WriteLine("\nInvalid input. Please try again.");
                    break;
            }
        }
    }

    private static void ListOwnersAndVehicles()
    {
        Console.WriteLine("\n");
        List<CarOwner> owners = ownerRepo.GetAllCarOwners();

        if (owners.Count == 0)
        {
            Console.WriteLine("No owners found.");
            return;
        }

        foreach (var owner in owners)
        {
            Console.WriteLine($"Owner: {owner.Name} (ID: {owner.Id})");
            foreach (var vehicle in owner.Vehicles)
            {
                Console.WriteLine(
                    $"\tDescription: {vehicle.Description}, License Plate: {vehicle.LicensePlate} (ID: {vehicle.Id})");
            }
            Console.WriteLine("");
        }
    }

    private static void AddCarOwner()
    {
        Console.WriteLine("\nEnter a name for the new owner. (Max 100 characters) (Enter X to exit)");
        string name = GetNonNullInput();

        while (name.Length > 100 || name.Trim().Length == 0)
        {
            Console.WriteLine("Name cannot be longer than 100 characters or empty. Please try again or enter X to exit.");
            name = GetNonNullInput();
        }

        if (name.Trim() == "x" || name.Trim() == "X") return;

        ownerRepo.AddCarOwner(name);
    }

    private static void AddVehicle()
    {
        Console.WriteLine("\nCurrent owners:");
        var owners = ownerRepo.GetAllCarOwners();
        foreach (var o in owners)
        {
            Console.WriteLine($"\t{o.Id}: {o.Name}");
        }

        Console.WriteLine("\nEnter the ID of the owner for the new vehicle. (Enter X to exit)");

        int ownerId;
        while (true)
        {
            string input = GetNonNullInput().Trim();

            if (input == "x" || input == "X") return;

            if (!int.TryParse(input, out ownerId))
            {
                Console.WriteLine("Invalid input. Enter a number for owner ID or enter X to exit.");
                continue;
            }

            if (!ownerRepo.CarOwnerExists(ownerId))
            {
                Console.WriteLine($"No owner found for ID {ownerId}. Please try again or enter X to exit.");
                continue;
            }

            break;
        }

        Console.WriteLine("Enter vehicle description (or X to cancel).");
        string description;
        while (true)
        {
            description = GetNonNullInput();
            if (description.Length > 500)
            {
                Console.WriteLine("Description cannot be longer than 500 characters. Please try again or enter X to cancel.");
                continue;
            }
            break;
        }
        if (description.Trim() == "x" || description.Trim() == "X") return;

        Console.WriteLine("Is the vehicle commercial? (y/n) (or enter X to cancel)");
        bool commercial;
        while (true)
        {
            string commInput = GetNonNullInput().Trim().ToLower();
            if (commInput == "x") return;
            else if (commInput == "y")
            {
                commercial = true;
                break;
            }
            else if (commInput == "n")
            {
                commercial = false;
                break;
            }
            else
            {
                Console.WriteLine("Invalid input. Enter 'y' for yes, 'n' for no, or 'X' to cancel.");
            }
        }
            
        if (commercial)
        {
            string license;
            Console.WriteLine("Enter license plate. (example: v1-2b-c3) (commercial license plate must start with 'v') (or enter X to cancel)");
            while (true)
            {
                license = GetNonNullInput();
                if (license.Trim() == "x" || license.Trim() == "X") return;

                try
                {
                    var v = new CommercialVehicle(0, ownerId, description, license, 0); // To test validity
                    break;
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"Invalid license plate: {ex.Message} Try again or enter X to cancel.");
                }
            }

            Console.WriteLine("Enter towing weight in kg. (or enter X to cancel)");
            int towingWeight;
            while (true)
            {
                string towInput = GetNonNullInput().Trim();
                if (towInput.Trim() == "x" || towInput.Trim() == "X") return;
                if (!int.TryParse(towInput, out towingWeight) || towingWeight < 0)
                {
                    Console.WriteLine("Invalid input. Enter a positive number for towing weight or X to cancel.");
                    continue;
                }
                break;
            }

            var commercialVehicle = new CommercialVehicle(0, ownerId, description, license, towingWeight);
            vehicleRepo.AddVehicle(commercialVehicle);
        }
        else
        {
            string license;
            Console.WriteLine("Enter license plate. (example: a1-2b-c3) (or enter X to cancel)");
            while (true)
            {
                license = GetNonNullInput();
                if (license.Trim() == "x" || license.Trim() == "X") return;

                try
                {
                    var v = new Vehicle(0, ownerId, description, license); // To test validity
                    break;
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"Invalid license plate: {ex.Message} Try again or enter X to cancel.");
                }
            }

            var vehicle = new Vehicle(0, ownerId, description, license);
            vehicleRepo.AddVehicle(vehicle);
        }

        Console.WriteLine("Vehicle added successfully.");
    }

    private static void EditVehicleLicensePlate()
    {
        Console.WriteLine("\nCurrent list of owners and vehicles:");
        ListOwnersAndVehicles();

        Console.WriteLine("Enter the ID of the vehicle to edit. (or enter X to cancel)");
        int vehicleId;
        while (true)
        {
            string input = GetNonNullInput().Trim();
            if (input == "x" || input == "X") return;
            if (!int.TryParse(input, out vehicleId))
            {
                Console.WriteLine("Invalid input. Enter a number for vehicle ID or enter X to cancel.");
                continue;
            }
            if (!vehicleRepo.VehicleExists(vehicleId))
            {
                Console.WriteLine($"No vehicle found for ID {vehicleId}. Please try again or enter X to cancel.");
                continue;
            }
            break;
        }

        Vehicle vehicleToEdit = vehicleRepo.GetVehicleById(vehicleId);

        Console.WriteLine($"Current license plate: {vehicleToEdit.LicensePlate}");

        Console.WriteLine("Enter new license plate. (or enter X to cancel)");
        string newLicensePlate;
        while (true)
        {
            newLicensePlate = GetNonNullInput();
            if (newLicensePlate.Trim() == "x" || newLicensePlate.Trim() == "X") return;
            try
            {
                vehicleToEdit.LicensePlate = newLicensePlate;
                break;
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Invalid license plate: {ex.Message} Try again or enter X to cancel.");
            }
        }

        vehicleRepo.UpdateVehicleLicensePlate(vehicleId, newLicensePlate);
    }

    private static void DeleteVehicle()
    {
        Console.WriteLine("\nCurrent list of owners and vehicles:");
        ListOwnersAndVehicles();

        Console.WriteLine("Enter the ID of the vehicle to edit. (or enter X to cancel)");
        int vehicleId;
        while (true)
        {
            string input = GetNonNullInput().Trim();
            if (input == "x" || input == "X") return;
            if (!int.TryParse(input, out vehicleId))
            {
                Console.WriteLine("Invalid input. Enter a number for vehicle ID or enter X to cancel.");
                continue;
            }
            if (!vehicleRepo.VehicleExists(vehicleId))
            {
                Console.WriteLine($"No vehicle found for ID {vehicleId}. Please try again or enter X to cancel.");
                continue;
            }
            break;
        }

        Vehicle vehicleToDelete = vehicleRepo.GetVehicleById(vehicleId);
        Console.WriteLine("You are about to delete the following vehicle:");
        Console.WriteLine(
            $"Description: {vehicleToDelete.Description}, License Plate: {vehicleToDelete.LicensePlate} (ID: {vehicleToDelete.Id})");
        Console.WriteLine("Are you sure you want to delete this vehicle? (y/n)");
        while (true)
        {
            string confInput = GetNonNullInput().Trim().ToLower();
            if (confInput == "y")
            {
                break;
            }
            else if (confInput == "n")
            {
                return;
            }
            else
            {
                Console.WriteLine("Invalid input. Enter 'y' for yes, 'n' for no.");
            }
        }

        vehicleRepo.DeleteVehicle(vehicleId);

        Console.WriteLine("Vehicle deleted successfully.");
    }

    private static String GetNonNullInput()
    {
        string? input = Console.ReadLine();
        while (input == null || input.Trim() == "")
        {
            Console.WriteLine("Please enter a value.");
            input = Console.ReadLine();
        }
        return input;
    }
}