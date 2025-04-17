using System.Windows;

namespace MosersGameOfLife.Views
{
    /// <summary>
    /// Dialog window for handling duplicate ruleset scenarios.
    /// </summary>
    public partial class DuplicateRulesetDialog : Window
    {
        /// <summary>
        /// Possible user actions when encountering a duplicate ruleset.
        /// </summary>
        public enum DialogResult
        {
            /// <summary>
            /// Use the existing ruleset instead of creating a duplicate.
            /// </summary>
            UseExisting,

            /// <summary>
            /// Create a duplicate ruleset despite identical rules.
            /// </summary>
            CreateDuplicate,

            /// <summary>
            /// Cancel the operation.
            /// </summary>
            Cancel
        }

        /// <summary>
        /// The result of the dialog interaction.
        /// </summary>
        public DialogResult Result { get; private set; } = DialogResult.Cancel;

        /// <summary>
        /// Initializes a new duplicate ruleset dialog.
        /// </summary>
        /// <param name="existingRulesetName">Name of the existing ruleset with identical rules</param>
        /// <param name="notation">Rule notation (e.g., "B3/S23")</param>
        public DuplicateRulesetDialog(string existingRulesetName, string notation)
        {
            InitializeComponent();

            MessageText.Text = $"A ruleset with these exact rules already exists with the name '{existingRulesetName}'.\nRule pattern: {notation}";
            Title = "Duplicate Ruleset Found";
        }

        /// <summary>
        /// Handles the "Use Existing" button click.
        /// </summary>
        private void UseExisting_Click(object sender, RoutedEventArgs e)
        {
            Result = DialogResult.UseExisting;
            Close();
        }

        /// <summary>
        /// Handles the "Create Duplicate" button click.
        /// </summary>
        private void CreateDuplicate_Click(object sender, RoutedEventArgs e)
        {
            Result = DialogResult.CreateDuplicate;
            Close();
        }

        /// <summary>
        /// Handles the "Cancel" button click.
        /// </summary>
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Result = DialogResult.Cancel;
            Close();
        }
    }
}
