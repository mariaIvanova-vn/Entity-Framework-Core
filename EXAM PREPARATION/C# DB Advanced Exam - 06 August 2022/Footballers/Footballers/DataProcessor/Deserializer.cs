namespace Footballers.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Data;
    using Footballers.Data.Models;
    using Footballers.Data.Models.Enums;
    using Footballers.DataProcessor.ImportDto;
    using Footballers.Utilities;
    using Newtonsoft.Json;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedCoach
            = "Successfully imported coach - {0} with {1} footballers.";

        private const string SuccessfullyImportedTeam
            = "Successfully imported team - {0} with {1} footballers.";


        private static XmlHelper xmlHelper;
        public static string ImportCoaches(FootballersContext context, string xmlString)
        {
            xmlHelper = new XmlHelper();
            StringBuilder sb = new StringBuilder();

            ImportCoachDto[] coachDtos = xmlHelper.Deserialize<ImportCoachDto[]>(xmlString, "Coaches");

            HashSet<Coach> coaches = new HashSet<Coach>();

            foreach (var item in coachDtos)
            {
                if (!IsValid(item))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                if (String.IsNullOrEmpty(item.Nationality))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                HashSet<Footballer> footballers = new HashSet<Footballer>();
                foreach (var footboollDto in item.Footballers)
                {
                    if (!IsValid(footboollDto))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }
                    bool isStartDateValid = DateTime.TryParseExact(footboollDto.ContractStartDate, "dd/MM/yyyy",
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime startDate);

                    bool isEndDateValid = DateTime.TryParseExact(footboollDto.ContractEndDate, "dd/MM/yyyy",
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime endDate);

                    if (!isStartDateValid || !isEndDateValid)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    if (startDate > endDate)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }
                    Footballer footballer = new Footballer()
                    {
                        Name = footboollDto.Name,
                        ContractStartDate = startDate,
                        ContractEndDate = endDate,
                        BestSkillType = (BestSkillType)footboollDto.BestSkillType,
                        PositionType = (PositionType)footboollDto.PositionType
                    };
                    footballers.Add(footballer);
                }
                Coach coach = new Coach()
                {
                    Name = item.Name,
                    Nationality = item.Nationality,
                    Footballers = footballers
                };
                coaches.Add(coach);

                sb.AppendLine(String.Format(SuccessfullyImportedCoach, item.Name, footballers.Count));
            }
            context.AddRange(coaches);
            context.SaveChanges();
            return sb.ToString();
        }
        public static string ImportTeams(FootballersContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();

            ImportTeamsDto[] teamsDtos = JsonConvert.DeserializeObject<ImportTeamsDto[]>(jsonString);
            HashSet<Team> teams = new HashSet<Team>();

            //ICollection<int> existingTeamId = context.Teams.Select(t => t.Id).ToArray();

            foreach (var teamDto in teamsDtos)
            {
                if (!IsValid(teamDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                if (string.IsNullOrEmpty(teamDto.Trophies) || int.Parse(teamDto.Trophies) == 0)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                if (String.IsNullOrEmpty(teamDto.Nationality))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                Team team = new Team()
                {
                    Name = teamDto.Name,
                    Nationality = teamDto.Nationality,
                    Trophies = int.Parse(teamDto.Trophies)
                };

                foreach (int footBallerId in teamDto.Footballers.Distinct())
                {
                    if (!context.Footballers.Any(f => f.Id == footBallerId))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }
                    TeamFootballer teamFootballer = new TeamFootballer()
                    {
                        Team = team,
                        FootballerId = footBallerId,
                    };
                    team.TeamsFootballers.Add(teamFootballer);
                }
                teams.Add(team);

                sb.AppendLine(String.Format(SuccessfullyImportedTeam, team.Name, team.TeamsFootballers.Count));
            }
            context.Teams.AddRange(teams);
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
