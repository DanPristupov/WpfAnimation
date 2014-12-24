namespace WpfAnimation.Demo.Models
{
    using System;
    using System.Windows.Media.Imaging;

    public class Commit
    {
        #region Properties
        public string UserName { get; set; }
        public string CommitId { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public BitmapImage Avatar { get; set; }
        #endregion
    }
}