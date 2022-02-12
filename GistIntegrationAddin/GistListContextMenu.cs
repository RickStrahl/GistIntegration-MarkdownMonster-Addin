using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MarkdownMonster;
using MarkdownMonster.Windows;

namespace GistIntegration
{

    public class GistListContextMenu
    {
        private LoadGistWindow Window { get; }

        private ContextMenu ContextMenu = new ContextMenu();

        private AppModel Model;

        ListBoxItem SelectedListItem { get; set; }

        /// <summary>
        /// This allows adding/removing items on the context menu from a plug in
        /// </summary>
        public static event EventHandler<ContextMenu> ContextMenuOpening;

        public GistListContextMenu(LoadGistWindow window, ListBoxItem selectedListItem)
        {
            Window = window;
            Model = mmApp.Model;
            SelectedListItem = selectedListItem;
        }

        private void ContextMenu_Closed(object sender, RoutedEventArgs e)
        {
            ContextMenu.IsOpen = false;

            //Model.ActiveEditor?.SetEditorFocus();
            ClearMenu();
        }

        /// <summary>
        /// Clears all items off the menu
        /// </summary>
        public void ClearMenu()
        {
            if (ContextMenu == null)
            {
                ContextMenu = new ContextMenu();
                ContextMenu.Closed += ContextMenu_Closed;
            }
            else
                ContextMenu?.Items.Clear();
        }

        public void Show()
        {
            ContextMenuOpening?.Invoke(this, ContextMenu);

            if (ContextMenu != null)
            {
                ContextMenu.PlacementTarget =Window.ListGists;
                ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint;
                ContextMenu.VerticalOffset = 8;

                //Window.WebBrowser.ContextMenu = ContextMenu;
                Window.ListGists.ContextMenu = ContextMenu;

                var item = ContextMenu.Items[0] as MenuItem;

                ContextMenu.Focus();
                ContextMenu.IsOpen = true;

                Window.Activate();
                ContextMenu.Dispatcher.InvokeAsync(() => item.Focus(),
                    System.Windows.Threading.DispatcherPriority.ApplicationIdle);
            }

        }

        public void ShowContextMenu()
        {
            ClearMenu();


            var cm = ContextMenu;
            cm.Items.Clear();

            MenuItem mi;

            mi = new MenuItem()
            {
                Header="Embed Gist into Markdown"
            };
            mi.Click += async (s, e) =>
            {
                var gist = Window.ListGists.SelectedItem as GistItem;
                if (gist != null)
                    await Window.PasteGistToEditor(gist);
            };
            cm.Items.Add(mi);

            mi = new MenuItem()
            {
                Header = "Open Gist File(s) in Editor"
            };
            mi.Click += Window.ButtonOpen_Click;
            cm.Items.Add(mi);

            mi = new MenuItem()
            {
                Header = "Open Gist in Browser"
            };
            mi.Click += Window.ButtonBrowseToGist_Click;
            cm.Items.Add(mi);

            cm.Items.Add(new Separator());

            mi = new MenuItem()
            {
                Header = "Delete Gist on Gist Site"
            };
            mi.Click += Window.ButtonDeleteGist_Click;
            cm.Items.Add(mi);



            Show();
        }

    }
}
