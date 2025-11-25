using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace AutoGarage.DTO;

public class CarOwnerDTO
{
    public int Id { get; private set; }
    public string Name { get; private set; }

    public CarOwnerDTO(int id, string name)
    {
        Id = id;
        Name = name;
    }
}
