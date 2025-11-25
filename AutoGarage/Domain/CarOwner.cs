using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoGarage.Domain;

public class CarOwner
{
    public int Id { get; private set; }
    private string _name = string.Empty;
    public string Name
    {
        get { return _name; }
        private set
        {
            if (value.Trim().Length != 0 && value.Length <= 100)
            {
                _name = value;
            }
            else
            {
                throw new ArgumentException("Name cannot be empty or longer than 100 characters.");
            }
        }
    }
    public List<Vehicle> Vehicles { get; set; } = new List<Vehicle>();

    public CarOwner(int id, string name, List<Vehicle> vehicles)
    {
        Id = id;
        Name = name;
        Vehicles = vehicles;
    }

    public CarOwner(string name, List<Vehicle> vehicles)
    {
        Name = name;
        Vehicles = vehicles;
    }
}
