
using System.Collections.Generic;
namespace Lab2Solution
{

    public partial class MainPage : ContentPage
    {
        
        public MainPage()
        {
            InitializeComponent();
            EntriesLV.ItemsSource = MauiProgram.ibl.GetEntries();
        }

        void AddEntry(System.Object sender, System.EventArgs e)
        {
            try
            {
                String clue = clueENT.Text;
                String answer = answerENT.Text;
                String date = dateENT.Text;

                int difficulty;
                bool validDifficulty = int.TryParse(difficultyENT.Text, out difficulty);
                if (validDifficulty)
                {
                    InvalidFieldError invalidFieldError = MauiProgram.ibl.AddEntry(clue, answer, difficulty, date);
                    EntriesLV.ItemsSource = MauiProgram.ibl.GetEntries();
                    if (invalidFieldError != InvalidFieldError.NoError)
                    {
                        DisplayAlert("An error has occurred while adding an entry", $"{invalidFieldError}", "OK");
                    }
                }
                else
                {
                    DisplayAlert("Difficulty", $"Please enter a valid number", "OK");
                }
            }
            catch(Exception nullException)
            {

            }
        }

        void DeleteEntry(System.Object sender, System.EventArgs e)
        {
            try
            {
                Entry selectedEntry = EntriesLV.SelectedItem as Entry;
                try
                {
                    EntryDeletionError entryDeletionError = MauiProgram.ibl.DeleteEntry(selectedEntry.Id);
                    EntriesLV.ItemsSource = MauiProgram.ibl.GetEntries();
                }
                catch (Exception ex)
                {

                    DisplayAlert("An error has occurred while deleting an entry", ex.Message, "OK");
                }
            }
            catch (Exception nullException)
            {

            }

        }

        void EditEntry(System.Object sender, System.EventArgs e)
        {
            try
            {
                Entry selectedEntry = EntriesLV.SelectedItem as Entry;
                selectedEntry.Clue = clueENT.Text;
                selectedEntry.Answer = answerENT.Text;
                selectedEntry.Date = dateENT.Text;


                int difficulty;
                bool validDifficulty = int.TryParse(difficultyENT.Text, out difficulty);
                if (validDifficulty)
                {
                    selectedEntry.Difficulty = difficulty;
                    Console.WriteLine($"Difficuilt is {selectedEntry.Difficulty}");
                    EntryEditError entryEditError = MauiProgram.ibl.EditEntry(selectedEntry.Clue, selectedEntry.Answer, selectedEntry.Difficulty, selectedEntry.Date, selectedEntry.Id);
                    EntriesLV.ItemsSource = MauiProgram.ibl.GetEntries();
                    if (entryEditError != EntryEditError.NoError)
                    {
                        DisplayAlert("An error has occurred while editing an entry", $"{entryEditError}", "OK");
                    }
                }
            }
            catch (System.NullReferenceException nullException)
            {

            }
        }
        
        void ToggleSort(System.Object sender, System.EventArgs e)
        {
            EntriesLV.ItemsSource = MauiProgram.ibl.ToggleSort(MauiProgram.ibl.GetEntries());
            
        }

        void EntriesLV_ItemSelected(System.Object sender, Microsoft.Maui.Controls.SelectedItemChangedEventArgs e)
        {
            Entry selectedEntry = e.SelectedItem as Entry;
            clueENT.Text = selectedEntry.Clue;
            answerENT.Text = selectedEntry.Answer;
            difficultyENT.Text = selectedEntry.Difficulty.ToString();
            dateENT.Text = selectedEntry.Date;

        }




    }
}

