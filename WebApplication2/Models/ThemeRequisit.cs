using System;

namespace Isa2017Cinema.Models
{
    public class ThemeRequisit
    {
        public Guid Id { get; set; }
        public String Name { get; set; }
        public String Description { get; set; }
        public byte[] Image { get; set; }
        public Double Price { get; set; }
        public int AvailableCount { get; set; }
    }
}