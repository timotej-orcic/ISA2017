using System;
using System.Collections.Generic;

namespace Isa2017Cinema.Models
{
    public enum Type { GOLDEN, SILVER, BRONZE }
    public class RegUser : User
    {
        public Double Points { get; set; }
        public Type UserType { get; set; }
        public List<RegUser> FriendList { get; set; }
        public List<RegUser> RequestsList { get; set; }
        public List<Ticket> ReservationsList { get; set; }
        public List<Recension> RecensionList { get; set; }
        public List<Post> PostsList { get; set; }
    }
}