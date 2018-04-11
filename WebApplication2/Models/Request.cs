using Isa2017Cinema.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication2.Models
{
    public class Request
    {
        public Guid Id { get; set; }
        public Guid Sender_id { get; set; }
        public string Sender_username { get; set; }
        public Guid Receiver_id { get; set; }
        public string Receiver_username { get; set; } 
    }
}