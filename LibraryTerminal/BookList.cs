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


        public void SaveBookList()
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
                Console.WriteLine(String.Format("{0,-6} {1}", "Title:", b.Title));
                //Console.WriteLine(String.Format($"Title: {b.Title,15} Author:  {b.Author} Status: {b.IsCheckedOut} Due by: {b.DueDate}"));
                Console.WriteLine(String.Format("{0,-5} {1}", "Author:", b.Author));
                Console.WriteLine(String.Format("{0,-5} {1}", "Status:", b.IsCheckedOut));
                Console.WriteLine(String.Format("{0,-5} {1}", "Due by:", b.DueDate));
                Console.WriteLine();
            }
        }

        public void SearchBookByAuthor(string author)
        {
            string formattedAuthorName = author.ToLower();
            
            bookList.Where(x => x.Author.ToLower().Contains(formattedAuthorName)).ToList().ForEach(b => Console.WriteLine(b.Title));
        }

        public void SearchBookByTitle(string title)
        {
            string formattedTitleName = title.ToLower();

            bookList.Where(x => x.Title.ToLower().Contains(formattedTitleName)).ToList().ForEach(b => Console.WriteLine(b.Title));
        }

        public void CheckOutBook()
        {
            Console.WriteLine("Enter the title of the book you want to check out: ");
            string title = Console.ReadLine();
            bookList.Where(x => x.Title.Contains(title, StringComparison.OrdinalIgnoreCase)).ToList().ForEach(b => b.IsCheckedOut = true);
            SaveBookList();
        }
        public void CheckInBook()
        {
            Console.WriteLine("Enter the title of the book you want to check in: ");
            string title = Console.ReadLine();
            bookList.Where(x => x.Title.Contains(title, StringComparison.OrdinalIgnoreCase)).ToList().ForEach(b => b.IsCheckedOut = false);
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
    }
}
