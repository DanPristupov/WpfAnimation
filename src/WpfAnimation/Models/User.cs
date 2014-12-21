namespace WpfAnimation.Models
{
    using System;

    public class User
    {
        public User(string name, string commitId, string commitDescription, DateTime date)
        {
            Name = name;
            CommitId = commitId;
            CommitDescription = commitDescription;
            Date = date;
        }

        #region Properties
        public string Name { get; set; }
        public string CommitId { get; set; }
        public string CommitDescription { get; set; }
        public DateTime Date { get; set; }
        #endregion
    }
}