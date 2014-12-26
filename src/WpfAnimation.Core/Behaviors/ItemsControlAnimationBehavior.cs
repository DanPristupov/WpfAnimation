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
                    ShowAddItemAnimation(itemsControl, args.NewItems);
                    var index = args.NewStartingIndex;
                    foreach (var newItem in args.NewItems)
                    {
                        mirrorItemsSource.Insert(index, newItem);
                        index++;
                    }
                }
                else if (args.Action == NotifyCollectionChangedAction.Remove)
                {
                    ShowRemoveItemAnimation(itemsControl, args.OldItems, mirrorItemsSource);
                }
            };
        }

        private static void ShowAddItemAnimation(ItemsControl itemsControl, IList newItems)
        {
            EventHandler statusChanged = null;
            statusChanged = (sender, args) =>
            {
                var showAnimationForMultipleItems = false; // todo: add similar property to showRemoveItem
                if (itemsControl.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
                {
                    itemsControl.ItemContainerGenerator.StatusChanged -= statusChanged;

                    // Don't show animation if we have more than one item
                    if (newItems.Count > 1 && !showAnimationForMultipleItems)
                    {
                        return;
                    }

                    foreach (var newItem in newItems)
                    {
                        var container = itemsControl.ItemContainerGenerator.ContainerFromItem(newItem) as UIElement;
                        var addItemAnimation = GetAddItemAnimation(itemsControl);
                        if (container != null && addItemAnimation != null)
                        {
                            Storyboard.SetTarget(addItemAnimation, container);
                            addItemAnimation.Begin();
                        }
                    }
                }
            };
            itemsControl.ItemContainerGenerator.StatusChanged += statusChanged;
        }

        private static void ShowRemoveItemAnimation(ItemsControl itemsControl, IList oldItems, IList mirrorItemsSource)
        {
            foreach (var oldItem in oldItems)
            {
                var container = itemsControl.ItemContainerGenerator.ContainerFromItem(oldItem) as UIElement;
                var removeItemAnimation = GetRemoveItemAnimation(itemsControl);
                if (container != null && removeItemAnimation != null)
                {
                    Storyboard.SetTarget(removeItemAnimation, container);

                    EventHandler onAnimationCompleted = null;
                    onAnimationCompleted = ((sender, args) =>
                    {
                        removeItemAnimation.Completed -= onAnimationCompleted;
                        mirrorItemsSource.Remove(oldItem);
                    });

                    removeItemAnimation.Completed += onAnimationCompleted;
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