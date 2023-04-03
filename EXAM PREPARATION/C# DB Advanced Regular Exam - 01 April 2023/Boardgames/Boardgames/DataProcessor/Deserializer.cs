namespace Boardgames.DataProcessor
{
    using System.ComponentModel.DataAnnotations;
    using System.Text;
    using Boardgames.Data;
    using Boardgames.Data.Models;
    using Boardgames.Data.Models.Enums;
    using Boardgames.DataProcessor.ImportDto;
    using Boardgames.helper;
    using Newtonsoft.Json;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedCreator
            = "Successfully imported creator – {0} {1} with {2} boardgames.";

        private const string SuccessfullyImportedSeller
            = "Successfully imported seller - {0} with {1} boardgames.";


        private static XmlHelper xmlHelper;


        public static string ImportCreators(BoardgamesContext context, string xmlString)
        {
            xmlHelper = new XmlHelper();
            StringBuilder sb = new StringBuilder();

            ImportCreatorDto[] creatorDtos = xmlHelper.Deserialize<ImportCreatorDto[]>(xmlString, "Creators");

            HashSet<Creator> creators = new HashSet<Creator>();

            foreach (var creatorDto in creatorDtos)
            {
                if (!IsValid(creatorDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                HashSet<Boardgame> boardgames = new HashSet<Boardgame>();
                foreach (var item in creatorDto.Boardgames)
                {
                    if (!IsValid(item))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }
                    if (String.IsNullOrEmpty(item.Name))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }
                    Boardgame boardgame = new Boardgame()
                    {
                        Name = item.Name,
                        Rating = item.Rating,
                        YearPublished = item.YearPublished,
                        CategoryType = (CategoryType)item.CategoryType,
                        Mechanics = item.Mechanics
                    };
                    boardgames.Add(boardgame);
                }
                Creator creator = new Creator()
                {
                    FirstName = creatorDto.FirstName,
                    LastName = creatorDto.LastName,
                    Boardgames = boardgames
                };
                creators.Add(creator);

                sb.AppendLine(String.Format(SuccessfullyImportedCreator, creatorDto.FirstName, creatorDto.LastName, boardgames.Count));
            }
            context.AddRange(creators);
            context.SaveChanges();
            return sb.ToString();
        }

        public static string ImportSellers(BoardgamesContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();

            ImportSellersDto[] sellerDtos = JsonConvert.DeserializeObject<ImportSellersDto[]>(jsonString);
            HashSet<Seller> sellers = new HashSet<Seller>();

            foreach (var sellersDto in sellerDtos)
            {
                if (!IsValid(sellersDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                if (string.IsNullOrEmpty(sellersDto.Country))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                if (string.IsNullOrEmpty(sellersDto.Website))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                if (string.IsNullOrEmpty(sellersDto.Address))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                Seller seller = new Seller()
                {
                    Name = sellersDto.Name,
                    Address = sellersDto.Address,
                    Country = sellersDto.Country,
                    Website = sellersDto.Website,
                };
                foreach (int boardId in sellersDto.Boardgames.Distinct())
                {
                    if (!context.Boardgames.Any(f => f.Id == boardId))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }
                    BoardgameSeller boardgameSeller = new BoardgameSeller()
                    {
                        Seller = seller,
                        BoardgameId = boardId,
                    };
                    seller.BoardgamesSellers.Add(boardgameSeller);
                }
                sellers.Add(seller);

                sb.AppendLine(String.Format(SuccessfullyImportedSeller, seller.Name, seller.BoardgamesSellers.Count));
            }
            context.Sellers.AddRange(sellers);
            context.SaveChanges();

            return sb.ToString();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}
