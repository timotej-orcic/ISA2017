using System;
using System.Collections.Generic;

namespace Isa2017Cinema.Models
{
    public class FanZone
    {
        public Guid Id { get; set; }
        public List<ThemeRequisit> RequisitsList { get; set; }
        public List<Post> PostsList { get; set; }
    }
}