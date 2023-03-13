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
            Console.WriteLine(ExportSongsAboveDuration(context, 4));
        }

        public static string ExportAlbumsInfo(MusicHubDbContext context, int producerId)
        {
            var albums = context.Albums.Where(a => a.ProducerId == producerId).ToList();
            var sb = new StringBuilder();
            foreach (var album in albums)
            {
                sb.AppendLine($"-AlbumName: {album.Name}" + Environment.NewLine +
                    $"-ReleaseDate: {album.ReleaseDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture)}" + Environment.NewLine +
                    $"-ProducerName: {album.Producer.Name}" + Environment.NewLine +
                    $"-Songs:");
                var count = 1;
                foreach (var item in album.Songs)
                {
                    sb.AppendLine($"---#{count}" + Environment.NewLine +
                    $"---SongName: {item.Name}" + Environment.NewLine +
                    $"---Price: {item.Price:F2}" + Environment.NewLine +
                    $"---Writer: {item.Writer.Name}");
                    count++;
                }
                sb.AppendLine($"-AlbumPrice: {album.Price:F2}");
            }
            return sb.ToString().Trim();
        }

        public static string ExportSongsAboveDuration(MusicHubDbContext context, int duration)
        {
            var songs = context.Songs.Where(a => a.Duration.TotalSeconds > duration).OrderBy(x => x.Name).ThenBy(x => x.Writer.Name).ToList();
            var sb = new StringBuilder();
            var count = 1;
            foreach (var song in songs)
            {
                sb.AppendLine($"-Song #{count}" + Environment.NewLine +
                   $"---SongName: {song.Name}" + Environment.NewLine +
                   $"---Writer: {song.Writer.Name}");
                foreach (var performer in song.SongPerformers.OrderBy(p => p.Performer.FirstName))
                {
                    sb.AppendLine($"---Performer: {performer.Performer.FirstName} {performer.Performer.LastName}");
                }
                sb.AppendLine($"---AlbumProducer: {song.Album.Producer.Name}" + Environment.NewLine +
                  $"---Duration: {song.Duration.ToString("c")}");
                count++;
            }
            return sb.ToString()
                .Trim();
        }
    }
}
