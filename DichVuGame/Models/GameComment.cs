using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DichVuGame.Models
{
    public class GameComment
    {
        public int GameID { get; set; }
        [ForeignKey("GameID")]
        public Game Game { get; set; }  
        public int CommentID { get; set; }
        [ForeignKey("CommentID")]
        public Comment Comment { get; set; }
    }
}
