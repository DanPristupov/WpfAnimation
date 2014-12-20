namespace WpfAnimation.ViewModels
{
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

        public Command AddItemCommand { get; private set; }

        private void OnAddItemCommandExecute()
        {
            ListBoxItems.Add("new Item");
        }
    }

}