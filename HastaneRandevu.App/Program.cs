namespace HastaneRandevu.App;

static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        var veri = UygulamaVerisi.Olustur();

        // Giris -> panel -> (cikis yapilinca) tekrar giris. Giris penceresi kapatilirsa program biter.
        while (true)
        {
            using var giris = new GirisForm(veri);
            if (giris.ShowDialog() != DialogResult.OK)
                break;

            using Form panel = PanelSecici.Sec(giris.GirisYapan!, veri);
            panel.ShowDialog();
        }
    }
}
