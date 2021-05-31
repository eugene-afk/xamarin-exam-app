using SQLite;

namespace ExamApp.Data.Model
{
    [Table("CONFIG")]
    public class Config
    {
        public bool signed { get; set; } = false;
        public int userID { get; set; } = 0;
    }
}
