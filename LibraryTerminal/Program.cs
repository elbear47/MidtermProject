using LibraryTerminal;

BookList bookList = new();
bookList.LoadBookList();
do
{
    bookList.BookMenu();
} while (true);
