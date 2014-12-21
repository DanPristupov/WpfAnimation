namespace WpfAnimation.Demo.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Catel.MVVM;
    using Models;

    public class MainWindowViewModel: ViewModelBase
    {
        public ObservableCollection<Commit> ListBoxItems { get; set; }
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

            ListBoxItems = new ObservableCollection<Commit>();
            for (int i = 0; i < 3; i++)
            {
                ListBoxItems.Add(CreateUser());
            }
            
            AddItemCommand = new Command(OnAddItemCommandExecute);
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
                UserName = _userNames[_random.Next(_userNames.Count - 1)],
                Description = _commitDescriptions[_random.Next(_userNames.Count - 1)],
                Date = DateTime.Today.Date - TimeSpan.FromDays(_random.Next(10)),
            };
        }
        #endregion
    }

}