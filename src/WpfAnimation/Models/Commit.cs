namespace WpfAnimation.Demo.Models
{
    using System;

    public class Commit
    {
        #region Properties
        public string UserName { get; set; }
        public string CommitId { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public string Avatar { get; set; }
        #endregion
    }
}