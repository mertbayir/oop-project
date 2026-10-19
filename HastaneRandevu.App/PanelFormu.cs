using System.Globalization;
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

    // Bir isi dener: basarili olursa yesil mesaj, servis kural hatasi verirse kirmizi mesaj gosterir.
    // Mesaji islem sonunda uretmek gerekiyorsa (ornegin icinde isim geciyorsa) asagidaki surum kullanilir.
    protected void Dene(Action islem, string basariMesaji)
        => Dene(() => { islem(); return basariMesaji; });

    protected void Dene(Func<string> islem)
    {
        try
        {
            Mesaj(islem(), true);
        }
        catch (Exception ex) when (ex is UnauthorizedAccessException or InvalidOperationException or ArgumentException)
        {
            Mesaj(HataMetni.Al(ex), false);
        }
    }

    // "kap" verilirse etiket o kapsayiciya (ornegin bir sekmeye), verilmezse dogrudan forma eklenir.
    protected void EtiketEkle(string metin, int sol, int ust, bool kalin = false, Control? kap = null)
        => (kap?.Controls ?? Controls).Add(new Label
        {
            Text = metin, Left = sol, Top = ust, Width = 100,
            Font = kalin ? new Font(Font, FontStyle.Bold) : Font
        });

    // Gun kutusunu doldurur: Pazartesi'den Pazar'a, ekranda Turkce adlariyla (kutunun icindeki deger DayOfWeek kalir).
    protected static void GunKutusunuHazirla(ComboBox kutu)
    {
        kutu.DropDownStyle = ComboBoxStyle.DropDownList;
        kutu.FormattingEnabled = true;
        kutu.Items.AddRange(new object[]
        {
            DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday,
            DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday
        });
        kutu.Format += (_, e) =>
        {
            if (e.ListItem is DayOfWeek gun)
                e.Value = CultureInfo.GetCultureInfo("tr-TR").DateTimeFormat.GetDayName(gun);
        };
        kutu.SelectedIndex = 0;
    }

    // "HH:mm" gosteren saat secici.
    protected static DateTimePicker SaatKutusu(string ad, int sol, int ust, int saat)
        => new()
        {
            Name = ad, Left = sol, Top = ust, Width = 100,
            Format = DateTimePickerFormat.Custom, CustomFormat = "HH:mm", ShowUpDown = true,
            Value = DateTime.Today.AddHours(saat)
        };

    // Siralama icin: Pazartesi=1 ... Pazar=7 (C#'ta Pazar 0'dir, listede en sona gelsin diye).
    protected static int GunSirasi(DayOfWeek gun) => gun == DayOfWeek.Sunday ? 7 : (int)gun;
}
