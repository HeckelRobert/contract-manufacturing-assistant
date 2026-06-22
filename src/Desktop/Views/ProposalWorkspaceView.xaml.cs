namespace QuotationAccelerator.Desktop.Views;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

public partial class ProposalWorkspaceView
{
    public ProposalWorkspaceView()
    {
        InitializeComponent();
    }

    private void ExportMenuButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button || button.ContextMenu is not ContextMenu contextMenu)
        {
            return;
        }

        contextMenu.PlacementTarget = button;
        contextMenu.Placement = PlacementMode.Bottom;
        contextMenu.IsOpen = true;
    }
}
