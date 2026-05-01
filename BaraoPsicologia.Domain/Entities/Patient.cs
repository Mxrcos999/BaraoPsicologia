using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaraoPsicologia.Domain.Entities
{
    public class Patient : EntityBase
    {
        public required string Name { get; set; }
        public DateTime DateBirth { get; set; }
        public required string PhoneNumber { get; set; }
        public required string ParentName { get; set; }
        public required string ParentDegree { get; set;}

    }
}
