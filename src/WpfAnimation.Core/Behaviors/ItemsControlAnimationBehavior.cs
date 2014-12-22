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
            ShowAddItemContainers(itemsControl, itemsSource);

            (itemsSource as INotifyCollectionChanged).CollectionChanged += (sender, args) =>
            {
                if (args.Action == NotifyCollectionChangedAction.Add)
                {
                    var index = args.NewStartingIndex;
                    foreach (var newItem in args.NewItems)
                    {
                        mirrorItemsSource.Insert(index, newItem);
                        index++;
                    }
                    ShowAddItemContainers(itemsControl, args.NewItems);
                }
                else if (args.Action == NotifyCollectionChangedAction.Remove)
                {
                    foreach (var oldItem in args.OldItems)
                    {
                        var container = itemsControl.ItemContainerGenerator.ContainerFromItem(oldItem) as UIElement;
                        var removeItemAnimation = GetRemoveItemAnimation(itemsControl);
                        if (container != null && removeItemAnimation != null)
                        {
                            Storyboard.SetTarget(removeItemAnimation, container);

                            EventHandler onAnimationCompleted = null;
                            onAnimationCompleted = ((sender2, args2) =>
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
            };
        }

        private static IList CreateMirrorItemsSource(IList itemsSource)
        {
            var listType = typeof(ObservableCollection<>).MakeGenericType(itemsSource.GetType().GetGenericArguments()[0]);

            return (IList)Activator.CreateInstance(listType);
        }

        private static void ShowAddItemContainers(ItemsControl itemsControl, IList newItems)
        {
            EventHandler statusChanged = null;
            statusChanged = new EventHandler(delegate
            {
                var showAnimationForMultipleItems = false;
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
            });
            itemsControl.ItemContainerGenerator.StatusChanged += statusChanged;
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