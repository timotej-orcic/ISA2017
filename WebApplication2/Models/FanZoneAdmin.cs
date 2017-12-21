using System.Collections.Generic;

namespace Isa2017Cinema.Models
{
    public class FanZoneAdmin : User
    {
        public List<Post> PendingPostsList { get; set; }
    }
}