namespace Boardgames.DataProcessor
{
    using Boardgames.Data;
    using Boardgames.DataProcessor.ExportDto;
    using Boardgames.helper;
    using Newtonsoft.Json;

    public class Serializer
    {
        private static XmlHelper xmlHelper;



        public static string ExportCreatorsWithTheirBoardgames(BoardgamesContext context)
        {
            xmlHelper = new XmlHelper();

            ExportCreatorDto[] result = context.Creators
                 .Where(c => c.Boardgames.Any())
                .ToArray()
                .Select(c=>new ExportCreatorDto()
                {
                    CreatorName = c.FirstName + " " + c.LastName,
                    BoardgamesCount = c.Boardgames.Count,
                    Boardgames = c.Boardgames.Select(b=>new ExportBoardgameDto()
                    {
                        BoardgameName = b.Name,
                        BoardgameYearPublished = b.YearPublished
                    })
                    .OrderBy(b => b.BoardgameName)
                    .ToArray()
                })
                 .OrderByDescending(d => d.BoardgamesCount)
                .ThenBy(d => d.CreatorName)
                .ToArray();

            return xmlHelper.Serialize(result, "Creators");
        }

        public static string ExportSellersWithMostBoardgames(BoardgamesContext context, int year, double rating)
        {
            var result = context.Sellers
                .Where(s => s.BoardgamesSellers.Any(bs => bs.Boardgame.YearPublished >= year
                && bs.Boardgame.Rating <= rating))
                .ToArray()
                .Select(s => new
                {
                    Name = s.Name,
                    Website = s.Website,
                    Boardgames = s.BoardgamesSellers
                    .Where(bs=>bs.Boardgame.YearPublished >= year && bs.Boardgame.Rating <= rating)
                    .Select(b => new
                    {
                        Name = b.Boardgame.Name,
                        Rating = b.Boardgame.Rating,
                        Mechanics = b.Boardgame.Mechanics,
                        Category = b.Boardgame.CategoryType.ToString()
                    })
                    .OrderByDescending(bs => bs.Rating)
                    .ThenBy(bs=>bs.Name)
                })
                .OrderByDescending(s=>s.Boardgames.Count())
                .ThenBy(s=>s.Name)
                .Take(5)
                .ToArray();

            string json = JsonConvert.SerializeObject(result);

            return json;
        }
    }
}