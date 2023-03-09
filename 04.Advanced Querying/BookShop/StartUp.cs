namespace BookShop
{
    using BookShop.Models.Enums;
    using Data;
    using Initializer;
    using System.Text;

    public class StartUp
    {
        public static void Main()
        {
            using var db = new BookShopContext();
            //DbInitializer.ResetDatabase(db);

            string input = Console.ReadLine();
            string result = GetBooksByCategory(db, input);

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
    }
}


