using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibraryTerminal;

namespace LibraryTerminal
{
    public class BookList
    {
        List<Book> bookList = new List<Book>();

        string filePath = @"\..\..\..\..\ListOfBooksDB.txt";
        private void LoadBookList()
        {
            StreamReader sr = new(filePath);
            while (true)
            {
                string line = sr.ReadLine();
                if (line == null)
                {
                    break;
                }
                string[] entries = line.Split(',');
                bookList.Add(new Book(entries[0], entries[1], decimal.Parse(entries[2]), DateTime.Parse(entries[3])));
            }
            sr.Close();
        }
    }
}
