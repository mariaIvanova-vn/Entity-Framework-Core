namespace MusicHub
{
    using System;
    using System.Globalization;
    using System.Text;
    using Data;
    using Initializer;

    public class StartUp
    {
        public static void Main()
        {
            MusicHubDbContext context =
                new MusicHubDbContext();

            DbInitializer.ResetDatabase(context);

            //Test your solutions here
            string result = ExportSongsAboveDuration(context, 9);
            Console.WriteLine(result);
        }

        public static string ExportAlbumsInfo(MusicHubDbContext context, int producerId)
        {
            StringBuilder sb = new StringBuilder();

            var albumInfo = context.Albums
                .Where(a => a.ProducerId.HasValue && a.ProducerId.Value == producerId)
                .ToArray()
                .OrderByDescending(a => a.Price)
                .Select(a => new
                {
                    a.Name,
                    ReleaseDate = a.ReleaseDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture),
                    ProducerName = a.Producer!.Name,
                    Songs = a.Songs.Select(s => new
                            {
                                SongName = s.Name,
                                Price = s.Price.ToString("f2"),
                                Writer = s.Writer.Name
                            })
                            .OrderByDescending(s => s.SongName).ThenBy(s => s.Writer),
                    AlbumPrice = a.Price.ToString("f2")
                }).ToArray();

            foreach (var a in albumInfo)
            {
                sb.AppendLine($"-AlbumName: {a.Name}");
                sb.AppendLine($"-ReleaseDate: {a.ReleaseDate}");
                sb.AppendLine($"-ProducerName: {a.ProducerName}"); 
                sb.AppendLine("-Songs:");

                int songNum = 1;
                foreach (var s in a.Songs)
                {
                    sb.AppendLine($"---#{songNum}");
                    sb.AppendLine($"---SongName: {s.SongName}");
                    sb.AppendLine($"---Price: {s.Price}");
                    sb.AppendLine($"---Writer: {s.Writer}");
                    songNum++;
                }
                sb.AppendLine($"-AlbumPrice: {a.AlbumPrice}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string ExportSongsAboveDuration(MusicHubDbContext context, int duration)
        {
            var sb = new StringBuilder();
            var songsInfo = context.Songs
                .AsEnumerable()
                .Where(s=>s.Duration.TotalSeconds > duration)
                 .Select(s => new
                        {
                            s.Name,
                            Performers = s.SongPerformers.Select(sp=>$"{sp.Performer.FirstName} {sp.Performer.LastName}")
                                 .OrderBy(p=>p)
                                 .ToArray(),
                            WriterName = s.Writer.Name,
                            AlbumProducer = s.Album!.Producer!.Name,
                            Duration = s.Duration.ToString("c")
                        })
                 .OrderBy(s=>s.Name).ThenBy(s=>s.WriterName).ToArray();

            int songNum = 1;

            foreach (var s in songsInfo)
            {
                sb.AppendLine($"-Song #{songNum}");
                sb.AppendLine($"---SongName: {s.Name}");
                sb.AppendLine($"---Writer: {s.WriterName}");
                foreach (var p in s.Performers)
                {
                    sb.AppendLine($"---Performer: {p}");
                }
                sb.AppendLine($"---AlbumProducer: {s.AlbumProducer}");
                sb.AppendLine($"---Duration: {s.Duration}");
                songNum++;
            }
            return sb.ToString();
        }
    }
}
