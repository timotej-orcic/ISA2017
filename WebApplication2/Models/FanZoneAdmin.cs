using System.Collections.Generic;
using WebApplication2.Models;

namespace Isa2017Cinema.Models
{
    public class FanZoneAdmin : Admin
    {
        public FanZone MyFanzone { get; set; }
        public List<Post> PendingPostsList { get; set; }
    }
}