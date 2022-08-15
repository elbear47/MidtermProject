

using LibraryTerminal;

//Book b = new Book("my title", "troy wilson", false);



//Console.WriteLine(b.DueDate);

BookList bookList = new();

bookList.LoadBookList();

bookList.PrintBookList();


//bookList.SearchBookByAuthor("jAne");

//bookList.SearchBookByTitle("gatsby");

bookList.CheckOutBook();