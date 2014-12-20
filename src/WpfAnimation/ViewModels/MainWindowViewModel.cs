namespace WpfAnimation.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using Catel.MVVM;

    public class MainWindowViewModel: ViewModelBase
    {
        public ObservableCollection<string> ListBoxItems { get; set; }

        public MainWindowViewModel()
        {
            ListBoxItems = new ObservableCollection<string>(){"item1", "item2", "item3"};
            AddItemCommand = new Command(OnAddItemCommandExecute);
        }

        public string SelectedItem { get; set; }

        public Command AddItemCommand { get; private set; }
        private void OnAddItemCommandExecute()
        {
            ListBoxItems.Add(DateTime.Now.Ticks.ToString());
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