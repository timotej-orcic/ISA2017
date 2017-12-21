using System;

namespace Isa2017Cinema.Models
{
    public class Licitation
    {
        public Guid Id { get; set; }
        public Post ParentPost { get; set; }
        public Double OfferedPrice { get; set; }
    }
}