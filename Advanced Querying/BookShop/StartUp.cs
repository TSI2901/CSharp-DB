namespace BookShop
{
    using Data;
    using Models;
    using Initializer;
    using System.Linq;
    using BookShop.Models.Enums;
    using System.Text;
    using System.Globalization;

    public class StartUp
    {
        public static void Main()
        {
            //using var db = new BookShopContext();
            //DbInitializer.ResetDatabase(db);
            var context = new BookShopContext();
            Console.WriteLine(GetTotalProfitByCategory(context));
        }
        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
             AgeRestriction ageRestriction = Enum.Parse<AgeRestriction>(command, true);
            var books = context.Books.Where(b => b.AgeRestriction == ageRestriction).OrderBy(x => x.Title).Select(x => x.Title)
                .ToList();
            return String.Join(Environment.NewLine, books);
        }
        public static string GetGoldenBooks(BookShopContext context)
        {
            var books = context.Books.Where(b => b.Copies < 5000 && b.EditionType == EditionType.Gold).OrderBy(b => b.BookId).Select(x => x.Title).ToList();
            return String.Join (Environment.NewLine, books);
        }
        public static string GetBooksByPrice(BookShopContext context)
        {
            var sb = new StringBuilder();
            var books = context.Books.Where(b => b.Price > 40)
                .OrderByDescending(x => x.Price)
                .Select(x => new
                {
                    Title = x.Title,
                    Price = x.Price
                }).ToList();
            foreach (var book in books)
            {
                sb.Append($"{book.Title} - {book.Price:f2}");
            }
            return sb.ToString().Trim();
        }
        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {

            var books = context.Books.Where(b => b.ReleaseDate.Value.Year != year).OrderBy(b => b.BookId).Select(book => book.Title).ToList();
            return String.Join(Environment.NewLine, books);
        }
        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            var list = input.Split(" ").Select(x => x.ToLower()).ToList();
            var books = context.Books.Where(b => b.BookCategories.Any(bc => list.Contains(bc.Category.Name.ToLower())))
                .OrderBy(x => x.Title).Select(x => x.Title).ToList();
            return String.Join(Environment.NewLine, books);
        }
        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            var parsedDate = DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            var books = context.Books.Where(b => b.ReleaseDate < parsedDate)
                .OrderByDescending(b => b.ReleaseDate)
                .Select(b => $"{b.Title} - {b.EditionType} - ${b.Price:f2}")
                .ToList();
            return String.Join(Environment.NewLine, books);
        }
        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            var books = context.Authors.Where(b => b.FirstName.EndsWith(input))
                .OrderBy(x => x.FirstName)
                .ThenBy(x => x.LastName)
                .Select(x => $"{x.FirstName} {x.LastName}")
                .ToList();
            return String.Join(Environment.NewLine, books);
        }
        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            var books = context.Books.Where(b => b.Title.ToLower().Contains(input.ToLower())).OrderBy(x => x.Title).Select(x => x.Title).ToList();
            return String.Join(Environment.NewLine, books);
        }
        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            var booksAndAuthors = context.Books
                .Where(b => b.Author.LastName.ToLower().StartsWith(input.ToLower()))
                .OrderBy(x => x.BookId)
                .Select(x => $"{x.Title} ({x.Author.FirstName} {x.Author.LastName})")
                .ToList() ;
            return String.Join(Environment.NewLine,booksAndAuthors) ;
        }
        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            var books = context.Books.Where(b => b.Title.Length > lengthCheck).ToList();
            return books.Count;
        }
        public static string CountCopiesByAuthor(BookShopContext context)
        {
            var authors = context.Authors.OrderByDescending(x => x.Books.Sum(b => b.Copies))
                .Select(x => $"{x.FirstName} {x.LastName} - {x.Books.Sum(x => x.Copies)}")
                .ToList();
            return String.Join (Environment.NewLine, authors) ;
        }
        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            var bookPrices = context.Categories.OrderByDescending(x => x.CategoryBooks.Sum(p => p.Book.Price * p.Book.Copies))
                .Select(x => $"{x.Name} ${x.CategoryBooks.Sum(p => p.Book.Price * p.Book.Copies):F2}")
                .ToList ();
            return String.Join (Environment.NewLine, bookPrices) ;
        }
    }
}


