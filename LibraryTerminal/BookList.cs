using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using LibraryTerminal;

namespace LibraryTerminal
{
    public class BookList
    {
        public List<Book> bookList = new List<Book>();

        string filePath = FindApplicationFile("ListOfBooksDB.txt").ToString();
        
        
        public void LoadBookList()
        {
            StreamReader sr = new(filePath);
            while(true)
            {
                string line = sr.ReadLine();
                if(line == null)
                {
                    break;
                }
                string[] entries = line.Split(',');
                bookList.Add(new Book(entries[0], entries[1], bool.Parse(entries[2]), DateOnly.Parse(entries[3])));
            }
            sr.Close();
        }


        private void SaveBookList()
        {
            List<string> outBookList = new();
            bookList = bookList.OrderBy(x => x.Author).ToList();
            foreach(Book book in bookList)
            {
                outBookList.Add(item: $"{book.Title},{book.Author},{book.IsCheckedOut},{book.DueDate}");
                

            }
            File.WriteAllLines(filePath, outBookList);
            
        }

        public void PrintBookList()
        {
            foreach(var b in bookList)
            {
                FormattedBookList(b);
            }
        }

        public void FormattedBookList(Book b)
        {
            Console.WriteLine(String.Format("{0,-6} {1}", "Title:", b.Title));
            Console.WriteLine(String.Format("{0,-5} {1}", "Author:", b.Author));
            Console.WriteLine(String.Format("{0,-5} {1}", "Status:", b.IsCheckedOut));
            Console.WriteLine(String.Format("{0,-5} {1}", "Due by:", b.DueDate));
            Console.WriteLine();
        }

        public void SearchBookByAuthor(string author)
        {
            string formattedAuthorName = author.ToLower();

            //bookList.Where(x => x.Author.ToLower().Contains(formattedAuthorName)).ToList().ForEach(b => Console.WriteLine(b.Title));
            List<Book> bl = bookList.Where(x => x.Author.ToLower().Contains(formattedAuthorName)).ToList(); 
            if (bl.Count > 0)
            {
                foreach (Book b in bl)
                {
                    FormattedBookList(b);
                }
            }
            else Console.WriteLine("Search produces no results.");
        }

        public void SearchBookByTitle(string title)
        {
            string formattedTitleName = title.ToLower();
            List<Book> bl = bookList.Where(x => x.Title.ToLower().Contains(formattedTitleName)).ToList();
            if (bl.Count > 0)
            {
                foreach (Book b in bl)
                {
                    FormattedBookList(b);
                }
            }
            else Console.WriteLine("Search produces no results.");
        }

        public void CheckOutBook()
        {
            Console.WriteLine("Enter the title of the book you want to check out: ");
            string title = Console.ReadLine();
            List<Book> relevantBooks = new List<Book>();
            relevantBooks = bookList.Where(x => x.Title.Contains(title, StringComparison.OrdinalIgnoreCase)).ToList();
            DateOnly returnDate = DateOnly.FromDateTime(DateTime.Now);
            
            // check if any books returned
            if (relevantBooks.Count() > 1)
            {
                Console.WriteLine("Search produces too many results please be more specific.");
            }
            else if (relevantBooks.Count() == 1)
            {
                foreach (Book b in relevantBooks)
                {
                    if (b.IsCheckedOut == false)
                    {
                        relevantBooks.ForEach(b => {
                            b.IsCheckedOut = true; // change checked out to true
                            b.DueDate = returnDate.AddDays(14);
                            Console.WriteLine(b.Title + " is available and you just checked it out and is due: "+ b.DueDate);

                        }); 
                     
                    }
                    else Console.WriteLine("Book has already been checked out . Sorry :) ");
                }
                

            }
            else Console.WriteLine("Search produced no results.");
            SaveBookList();
        }
        public void CheckInBook()
        {
            Console.WriteLine("Enter the title of the book you want to check in: ");
            string title = Console.ReadLine();
            List<Book> relevantBooks = new List<Book>();
            relevantBooks = bookList.Where(x => x.Title.Contains(title, StringComparison.OrdinalIgnoreCase)).ToList();

            // check if any books returned
            if (relevantBooks.Count() > 1)
            {
                Console.WriteLine("Search produces too many results please be more specific.");
            }
            else if (relevantBooks.Count() == 1)
            {
                foreach (Book b in relevantBooks)
                {
                    if (b.IsCheckedOut == true)
                    {
                        relevantBooks.ForEach(b => b.IsCheckedOut = false);
                        relevantBooks.ForEach(b => Console.WriteLine("Thank you for returning:  "+ b.Title));
                    }
                    else Console.WriteLine("Book has already been checked in.");
                }


            }
            else Console.WriteLine("Search produced no results.");
            SaveBookList();
        }


        public static FileInfo FindApplicationFile(string fileName)
        {
            string startPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), fileName);
            FileInfo file = new FileInfo(startPath);
            while(!file.Exists)
            {
                if(file.Directory.Parent == null)
                {
                    return null;
                }
                DirectoryInfo parentDir = file.Directory.Parent;
                file = new FileInfo(Path.Combine(parentDir.FullName, file.Name));
            }
            return file;
        }

        public void BookMenu()
        {
            Console.Clear();
            Console.WriteLine("Hello, welcome to our library!:");
            Console.WriteLine("1. Browse the list of books");
            Console.WriteLine("2. Search book by author");
            Console.WriteLine("3. Search book by title");
            Console.WriteLine("4. Check out a book");
            Console.WriteLine("5. Check in a book");
            Console.WriteLine("6. Exit program"); 
            Console.Write("Please enter your numbered choice from the selection above: ");
            int userInput = int.Parse(Console.ReadLine());
            Console.Clear();
            if (userInput == 1)
            {
                PrintBookList();
            }
            else if (userInput == 2)
            {
                Console.WriteLine("Please enter the name of the author:");               
                SearchBookByAuthor(Console.ReadLine());
            }
            else if (userInput == 3)
            {
                Console.WriteLine("Please enter the name of the book:");
                SearchBookByTitle(Console.ReadLine());
            }
            else if (userInput == 4)
            {
                PrintBookList();
                CheckOutBook();
            }
            else if (userInput == 5)
            {
                PrintBookList();
                CheckInBook();
            }
            else if (userInput == 6)
            {
                Console.WriteLine("Goodbye!");
                Environment.Exit(0);
            }
            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
        }
    }
}
