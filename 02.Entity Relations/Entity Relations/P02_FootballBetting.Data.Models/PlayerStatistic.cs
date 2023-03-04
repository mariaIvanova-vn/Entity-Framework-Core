
using System.ComponentModel.DataAnnotations.Schema;

namespace P02_FootballBetting.Data.Models
{
    public class PlayerStatistic
    {
        [ForeignKey(nameof(Games))]
        public int GameId { get; set; }
        public virtual Game Games { get; set; } = null!;

        [ForeignKey(nameof(Players))]   
        public int PlayerId { get; set; }
        public Player Players { get; set; } = null!;

        public int ScoredGoals { get; set; }

        public int Assists { get; set; }

        public int MinutesPlayed { get; set; }
    }
}
