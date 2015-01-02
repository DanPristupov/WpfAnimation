namespace WpfAnimation.Demo.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Windows.Media.Imaging;
    using Catel.MVVM;
    using Models;

    public class MainWindowViewModel: ViewModelBase
    {
        public ObservableCollection<Commit> ListBoxItems { get; set; }
        private Random _random;
        private List<BitmapImage> _avatars;
        private List<string> _userNames = new List<string>() { "John", "Steve", "Mike", "Suzanne", "Tom", "Alen", "Craig", "Rob", "Colin", "Jeff" };
        private List<string> _commitDescriptions = new List<string>()
        {
            "Update README.md",
            "Updated styles", 
            "Added list box animation",
            "Added remove button",
            "Fixed ribbon data context example",
            "Added build scripts",
            "Upgraded all external libraries",
            "Updated NuGet packages",
            "Updated list box item template",
        };

        public MainWindowViewModel()
        {
            _random = new Random();
            _avatars = GetAvatars();
            ListBoxItems = new ObservableCollection<Commit>();
            for (int i = 0; i < 3; i++)
            {
                ListBoxItems.Add(CreateUser());
            }
            
            AddItemCommand = new Command(OnAddItemCommandExecute);
        }

        private List<BitmapImage> GetAvatars()
        {
            var result = new List<BitmapImage>();

            foreach (var file in Directory.GetFiles(@"Resources\Avatars"))
            {
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                bitmapImage.UriSource = new Uri( file, UriKind.Relative );
                bitmapImage.EndInit();
                result.Add(bitmapImage);
            }

            return result;
        }

        public Commit SelectedItem { get; set; }

        public Command AddItemCommand { get; private set; }
        private void OnAddItemCommandExecute()
        {
            ListBoxItems.Insert(0, CreateUser());
        }

        #region DeleteListBoxItem command

        private Command _deleteListBoxItemCommand;

        /// <summary>
        /// Gets the DeleteListBoxItem command.
        /// </summary>
        public Command DeleteListBoxItemCommand
        {
            get { return _deleteListBoxItemCommand ?? (_deleteListBoxItemCommand = new Command(DeleteListBoxItem)); }
        }

        /// <summary>
        /// Method to invoke when the DeleteListBoxItem command is executed.
        /// </summary>
        private void DeleteListBoxItem()
        {
            ListBoxItems.Remove(SelectedItem);
        }

        private Commit CreateUser()
        {
            return new Commit
            {
                CommitId = Guid.NewGuid().ToString().ToLower().Substring(0, 8),
                UserName = _userNames[_random.Next(_userNames.Count)],
                Description = _commitDescriptions[_random.Next(_commitDescriptions.Count)],
                Date = DateTime.Today.Date - TimeSpan.FromDays(_random.Next(10)),
                Avatar = _avatars[_random.Next(0, _avatars.Count)],
            };
        }
        #endregion
    }

}