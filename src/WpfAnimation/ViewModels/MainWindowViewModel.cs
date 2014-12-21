namespace WpfAnimation.Demo.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Linq;
    using Catel.MVVM;
    using Models;

    public class MainWindowViewModel: ViewModelBase
    {
        public ObservableCollection<User> ListBoxItems { get; set; }
        private Random _random;
        private List<string> _userNames = new List<string>() { "John", "Steve", "Suzanne", "Alen", "Craig", "Rob", "Colin" };
        private List<string> _commitDescriptions = new List<string>()
        {
            "Added remove button",
            "Updated styles", 
            "Added list box animation",
            "Added remove button",
            "Updated NuGet packages",
            "Updated list box item template",
        };

        public MainWindowViewModel()
        {
            _random = new Random();

            ListBoxItems = new ObservableCollection<User>();
            for (int i = 0; i < 3; i++)
            {
                ListBoxItems.Add(CreateUser());
            }
            
            AddItemCommand = new Command(OnAddItemCommandExecute);
        }

        public User SelectedItem { get; set; }

        public Command AddItemCommand { get; private set; }
        private void OnAddItemCommandExecute()
        {
//            ListBoxItems.Add(DateTime.Now.Ticks.ToString());
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

        private User CreateUser()
        {
            var name = _userNames[_random.Next(_userNames.Count - 1)];
            var commitId = Guid.NewGuid().ToString().ToLower().Substring(0, 8);
            var commitDescription = _commitDescriptions[_random.Next(_userNames.Count - 1)];
            var date = DateTime.Today.Date - TimeSpan.FromDays(_random.Next(10));

            return new User(name, commitId, commitDescription, date);
        }
        #endregion
    }

}