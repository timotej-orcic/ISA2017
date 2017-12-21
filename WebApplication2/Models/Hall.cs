using System;
using System.Collections.Generic;

namespace Isa2017Cinema.Models
{
    public enum SeatType { OPENED_FREE, OPENED_TAKEN, CLOSED}
    public class Hall
    {
        public Guid Id { get; set; }
        public Location ParentLocation { get; set; }
        public int RowsCount { get; set; }
        public int ColsCount { get; set; }
        public List<List<SeatType>> SeatsList { get; set; }
    }
}