using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            //Console.WriteLine(bookList.Where(x => x.Author == author));
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
