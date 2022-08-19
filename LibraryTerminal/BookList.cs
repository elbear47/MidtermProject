using System.Reflection;

namespace LibraryTerminal
{
    public class BookList
    {
        public List<Book> bookList = new List<Book>();

        string filePath = FindApplicationFile("ListOfBooksDB.txt").ToString();

        private static BookList _bookList = new(); // singleton pattern

        /// <summary>
        /// Returns the singleton instance of the BookList class.
        /// </summary>
        /// <returns></returns>
        public static BookList GetBookList()
        {
            return _bookList;
        }

        /// <summary>
        /// Private constructor
        /// </summary>
        private BookList()
        {
            LoadBookList();
        }
        
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
            bookList = bookList.OrderBy(x => x.Author).ThenBy(x => x.Title).ToList();
            foreach(Book book in bookList)
            {
                outBookList.Add(item: $"{book.Title},{book.Author},{book.IsCheckedOut},{book.DueDate}");
            }
            File.WriteAllLines(filePath, outBookList);
        }

        private void BurnItDown()
        {
            bookList.Clear();
            for (int i = 0; i < 15; i++)
            {
                Console.BackgroundColor = ConsoleColor.DarkYellow;
                Console.Clear();
                Console.OutputEncoding = System.Text.Encoding.Unicode;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.WriteLine(@"
                                     (  (     ( /(  ( /( |   /   / 
                                     )\))(   ')\()) )\())|  /|  /  
                                    ((_)()\ )((_)\ ((_)\ | / | /   
                                    _(())\_)()_((_)_ ((_)|/  |/    
                                    \ \((_)/ / || \ \ / (   (      
                                     \ \/\/ /| __ |\ V /)\  )\     
                                      \_/\_/ |_||_| |_|((_)((_)  ");       
                Thread.Sleep(50);
                Console.BackgroundColor = ConsoleColor.DarkRed;
                Thread.Sleep(50);
                Console.Clear();
            }
            Console.ResetColor();        
            Console.Clear();
            Console.WriteLine("Are you happy now?!?");
            SaveBookList();
            Console.ReadKey();
            Environment.Exit(0);
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

        /// <summary>
        /// Prints a formatted list of all books
        /// </summary>
        /// <param name="b"></param>
        private void FormattedBookList(Book b)
        {
            Console.WriteLine("Index #: " + (bookList.IndexOf(b)+1));
            Console.WriteLine(String.Format("{0,-6} {1}", "Title:", b.Title));
            Console.WriteLine(String.Format("{0,-5} {1}", "Author:", b.Author));
            Console.Write("Status: ");
            if(b.IsCheckedOut)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine(String.Format("Book is checked out"));
                Console.ResetColor();
                Console.Write(String.Format("{0,-5}", "Due by: "));
                DateTime dueDate = b.DueDate.ToDateTime(TimeOnly.FromDateTime(DateTime.Now));
                int timeSpan = (dueDate - DateTime.Now).Days;
                if(timeSpan < 0)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine(b.DueDate);
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine(b.DueDate);
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(String.Format("Book is available"));
                Console.ResetColor();
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Search the books by author
        /// </summary>
        /// <param name="author"></param>
        public void SearchBookByAuthor(string author)
        {
            string formattedAuthorName = author.ToLower();

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
        /// 
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

        /// <summary>
        /// Search the books by index
        /// </summary>
        /// <param name="index"></param>
        /// 
        public void SearchBookByIndex()
        {
            do
            {
                Console.Clear();
                Console.WriteLine("Please enter the Index Number of the book:");
                string index = Console.ReadLine();
                if (int.TryParse(index, out int indexNo) && (indexNo < bookList.Count) && indexNo > 0)
                {
                    FormattedBookList(bookList.ElementAt(--indexNo));
                    return;
                }
                Console.WriteLine("Search produces no results. Press any key to continue:");
                Console.ReadKey();
            } while (true);
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
            else if(relevantBooks.Count() == 1 )
            {
                Book b1 = relevantBooks[0];
                if(!b1.IsCheckedOut)
                {
                    do
                    {
                        Console.Clear();
                        Console.Write("Are you sure you would like to checkout " + b1.Title + " --> (y/n)? ");// prompt if they are sure
                        ConsoleKey userInput = Console.ReadKey().Key;
                        Console.WriteLine();
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
            if (relevantBooks.Count() > 1)
            {
                Console.WriteLine("Search produces too many results please be more specific.");
            }
            else if(relevantBooks.Count() == 1)
            {
                Book b1 = relevantBooks[0];
                decimal lateFee = 0.50m;
                DateTime dueDate = b1.DueDate.ToDateTime(TimeOnly.FromDateTime(DateTime.Now));
                int timeSpan = (dueDate - DateTime.Now).Days; 
                decimal totalFee = Math.Abs(timeSpan) * lateFee;
            if (b1.IsCheckedOut == true)
                {
                    do
                    {
                        Console.Clear();
                        Console.WriteLine($"Are you sure you wish to check in {b1.Title}? (y/n):");
                        ConsoleKey userInput = Console.ReadKey().Key;
                        Console.WriteLine();
                        if (userInput == ConsoleKey.Y) //confirm if they would like to check in
                        {
                            b1.IsCheckedOut = false; // change checked out to false
                            if (b1.DueDate >= DateOnly.FromDateTime(DateTime.Now))
                            {
                                Console.WriteLine("Thank you for returning:  " + b1.Title);
                            }
                            else 
                            {
                                if (b1.Title == "How to Conquer Rome")
                                {
                                    BurnItDown();
                                    Console.WriteLine("Are you happy now?");
                                    Environment.Exit(0);
                                }
                                Console.WriteLine($"This is late! Your late fee will be {totalFee:c}.");
                            }
                            SaveBookList(); // save changes to file
                            return;
                        }
                        else if (userInput == ConsoleKey.N)
                        {
                            if (b1.DueDate >= DateOnly.FromDateTime(DateTime.Now))
                            {
                                Console.WriteLine($"Ok, just make sure to return it before {b1.DueDate}");
                            }
                            else
                            {
                                Console.WriteLine($"This is late! Your current late fee is {totalFee:c}, and will increase by {lateFee:c} each day.");
                            }
                            Console.WriteLine();
                            return;
                        } 
                    } while (true);
                }
                else Console.WriteLine("Book has already been checked in.");
            }
            else Console.WriteLine("Search produced no results.");
        }

        /// <summary>
        /// Adds a book to the list
        /// </summary>
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
        /// Removes a book from the list
        /// </summary>
        public void RemoveBook()
        {
            PrintBookList();
            List<Book> relevantBooks = new List<Book>();           
            Console.WriteLine("Enter book title: ");
            string t = Console.ReadLine();
            relevantBooks = bookList.Where(x => x.Title.Contains(t, StringComparison.OrdinalIgnoreCase)).ToList();
            if(relevantBooks.Count() > 1)
            {
                Console.WriteLine("Search produces too many results please be more specific.");
            }
            else if(relevantBooks.Count == 1)
            {
                Book b1 = relevantBooks[0];
                do
                {
                    Console.Clear();
                    Console.WriteLine($"Are you sure you wish to remove {b1.Title}? (y/n):");
                    ConsoleKey userInput = Console.ReadKey().Key;
                    Console.WriteLine();
                    if(userInput == ConsoleKey.Y) //confirm if they would like to remove
                    {
                        Console.WriteLine($"{b1.Title} removed.");
                        bookList.Remove(b1);
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
            else Console.WriteLine("Search produced no results.");
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
                    Console.WriteLine($"The file {fileName} was not found.");
                    Console.WriteLine("Press any key to continue.");
                    Console.ReadKey();
                    Environment.Exit(1);
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
            Console.WriteLine("Hello, welcome to our library! \n");
            Console.WriteLine("1. Browse the list of books");
            Console.WriteLine("2. Search book by author");
            Console.WriteLine("3. Search book by title or index number");
            Console.WriteLine("4. Check out a book");
            Console.WriteLine("5. Check in a book");
            Console.WriteLine("6. Add a book");
            Console.WriteLine("7. Remove a book");
            Console.WriteLine("8. Exit program \n");
            Console.Write("Please enter your numbered choice from the selection above: ");
            string userInput = Console.ReadLine().ToUpper().Trim();
            int selection = 0;

            if(int.TryParse(userInput, out selection) && selection > 0 && selection < 9)
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
                    Console.WriteLine("Would you like to search by: \n1. Title \n2. Index No." );
                    ConsoleKey userInputTwo = Console.ReadKey().Key;
                    Console.WriteLine();
                    if (userInputTwo == ConsoleKey.D1 || userInputTwo == ConsoleKey.NumPad1) 
                    {
                        Console.WriteLine("Please enter the name of the book:");
                        SearchBookByTitle(Console.ReadLine());
                    }
                    else if (userInputTwo == ConsoleKey.D2 || userInputTwo == ConsoleKey.NumPad2)
                    {
                        SearchBookByIndex();
                    }
                    else Console.WriteLine("That is not a valid input");
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
                    RemoveBook();
                }
                else if(selection == 8)
                {
                    Console.WriteLine("Goodbye!");
                    Environment.Exit(0);
                }
            }
            else if (userInput == "48BC")
            {
                BurnItDown();
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
