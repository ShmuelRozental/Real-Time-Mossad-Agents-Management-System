﻿using System.ComponentModel.DataAnnotations.Schema;

namespace Real_Time_Mossad_Agents_Management_System.Models
{
    public class Agent
    {
        public int Id { get; set; }
        public string? nickname { get; set; }

        public string? photoUrl { get; set; }
        public Location? Location { get; set; }
        public bool ActiveStatus { get; set; } = false;
    }
}
