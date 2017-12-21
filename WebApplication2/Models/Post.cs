using System;
using System.Collections.Generic;

namespace Isa2017Cinema.Models
{
    public class Post
    {
        public Guid Id { get; set; }
        public String Name { get; set; }
        public String Description { get; set; }
        public DateTime OfferExpireDate { get; set; }
        public byte[] Image { get; set; }
        public List<Licitation> LicitationsList { get; set; }
    }
}