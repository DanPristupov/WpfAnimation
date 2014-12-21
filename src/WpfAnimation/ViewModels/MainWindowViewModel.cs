namespace WpfAnimation.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Linq;
    using Catel.MVVM;
    using Models;

    public class MainWindowViewModel: ViewModelBase
    {
        public ObservableCollection<User> ListBoxItems { get; set; }

        public MainWindowViewModel()
        {
            ListBoxItems = new ObservableCollection<User>()
            {
                new User("John","2738sd67", "Added remove button", DateTime.Today.Date),
                new User("Steve","4s2374g5", "Added add button", DateTime.Today.Date),
                new User("Suzanne","129sre8j", "Added list box animation", DateTime.Today.Date),
            };
            AddItemCommand = new Command(OnAddItemCommandExecute);
        }

        public User SelectedItem { get; set; }

        public Command AddItemCommand { get; private set; }
        private void OnAddItemCommandExecute()
        {
//            ListBoxItems.Add(DateTime.Now.Ticks.ToString());
            ListBoxItems.Insert(0, new User("Dave", "dh37491d", "Updated list box item style", DateTime.Today.Date));
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

        #endregion
    }

}