using Isa2017Cinema.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication2.Models
{
    public class Graphic
    {
    }

    public class GraphicViewModel
    {
        public Location loc { get; set; }
        public List<Pair> pairs { get; set; }
        public List<Pair> nedeljni { get; set; }
        public List<Pair2> mesecni { get; set; }
        public string DatumOd { get; set; }
        public string DatumDo {get;set;}
    }

    public class Pair
    {
        public string date { get; set; }
        public int year { get; set; }
        public int numberofday { get; set; }
        public int number { get; set; }
    }

    public class Pair2
    {
        public string date { get; set; }
        public int year { get; set; }
        public int numberofday { get; set; }
        public int number { get; set; }
        public int numberofmonth { get; set; }
    }
}