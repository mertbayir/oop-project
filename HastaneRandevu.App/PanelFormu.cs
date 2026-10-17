using HastaneRandevu.Core.Models;

namespace HastaneRandevu.App;

// Giris sonrasi acilan panellerin ortak tabani (KALITIM: her panel bundan turer).
// Ortak kisimlar: karsilama yazisi, "Cikis yap" dugmesi, mesaj etiketi ve kucuk yardimcilar.
public abstract class PanelFormu : Form
{
    protected UygulamaVerisi Veri { get; }
    protected Kullanici Kullanici { get; }

    // Basari/hata mesajinin gosterildigi etiket; alt siniflar gerekirse yerini/genisligini ayarlar.
    protected readonly Label lblMesaj = new() { Name = "lblMesaj", Left = 15, Top = 430, Width = 560, Height = 70 };

    protected PanelFormu(Kullanici kullanici, UygulamaVerisi veri, string baslik)
    {
        Kullanici = kullanici;
        Veri = veri;

        Text = $"{baslik} - {kullanici.TamAd}";
        ClientSize = new Size(760, 480);
        StartPosition = FormStartPosition.CenterScreen;

        var karsilama = new Label
        {
            Text = $"Hoş geldiniz, {kullanici.TamAd} ({kullanici.RolAdi})",
            Left = 15, Top = 12, Width = 560, Font = new Font(Font, FontStyle.Bold)
        };
        var cikis = new Button { Text = "Çıkış yap", Left = 645, Top = 8, Width = 100, Anchor = AnchorStyles.Top | AnchorStyles.Right };
        cikis.Click += (_, _) => Close();

        Controls.Add(karsilama);
        Controls.Add(cikis);
        Controls.Add(lblMesaj);
    }

    protected void Mesaj(string metin, bool basari)
    {
        lblMesaj.ForeColor = basari ? Color.DarkGreen : Color.Firebrick;
        lblMesaj.Text = metin;
    }

    protected void EtiketEkle(string metin, int sol, int ust, bool kalin = false)
        => Controls.Add(new Label
        {
            Text = metin, Left = sol, Top = ust, Width = 100,
            Font = kalin ? new Font(Font, FontStyle.Bold) : Font
        });
}
