using DichVuGame.Areas.Admin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DichVuGame.Models.ViewModels
{
    public class GamesViewModel
    {
        public Game Game { get; set; }
        public int Amount { get; set; }
        public Code Code { get; set; }
        public List<Game> Games { get; set; }
        public List<GameTag> GameTags { get; set; }
        public Studio Studio { get; set; }
        public List<Studio> Studios { get; set; }
        public List<Country> Countries { get; set; }
        public SystemRequirement SystemRequirement { get; set; }
        public string Tags { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public List<Comment> Comments { get; set; }
        public List<GameReview> Reviews { get; set; }
        public bool WasBought { get; set; }
        public string ShareUrl { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}
