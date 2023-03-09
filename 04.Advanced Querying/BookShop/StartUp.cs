namespace BookShop
{
    using BookShop.Models.Enums;
    using Data;
    using Initializer;
    using System.Globalization;
    using System.Text;

    public class StartUp
    {
        public static void Main()
        {
            using var db = new BookShopContext();
            //DbInitializer.ResetDatabase(db);

            //int input = int.Parse(Console.ReadLine());
           var result = RemoveBooks(db);

            Console.WriteLine(result);
        }


        //2.	Age Restriction

        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            try
            {
                AgeRestriction ageRestriction = Enum.Parse<AgeRestriction>(command, true);

                string[] bookTitles = context.Books
                .Where(b => b.AgeRestriction == ageRestriction)
                .OrderBy(b => b.Title)
                .Select(b => b.Title)
                .ToArray();

                return String.Join(Environment.NewLine, bookTitles);
            }
            catch (Exception e)
            {

                return e.Message;
            }
            
        }


        //3.	Golden Books

        public static string GetGoldenBooks(BookShopContext context)
        {
            var bookTitles = context.Books
                .Where(b => b.EditionType == EditionType.Gold && b.Copies < 5000)
                .OrderBy(b => b.BookId)
                .Select(b => b.Title)
                .ToArray();

            return String.Join(Environment.NewLine, bookTitles);
        }


        //4.	Books by Price

        public static string GetBooksByPrice(BookShopContext context)
        {
            var sb = new StringBuilder();

            var result = context.Books.
                Where(b => b.Price > 40)
                .OrderByDescending(b => b.Price)
                .Select(b => new
                {
                    b.Title,
                    b.Price
                }).ToArray();

            foreach (var item in result)
            {
                sb.AppendLine($"{item.Title} - ${item.Price:f2}");
            }

            return sb.ToString();
        }


        //5.	Not Released In

        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            try
            {
                string[] result = context.Books
                .Where(b => b.ReleaseDate.Value.Year != year)
                .OrderBy(b => b.BookId)
                .Select(b=>b.Title)
                .ToArray();

                return string.Join(Environment.NewLine, result);

            }
            catch (Exception e)
            {

                return e.Message;
            }
            
        }

        //6.	Book Titles by Category

        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            string[] categories = input.Split(' ',StringSplitOptions.RemoveEmptyEntries)
                .Select(c=>c.ToLower()).ToArray();

            var booksTitle = context.Books
                .Where(b => b.BookCategories.Any(bc => categories.Contains(bc.Category.Name.ToLower())))
                .OrderBy(b => b.Title)
                .Select(b => b.Title)
                .ToArray();

            return String.Join(Environment.NewLine, booksTitle);
        }


        //7.	Released Before Date

        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            var output = new StringBuilder();

            var parsedDate = DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            var books = context
                .Books
                .Where(b => b.ReleaseDate < parsedDate)
                .OrderByDescending(b => b.ReleaseDate)
                .Select(b => new
                {
                    Title = b.Title,
                    EditionType = b.EditionType.ToString(),
                    Price = b.Price,
                })
                .ToArray();

            foreach (var b in books)
            {
                output
                    .AppendLine($"{b.Title} - {b.EditionType} - ${b.Price:f2}");
            }

            return output.ToString().TrimEnd();
        }


        //8.	Author Search

        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            var output = new StringBuilder();

            var result = context.Authors.Where(a => a.FirstName.EndsWith(input))
                .OrderBy(a => a.FirstName)
                .ThenBy(a => a.LastName)
                .Select(a => new
                {
                    a.FirstName,
                    a.LastName
                }).ToArray();

            foreach (var a in result)
            {
                output.AppendLine($"{a.FirstName} {a.LastName}");
            }

            return output.ToString();
        }


        //9.	Book Search
         public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            var result = context.Books.Where(b=>b.Title.ToLower().Contains(input.ToLower()))
                .OrderBy(b => b.Title)
                .Select(b=>b.Title)
                .ToArray();

            return String.Join(Environment.NewLine, result);               
        }


        //10.	Book Search by Author
        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            var output = new StringBuilder();

            var result = context.Books
                .Where(b => b.Author.LastName.ToLower().StartsWith(input.ToLower()))
                .OrderBy(b => b.BookId)
                .Select(b => new
                {
                    b.Title,
                    authorsName = b.Author.FirstName + " " + b.Author.LastName,
                }).ToArray();

            foreach (var b in result)
            {
                output.AppendLine($"{b.Title} ({b.authorsName})");
            }

            return output.ToString();
        }


        //11.	Count Books
        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            var result = context.Books
                .Where(b=>b.Title.Length>lengthCheck)
                .Select(b=>b.Title.Count())
                .ToArray();

            return result.Count();    
        }

        //12.	Total Book Copies
        public static string CountCopiesByAuthor(BookShopContext context)
        {
            var output = new StringBuilder();

            var result = context.Authors
                
                .Select(a => new
                {
                    authorsName = a.FirstName + " " + a.LastName,
                    copies = a.Books.Sum(b=>b.Copies)
                })
                .OrderByDescending(a=>a.copies)
                .ToArray();

            foreach (var b in result)
            {
                output.AppendLine($"{b.authorsName} - {b.copies}");
            }
            return output.ToString().Trim();    
        }


        //13.	Profit by Category
        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            var output = new StringBuilder();

            var result = context.Categories
                .Select(c => new
                {
                    c.Name,
                    totalProfit = c.CategoryBooks.Sum(cb => cb.Book.Copies * cb.Book.Price)
                })
                .OrderByDescending(c => c.totalProfit)
                .ThenBy(c => c.Name)
                .ToArray();

            foreach (var c in result)
            {
                output.AppendLine($"{c.Name} ${c.totalProfit:f2}");
            }

            return output.ToString();
        }

        //14.	Most Recent Books
        public static string GetMostRecentBooks(BookShopContext context)
        {
            var output = new StringBuilder();

            var result = context.Categories
                .OrderBy(c=>c.Name)
                .Select(c=> new
                {
                    catName = c.Name,
                    recentBooks = c.CategoryBooks
                       .OrderByDescending(cb=>cb.Book.ReleaseDate)
                       .Take(3)
                       .Select(cb => new
                       {
                           bookTitle = cb.Book.Title,
                           releaseYear=cb.Book.ReleaseDate.Value.Year,
                       }).ToArray()
                }).ToArray();

            foreach (var c in result)
            {
                output.AppendLine($"--{c.catName}");
                foreach (var b in c.recentBooks)
                {
                    output.AppendLine($"{b.bookTitle} ({b.releaseYear})");
                }
            }

            return output.ToString();
        }

        //15.	Increase Prices
        public static void IncreasePrices(BookShopContext context)
        {
            var searchBooks = context.Books
                .Where(b=>b.ReleaseDate.Value.Year<2010)
                .ToArray();

            foreach (var b in searchBooks)
            {
                b.Price += 5;
            }
            context.SaveChanges();
        }

        //16.	Remove Books
        public static int RemoveBooks(BookShopContext context)
        {
            var searchBooks = context.Books
                .Where(b => b.Copies < 4200);

           var bookCount = searchBooks.Count();


            context.Books.RemoveRange(searchBooks);
            context.SaveChanges();

            return bookCount;
        }
    }
}


