using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
/**
 * Name: Sean Stille
 * Description: Lab 2
 * Date: 10/9/2022
 * Git Repo:    https://github.com/Sean-Stille/Lab3
 * Reflection:  I ran into some issues with this lab. The SQL portion went very smoothly, but while working on other portions I
 *              began to experience some issues with visual studio. It began telling me that it couldn't deploy due to file permissions.
 *              I had to redownload the lab2solution to get this issue to go away, I'm still unsure what was causing it, but I have heard other
 *              students have had this issue. 
 *              The sorting toggle took a bit of messing around with, I ended up changing the interfaces to incorporate additional methods.
 *              I chose to create comparators that would resort these lists, however apparently the ObservableCollections don't utilize
 *              comparators, so I had to convert them to lists and then back to observablecollections. Due to how getEntries() is set up,
 *              my solution required that getEntries get called after every button press, otherwise the list would desync.
 *              
 * Bugs:        I'm not sure what the initial bug was, however I put try and catch blocks on the mainpage.xaml.cs button methods, this way an empty
 *              box won't crash things. I also opted to get rid of the answer and clue placeholder text, they seemed a bit random.
 * */
namespace Lab2Solution
{

    /// <summary>
    /// Handles the BusinessLogic
    /// </summary>
    public class BusinessLogic : IBusinessLogic
    {
        const int MAX_CLUE_LENGTH = 250;
        const int MAX_ANSWER_LENGTH = 21;
        const int MAX_DIFFICULTY = 5;
        int latestId = 0;
        bool isClueSort = false;

        IDatabase db;                     // the actual database that does the hardwork

        public BusinessLogic()
        {
            db = new RelationalDatabase(); // new RelationalDatabase();           // 
        }


        /// <summary>
        /// Represents all entries
        /// This also could have been a property
        /// </summary>
        /// <returns>ObservableCollection of entrties</returns>
        public ObservableCollection<Entry> GetEntries()
        {
            return db.GetEntries();
        }

        public Entry FindEntry(int id)
        {
            return db.FindEntry(id);
        }

        /// <summary>
        /// Verifies that all the entry fields are valied
        /// </summary>
        /// <param name="clue"></param>
        /// <param name="answer"></param>
        /// <param name="difficulty"></param>
        /// <param name="date"></param>
        /// <returns>an error if there is an error, InvalidFieldError.None otherwise</returns>

        private InvalidFieldError CheckEntryFields(string clue, string answer, int difficulty, string date)
        {
            if (clue.Length < 1 || clue.Length > MAX_CLUE_LENGTH)
            {
                return InvalidFieldError.InvalidClueLength;
            }
            if (answer.Length < 1 || answer.Length > MAX_ANSWER_LENGTH)
            {
                return InvalidFieldError.InvalidAnswerLength;
            }
            if (difficulty < 0 || difficulty > MAX_DIFFICULTY)
            {
                return InvalidFieldError.InvalidDifficulty;
            }

            return InvalidFieldError.NoError;
        }


        /// <summary>
        /// Adds an entry
        /// </summary>
        /// <param name="clue"></param>
        /// <param name="answer"></param>
        /// <param name="difficulty"></param>
        /// <param name="date"></param>
        /// <returns>an error if there is an error, InvalidFieldError.None otherwise</returns>
        public InvalidFieldError AddEntry(string clue, string answer, int difficulty, string date)
        {

            var result = CheckEntryFields(clue, answer, difficulty, date);
            if (result != InvalidFieldError.NoError)
            {
                return result;
            }
            Entry entry = new Entry(clue, answer, difficulty, date, ++latestId);
            db.AddEntry(entry);

            return InvalidFieldError.NoError;
        }

        /// <summary>
        /// Deletes an entry
        /// </summary>
        /// <param name="entryId"></param>
        /// <returns>an erreor if there is one, EntryDeletionError.NoError otherwise</returns>
        public EntryDeletionError DeleteEntry(int entryId)
        {
            try
            {

                var entry = db.FindEntry(entryId);

                if (entry != null)
                {
                    bool success = db.DeleteEntry(entry);
                    if (success)
                    {
                        return EntryDeletionError.NoError;

                    }
                    else
                    {
                        return EntryDeletionError.DBDeletionError;
                    }
                }
                else
                {
                    return EntryDeletionError.EntryNotFound;
                }
            }
            catch (Exception e)
            {
                return EntryDeletionError.DBDeletionError;
            }
        }

        /// <summary>
        /// Edits an Entry
        /// </summary>
        /// <param name="clue"></param>
        /// <param name="answer"></param>
        /// <param name="difficulty"></param>
        /// <param name="date"></param>
        /// <param name="id"></param>
        /// <returns>an error if there is one, EntryEditError.None otherwise</returns>
        public EntryEditError EditEntry(string clue, string answer, int difficulty, string date, int id)
        {
            try
            {
                var fieldCheck = CheckEntryFields(clue, answer, difficulty, date);
                if (fieldCheck != InvalidFieldError.NoError)
                {
                    return EntryEditError.InvalidFieldError;
                }

                var entry = db.FindEntry(id);
                entry.Clue = clue;
                entry.Answer = answer;
                entry.Difficulty = difficulty;
                entry.Date = date;

                bool success = db.EditEntry(entry);
                if (!success)
                {
                    return EntryEditError.DBEditError;
                }

                return EntryEditError.NoError;
            }
            catch(Exception e)
            {
                //if error is not selected
                return EntryEditError.InvalidFieldError;
            }
        }

        /**
         * Comparator for ordering the list via clues
         * Uses default String comparison
         */
        class ClueSort : IComparer<Entry>
        {
            public int Compare(Entry x, Entry y)
            {

                // CompareTo() method
                return x.Clue.CompareTo(y.Clue);

            }
        }

        /**
         * Comparator for ordering the list via answers
         * Uses default String comparison
         */
        class AnswerSort : IComparer<Entry>
        {
            public int Compare(Entry x, Entry y)
            {

                // CompareTo() method
                return x.Answer.CompareTo(y.Answer);

            }
        }

        /**
         * Method that handles switching between sorts on the entry list
         * Essentially it creates two comparators using the classes above, then identifies which is being used.
         * After that, it switches to the other comparator.
         */
        public ObservableCollection<Entry> ToggleSort(ObservableCollection<Entry> entries)
        {
            ClueSort clue = new ClueSort();
            AnswerSort answer = new AnswerSort();

            List<Entry> sortingList = entries.ToList<Entry>();        //Comparators don't seem to work on ObservableCollections
            ObservableCollection<Entry> sortedList = new ObservableCollection<Entry>();
            if ( isClueSort)
            {
                sortingList.Sort(answer);
                isClueSort = false;
            }
            else
            {
                sortingList.Sort(clue);
                isClueSort = true;
            }

            foreach (Entry entry in sortingList)
            {
                sortedList.Add(entry);
            }
            

            return sortedList;
        }
    }


}
