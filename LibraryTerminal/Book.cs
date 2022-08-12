using System;
namespace LibraryTerminal
{
	public class Book
	{
        //title, author, status, and due date

        public string Title { get; set; }
        public string Author { get; set; }
        public bool IsCheckedOut { get; set; }  // status property
        public DateOnly DueDate { get; set; }

        
        public Book(string title, string author, bool isCheckedOut, DateOnly dueDate)
		{
            Title = title;
            Author = author;
            IsCheckedOut = isCheckedOut;
            DueDate = dueDate;
		}
	}
}

