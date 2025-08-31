using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MotoNow.Domain.Entities
{
    public abstract class BaseEntity
    {
        public string Identifier { get; set; } = default!;
    }
}
