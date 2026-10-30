using HastaneRandevu.Core.Models;

namespace HastaneRandevu.App;

// Sistem yoneticisinin ekrani: kurum tanimlar ve kuruma yonetici atar.
// Burada is kurali yok; her dugme KurumYonetimServisi'nin bir metodunu cagirir (yetki kontrolu orada).
public class SistemPaneli : PanelFormu
{
    private readonly ListBox lstKurumlar = new() { Name = "lstKurumlar", Left = 15, Top = 75, Width = 400, Height = 230 };
    private readonly TextBox txtKurumAd = new() { Name = "txtKurumAd", Left = 100, Top = 322, Width = 200 };
    private readonly Button btnKurumTanimla = new() { Name = "btnKurumTanimla", Text = "Kurum tanımla", Left = 310, Top = 320, Width = 105 };

    private readonly TextBox txtAd = new() { Name = "txtAd", Left = 540, Top = 80, Width = 200 };
    private readonly TextBox txtSoyad = new() { Name = "txtSoyad", Left = 540, Top = 112, Width = 200 };
    private readonly TextBox txtEmail = new() { Name = "txtEmail", Left = 540, Top = 144, Width = 200 };
    private readonly TextBox txtSifre = new() { Name = "txtSifre", Left = 540, Top = 176, Width = 200, UseSystemPasswordChar = true };
    private readonly Button btnYoneticiAta = new() { Name = "btnYoneticiAta", Text = "Yönetici ata", Left = 540, Top = 215, Width = 200 };

    public SistemPaneli(Kullanici kullanici, UygulamaVerisi veri)
        : base(kullanici, veri, "Sistem Paneli")
    {
        ClientSize = new Size(880, 420);
        lblMesaj.Top = 360;
        lblMesaj.Width = 850;
        lblMesaj.Height = 50;

        EtiketEkle("Kurumlar", 15, 50, true);
        EtiketEkle("Kurum adı", 15, 325);
        EtiketEkle("Seçili kuruma yönetici ata", 440, 50, true);
        EtiketEkle("Ad", 440, 83); EtiketEkle("Soyad", 440, 115);
        EtiketEkle("E-posta", 440, 147); EtiketEkle("Şifre", 440, 179);

        Controls.AddRange(new Control[]
        {
            lstKurumlar, txtKurumAd, btnKurumTanimla,
            txtAd, txtSoyad, txtEmail, txtSifre, btnYoneticiAta
        });

        btnKurumTanimla.Click += (_, _) => Dene(() =>
        {
            // Kurum numarasini servis verir; ekran sadece adi iletir.
            Kurum kurum = Veri.YonetimServisi.KurumTanimla(Kullanici, txtKurumAd.Text);
            txtKurumAd.Clear();
            KurumlariYenile(kurum);
        }, "Kurum tanımlandı. Şimdi bir yönetici atayın.");

        btnYoneticiAta.Click += (_, _) =>
        {
            if (lstKurumlar.SelectedItem is not Kurum kurum)
            {
                Mesaj("Önce listeden bir kurum seçin.", false);
                return;
            }

            Dene(() =>
            {
                var yonetici = Veri.YonetimServisi.KurumYoneticisiAta(
                    Kullanici, kurum.Id, txtAd.Text, txtSoyad.Text, txtEmail.Text, txtSifre.Text);

                foreach (var kutu in new[] { txtAd, txtSoyad, txtEmail, txtSifre })
                    kutu.Clear();
                return $"{yonetici.TamAd}, {kurum.Ad} kurumuna yönetici olarak atandı. Giriş yapabilir.";
            });
        };

        KurumlariYenile();
    }

    private void KurumlariYenile(Kurum? secilecek = null)
    {
        lstKurumlar.Items.Clear();
        lstKurumlar.Items.AddRange(Veri.YonetimServisi.Kurumlar.Cast<object>().ToArray());

        if (secilecek is not null)
            lstKurumlar.SelectedItem = secilecek;
        else if (lstKurumlar.Items.Count > 0)
            lstKurumlar.SelectedIndex = 0;
    }
}
