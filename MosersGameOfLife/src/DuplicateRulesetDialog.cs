// Add this class to your project in a new file named DuplicateRulesetDialog.xaml.cs
using System.Windows;

namespace MosersGameOfLife
{
    public partial class DuplicateRulesetDialog : Window
    {
        public enum DialogResult
        {
            UseExisting,
            CreateDuplicate,
            Cancel
        }

        public DialogResult Result { get; private set; } = DialogResult.Cancel;

        public DuplicateRulesetDialog(string existingRulesetName, string notation)
        {
            InitializeComponent();
            
            MessageText.Text = $"A ruleset with these exact rules already exists with the name '{existingRulesetName}'.\nRule pattern: {notation}";
            Title = "Duplicate Ruleset Found";
        }

        private void UseExisting_Click(object sender, RoutedEventArgs e)
        {
            Result = DialogResult.UseExisting;
            Close();
        }

        private void CreateDuplicate_Click(object sender, RoutedEventArgs e)
        {
            Result = DialogResult.CreateDuplicate;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Result = DialogResult.Cancel;
            Close();
        }
    }
}
