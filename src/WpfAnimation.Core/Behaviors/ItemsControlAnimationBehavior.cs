namespace WpfAnimation
{
    using System;
    using System.Collections;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Media.Animation;

    public class ItemsControlAnimationBehavior
    {
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.RegisterAttached("ItemsSource",
                typeof(IList),
                typeof(ItemsControlAnimationBehavior),
                new UIPropertyMetadata(null, ItemsSourcePropertyChanged));

        public static void SetItemsSource(DependencyObject element, IList value)
        {
            element.SetValue(ItemsSourceProperty, value);
        }

        public static IList GetItemsSource(DependencyObject element)
        {
            return (IList)element.GetValue(ItemsSourceProperty);
        }

        private static void ItemsSourcePropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            // todo: unsubscribe from the oldValue
            var itemsControl = source as ItemsControl;
            var itemsSource = e.NewValue as IList;
            if (itemsControl == null)
            {
                return;
            }
            if (itemsSource == null)
            {
                itemsControl.ItemsSource = null;
                return;
            }

            var mirrorItemsSource = CreateMirrorItemsSource(itemsSource);
            itemsControl.SetBinding(ItemsControl.ItemsSourceProperty, new Binding { Source = mirrorItemsSource });

            foreach (var item in itemsSource)
            {
                mirrorItemsSource.Add(item);
            }

            (itemsSource as INotifyCollectionChanged).CollectionChanged += (sender, args) =>
            {
                if (args.Action == NotifyCollectionChangedAction.Add)
                {
                    AddItemWithAnimation(itemsControl, mirrorItemsSource, args.NewStartingIndex, args.NewItems);
                }
                else if (args.Action == NotifyCollectionChangedAction.Remove)
                {
                    RemoveItemWithAnimation(itemsControl, mirrorItemsSource, args.OldItems);
                }
            };
        }

        private static void AddItemWithAnimation(ItemsControl itemsControl, IList mirrorItemsSource, int startingIndex, IList newItems)
        {
            var index = startingIndex;
            foreach (var newItem in newItems)
            {
                WaitForContainerAndShowAddItemAnimation(itemsControl, newItem);
                mirrorItemsSource.Insert(index, newItem);
                index++;
            }
        }

        private static void WaitForContainerAndShowAddItemAnimation(ItemsControl itemsControl, object newItem)
        {
            EventHandler itemContainerGeneratorOnStatusChanged = null;
            itemContainerGeneratorOnStatusChanged = (sender, args) =>
            {
                if (itemsControl.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
                {
                    itemsControl.ItemContainerGenerator.StatusChanged -= itemContainerGeneratorOnStatusChanged;

                    var container = itemsControl.ItemContainerGenerator.ContainerFromItem(newItem) as UIElement;
                    var addItemAnimation = GetAddItemAnimation(itemsControl);
                    if (container != null && addItemAnimation != null)
                    {
                        Storyboard.SetTarget(addItemAnimation, container);
                        addItemAnimation.Begin();
                    }
                }
            };
            itemsControl.ItemContainerGenerator.StatusChanged += itemContainerGeneratorOnStatusChanged;
        }

        private static void RemoveItemWithAnimation(ItemsControl itemsControl, IList mirrorItemsSource, IList oldItems)
        {
            foreach (var oldItem in oldItems)
            {
                var container = itemsControl.ItemContainerGenerator.ContainerFromItem(oldItem) as UIElement;
                var removeItemAnimation = GetRemoveItemAnimation(itemsControl);
                if (container != null && removeItemAnimation != null)
                {
                    Storyboard.SetTarget(removeItemAnimation, container);

                    EventHandler removeItemAnimationOnCompleted = null;
                    removeItemAnimationOnCompleted = (sender, args) =>
                    {
                        removeItemAnimation.Completed -= removeItemAnimationOnCompleted;
                        mirrorItemsSource.Remove(oldItem);
                    };

                    removeItemAnimation.Completed += removeItemAnimationOnCompleted;
                    removeItemAnimation.Begin();
                }
                else
                {
                    mirrorItemsSource.Remove(oldItem);
                }
            }
        }

        private static IList CreateMirrorItemsSource(IList itemsSource)
        {
            var listType = typeof(ObservableCollection<>).MakeGenericType(itemsSource.GetType().GetGenericArguments()[0]);

            return (IList)Activator.CreateInstance(listType);
        }


        public static readonly DependencyProperty AddItemAnimationProperty =
            DependencyProperty.RegisterAttached("AddItemAnimation",
                typeof(Storyboard),
                typeof(ItemsControlAnimationBehavior),
                new UIPropertyMetadata(null));

        public static void SetAddItemAnimation(DependencyObject element, Storyboard value)
        {
            element.SetValue(AddItemAnimationProperty, value);
        }

        public static Storyboard GetAddItemAnimation(DependencyObject element)
        {
            return (Storyboard)element.GetValue(AddItemAnimationProperty);
        }

        public static readonly DependencyProperty RemoveItemAnimationProperty =
            DependencyProperty.RegisterAttached("RemoveItemAnimation",
                typeof(Storyboard),
                typeof(ItemsControlAnimationBehavior),
                new UIPropertyMetadata(null));

        public static void SetRemoveItemAnimation(DependencyObject element, Storyboard value)
        {
            element.SetValue(RemoveItemAnimationProperty, value);
        }

        public static Storyboard GetRemoveItemAnimation(DependencyObject element)
        {
            return (Storyboard)element.GetValue(RemoveItemAnimationProperty);
        }
    }
}