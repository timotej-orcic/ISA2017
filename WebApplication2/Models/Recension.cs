﻿using System;

namespace Isa2017Cinema.Models
{
    public class Recension
    {
        public Guid Id { get; set; }
        public Object RecensionObject { get; set; }
        public RegUser RecensionUser { get; set; }
        public Double Rating { get; set; }
    }
}