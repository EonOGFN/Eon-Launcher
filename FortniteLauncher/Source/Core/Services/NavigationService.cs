using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using System;
using System.Collections.ObjectModel;

class NavigationService
{
    #region Classes
    public class Breadcrumb
    {
        public Breadcrumb(string label, Type page)
        {
            Label = label;
            Page = page;
        }
        public string Label { get; }

        public Type Page { get; }

        public override string ToString() => Label;

        public void NavigateToFromBreadcrumb(int BreadcrumbItemIndex)
        {
            NavigateInternal(Page, BreadcrumbItemIndex);
        }
    }
    #endregion

    #region Properties
    public static NavigationView MainNavigation { get; private set; }
    public static BreadcrumbBar MainBreadcrumb { get; private set; }

    public static Frame MainFrame { get; private set; }

    public static ObservableCollection<Breadcrumb> BreadCrumbs = new ObservableCollection<Breadcrumb>();
    #endregion

    #region Constructor
    public static void InitializeNavigationService(NavigationView navigationView, BreadcrumbBar breadcrumbBar, Frame frame)
    {
        MainNavigation = navigationView;
        MainBreadcrumb = breadcrumbBar;
        MainFrame = frame;
    }
    #endregion

    #region Private Functions
    private static void UpdateBreadcrumb()
    {
        MainBreadcrumb.ItemsSource = BreadCrumbs;
    }
    private static void NavigateInternal(Type page, int BreadcrumbBarIndex)
    {
        SlideNavigationTransitionInfo info = new SlideNavigationTransitionInfo
        {
            Effect = SlideNavigationTransitionEffect.FromLeft
        };
        MainFrame.Navigate(page, null, info);

        int indexToRemoveAfter = BreadcrumbBarIndex;

        if (indexToRemoveAfter < BreadCrumbs.Count - 1)
        {
            int itemsToRemove = BreadCrumbs.Count - indexToRemoveAfter - 1;

            for (int i = 0; i < itemsToRemove; i++)
            {
                BreadCrumbs.RemoveAt(indexToRemoveAfter + 1);
            }
        }
    }
    #endregion

    #region Public Functions
    public static void Navigate(Type TargetPageType, bool ClearNavigation)
    {
        if (ClearNavigation)
        {
            BreadCrumbs.Clear();
            MainFrame.BackStack.Clear();
        }

        UpdateBreadcrumb();
        ChangeBreadcrumbVisibility(false);

        SlideNavigationTransitionInfo info = new SlideNavigationTransitionInfo
        {
            Effect = SlideNavigationTransitionEffect.FromLeft
        };

        MainFrame.Navigate(TargetPageType, null, info);
    }


    public static void ChangeBreadcrumbVisibility(bool IsBreadcrumbVisible)
    {
        if (IsBreadcrumbVisible)
        {
            MainBreadcrumb.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
            MainNavigation.AlwaysShowHeader = true;
        }
        else
        {
            MainBreadcrumb.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
            MainNavigation.AlwaysShowHeader = false;
        }
    }
    #endregion
}