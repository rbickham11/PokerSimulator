using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PokerSimulator.Web.Models
{
    public class SimulationRequestModel
    {
        [Range(0, 51)]
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