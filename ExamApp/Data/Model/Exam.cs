using ExamApp.ViewModel;
using SQLite;

namespace ExamApp.Data.Model
{
    [Table("EXAMS")]
    public class Exam : Base
    {
        [PrimaryKey, AutoIncrement, Column("id")]
        public int id { get; set; }
        [Ignore]
        private string _name { get; set; }
        public string name 
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged("nameView"); }
        }
        public int ownerID { get; set; }
        [Ignore]
        private bool _isDraft { get; set; }
        public bool isDraft 
        {
            get { return _isDraft; }
            set 
            { 
                _isDraft = value;
                OnPropertyChanged();
                if (value)
                {
                    isCanPlay = false;
                    return;
                }
                isCanPlay = true; 
            }
        }

        [Ignore]
        private bool _isCanPlay { get; set; }
        [Ignore]
        public bool isCanPlay
        {
            get { return _isCanPlay; }
            set { _isCanPlay = value; OnPropertyChanged(); }
        }
        [Ignore]
        public bool isEditable
        {
            get 
            {
                if(ownerID != Common.CommonData.userID)
                {
                    return false;
                }
                return true;
            }
        }
        public bool isDeleted { get; set; } = false;

        [Ignore]
        private string _nameView { get; set; }
        [Ignore]
        public string nameView
        {
            get
            {
                _nameView = name;
                if (name.Length > 14)
                {
                    _nameView = name.Substring(0, 14) + "...";
                }
                return _nameView;
            }
            set
            {
                _nameView = value;
                OnPropertyChanged();
            }
        }

        public int playedTimes { get; set; }

    }
}
