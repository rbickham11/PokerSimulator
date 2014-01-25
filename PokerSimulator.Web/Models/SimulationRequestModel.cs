using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

using PokerSimulator.Lib;

namespace PokerSimulator.Web.Models
{
    public class SimulationRequestModel
    {
        public IEnumerable<int> SetHands { get; set; }
        [Required]
        [Display(Name=("Additional Random Hands"))]
        public int RandomHands { get; set; }
        public IEnumerable<int> SetBoard { get; set; }
        [Required] 
        [Display(Name="Hands to Simulate")]
        public int HandsToSimulate { get; set; }
        [Display(Name="Change random hands on each simulation?")]
        public bool RandomChange { get; set; }
    }
}