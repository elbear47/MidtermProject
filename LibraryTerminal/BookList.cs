using System.Reflection;

namespace LibraryTerminal
{
    public class BookList
    {
        public List<Book> bookList = new List<Book>();

        string filePath = FindApplicationFile("ListOfBooksDB.txt").ToString();

        /// <summary>
        /// Loads book list form file
        /// </summary>
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

        /// <summary>
        /// Saves book list to file
        /// </summary>
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

        private void BurnItDown()
        {
            bookList.Clear();
            SaveBookList();
        }

        /// <summary>
        /// Prints a list of all books
        /// </summary>
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
            if(b.IsCheckedOut)
            {
                Console.WriteLine(String.Format("Status: Book is checked out"));
            }
            else
            {
                Console.WriteLine(String.Format("Status: Book is available"));
            }
            Console.WriteLine(String.Format("{0,-5} {1}", "Due by:", b.DueDate));
            Console.WriteLine();
        }

        /// <summary>
        /// Search the books by author
        /// </summary>
        /// <param name="author"></param>
        public void SearchBookByAuthor(string author)
        {
            string formattedAuthorName = author.ToLower();

            //bookList.Where(x => x.Author.ToLower().Contains(formattedAuthorName)).ToList().ForEach(b => Console.WriteLine(b.Title));
            List<Book> bl = bookList.Where(x => x.Author.ToLower().Contains(formattedAuthorName)).ToList();
            if(bl.Count > 0)
            {
                foreach(Book b in bl)
                {
                    FormattedBookList(b);
                }
            }
            else Console.WriteLine("Search produces no results.");
        }

        /// <summary>
        /// Search the books by title
        /// </summary>
        /// <param name="title"></param>
        public void SearchBookByTitle(string title)
        {
            string formattedTitleName = title.ToLower();
            List<Book> bl = bookList.Where(x => x.Title.ToLower().Contains(formattedTitleName)).ToList();
            if(bl.Count > 0)
            {
                foreach(Book b in bl)
                {
                    FormattedBookList(b);
                }
            }
            else Console.WriteLine("Search produces no results.");
        }

        /// <summary>
        /// Checks a book in
        /// </summary>
        public void CheckOutBook()
        {
            Console.WriteLine("Enter the title of the book you want to check out: ");
            string title = Console.ReadLine();
            List<Book> relevantBooks = new List<Book>();
            relevantBooks = bookList.Where(x => x.Title.Contains(title, StringComparison.OrdinalIgnoreCase)).ToList();
            DateOnly returnDate = DateOnly.FromDateTime(DateTime.Now);

            // check if any books returned
            if(relevantBooks.Count() > 1)
            {
                Console.WriteLine("Search produces too many results please be more specific.");
            }
            else if(relevantBooks.Count() == 1)
            {
                Book b1 = relevantBooks[0];
                if(!b1.IsCheckedOut)
                {
                    do
                    {
                        Console.Clear();
                        Console.Write("Are you sure you would like to checkout " + b1.Title + " --> (y/n)? ");// prompt if they are sure
                        ConsoleKey userInput = Console.ReadKey().Key;
                        if(userInput == ConsoleKey.Y) //confirm if they would like to check out
                        {
                            b1.IsCheckedOut = true; // change checked out to true
                            b1.DueDate = returnDate.AddDays(14);
                            Console.WriteLine(b1.Title + " is available and you just checked it out and is due: " + b1.DueDate);
                            SaveBookList(); // save changes to file
                            return;
                        }
                        else if(userInput == ConsoleKey.N)
                        {
                            Console.WriteLine();
                            Console.WriteLine("I'll put it back for you.");
                            return;
                        }
                    } while(true);
                }
                else Console.WriteLine("Book has already been checked out . Sorry :) ");
            }
            else Console.WriteLine("Search produced no results.");
        }

        /// <summary>
        /// Checks a book in
        /// </summary>
        public void CheckInBook()
        {
            Console.WriteLine("Enter the title of the book you want to check in: ");
            string title = Console.ReadLine();
            List<Book> relevantBooks = new List<Book>();
            relevantBooks = bookList.Where(x => x.Title.Contains(title, StringComparison.OrdinalIgnoreCase)).ToList();

            // check if any books returned
            if(relevantBooks.Count() > 1)
            {
                Console.WriteLine("Search produces too many results please be more specific.");
            }
            else if(relevantBooks.Count() == 1)
            {
                Book book = relevantBooks[0];
                if(book.IsCheckedOut == true)
                {
                    book.IsCheckedOut = false;
                    Console.WriteLine("Thank you for returning:  " + book.Title);
                    SaveBookList();
                }
                else Console.WriteLine("Book has already been checked in.");
            }
            else Console.WriteLine("Search produced no results.");
        }

        public void AddBook()
        {
            Console.WriteLine("Enter book title: ");
            string t = Console.ReadLine();
            Console.WriteLine("Enter book author: ");
            string a = Console.ReadLine();

            DateOnly currentDate = DateOnly.FromDateTime(DateTime.Now);

            Book newBook = new Book(t, a, false, currentDate);

            bookList.Add(newBook);
            SaveBookList();
        }


        /// <summary>
        /// Finds the file path of the file
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Main menu
        /// </summary>
        public void BookMenu()
        {
            Console.Clear();
            Console.WriteLine("Hello, welcome to our library!:");
            Console.WriteLine("1. Browse the list of books");
            Console.WriteLine("2. Search book by author");
            Console.WriteLine("3. Search book by title");
            Console.WriteLine("4. Check out a book");
            Console.WriteLine("5. Check in a book");
            Console.WriteLine("6. Add a book");
            Console.WriteLine("7. Exit program");
            Console.Write("Please enter your numbered choice from the selection above: ");
            string userInput = Console.ReadLine();
            int selection = 0;

            if(int.TryParse(userInput, out selection) && selection > 0 && selection < 8)
            {
                Console.Clear();
                if(selection == 1)
                {
                    PrintBookList();
                }
                else if(selection == 2)
                {
                    Console.WriteLine("Please enter the name of the author:");
                    SearchBookByAuthor(Console.ReadLine());
                }
                else if(selection == 3)
                {
                    Console.WriteLine("Please enter the name of the book:");
                    SearchBookByTitle(Console.ReadLine());
                }
                else if(selection == 4)
                {
                    PrintBookList();
                    CheckOutBook();
                }
                else if(selection == 5)
                {
                    PrintBookList();
                    CheckInBook();
                }
                else if(selection == 6)
                {
                    AddBook();
                }
                else if(selection == 7)
                {
                    Console.WriteLine("Goodbye!");
                    Environment.Exit(0);
                }
            }
            else
            {
                Console.WriteLine("Not a valid input.");
            }
            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
        }
    }
}
