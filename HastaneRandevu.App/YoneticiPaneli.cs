using HastaneRandevu.Core.Models;

namespace HastaneRandevu.App;

// Kurum yoneticisinin ekrani: kendi kurumunun subelerini, calisma saatlerini, bolumlerini ve hizmetlerini tanimlar.
// Burada da is kurali yok; her dugme KurumYonetimServisi'nin bir metodunu cagirir (yetki kontrolu orada).
public class YoneticiPaneli : PanelFormu
{
    private readonly KurumYoneticisi _yonetici;

    private readonly TabControl sekmeler = new() { Name = "sekmeler", Left = 15, Top = 45, Width = 970, Height = 395 };
    private readonly TabPage sekmeSube = new("Şubeler ve çalışma saatleri");
    private readonly TabPage sekmeBolum = new("Bölümler ve hizmetler");
    private readonly TabPage sekmeCalisan = new("Çalışanlar");

    // ---- Sekme 1: Subeler ve calisma saatleri ----
    private readonly ListBox lstSubeler = new() { Name = "lstSubeler", Left = 10, Top = 30, Width = 450, Height = 160 };
    private readonly TextBox txtSubeAd = new() { Name = "txtSubeAd", Left = 80, Top = 205, Width = 380 };
    private readonly TextBox txtSubeAdres = new() { Name = "txtSubeAdres", Left = 80, Top = 235, Width = 380 };
    private readonly Button btnSubeEkle = new() { Name = "btnSubeEkle", Text = "Şube ekle", Left = 80, Top = 268, Width = 380 };

    private readonly ListBox lstSaatler = new() { Name = "lstSaatler", Left = 490, Top = 30, Width = 450, Height = 160 };
    private readonly ComboBox cmbSaatGun = new() { Name = "cmbSaatGun", Left = 570, Top = 205, Width = 200 };
    private readonly DateTimePicker dtpSaatBas = SaatKutusu("dtpSaatBas", 570, 237, 8);
    private readonly DateTimePicker dtpSaatBit = SaatKutusu("dtpSaatBit", 570, 269, 17);
    private readonly Button btnSaatEkle = new() { Name = "btnSaatEkle", Text = "Çalışma saati ekle", Left = 570, Top = 302, Width = 200 };

    // ---- Sekme 2: Bolumler ve hizmetler ----
    private readonly ComboBox cmbSube2 = new() { Name = "cmbSube2", Left = 80, Top = 10, Width = 380, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly ListBox lstBolumler = new() { Name = "lstBolumler", Left = 10, Top = 60, Width = 450, Height = 140 };
    private readonly TextBox txtBolumAd = new() { Name = "txtBolumAd", Left = 80, Top = 215, Width = 270 };
    private readonly Button btnBolumEkle = new() { Name = "btnBolumEkle", Text = "Bölüm ekle", Left = 360, Top = 213, Width = 100 };

    private readonly ListBox lstHizmetler = new() { Name = "lstHizmetler", Left = 490, Top = 60, Width = 450, Height = 140 };
    private readonly TextBox txtHizmetAd = new() { Name = "txtHizmetAd", Left = 570, Top = 215, Width = 370 };
    private readonly NumericUpDown numSure = new() { Name = "numSure", Left = 570, Top = 247, Width = 100, Minimum = 5, Maximum = 240, Increment = 5, Value = 30 };
    private readonly NumericUpDown numUcret = new() { Name = "numUcret", Left = 570, Top = 279, Width = 100, Minimum = 0, Maximum = 100000, DecimalPlaces = 2, Increment = 50, Value = 300 };
    private readonly Button btnHizmetEkle = new() { Name = "btnHizmetEkle", Text = "Hizmet ekle", Left = 570, Top = 312, Width = 200 };

    // ---- Sekme 3: Calisanlar ----
    private readonly ComboBox cmbSube3 = new() { Name = "cmbSube3", Left = 80, Top = 10, Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly ListBox lstCalisanlar = new() { Name = "lstCalisanlar", Left = 10, Top = 60, Width = 320, Height = 270, HorizontalScrollbar = true };
    private readonly ComboBox cmbTur = new() { Name = "cmbTur", Left = 455, Top = 37, Width = 175, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly TextBox txtCalAd = new() { Name = "txtCalAd", Left = 455, Top = 69, Width = 175 };
    private readonly TextBox txtCalSoyad = new() { Name = "txtCalSoyad", Left = 455, Top = 101, Width = 175 };
    private readonly TextBox txtCalEmail = new() { Name = "txtCalEmail", Left = 455, Top = 133, Width = 175 };
    private readonly TextBox txtCalSifre = new() { Name = "txtCalSifre", Left = 455, Top = 165, Width = 175, UseSystemPasswordChar = true };
    private readonly ComboBox cmbBolum3 = new() { Name = "cmbBolum3", Left = 755, Top = 37, Width = 195, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly Label lblOzellik = new() { Name = "lblOzellik", Left = 650, Top = 72, Width = 100, Text = "Uzmanlık alanı" };
    private readonly TextBox txtOzellik = new() { Name = "txtOzellik", Left = 755, Top = 69, Width = 195 };
    private readonly CheckedListBox clbHizmetler = new() { Name = "clbHizmetler", Left = 755, Top = 101, Width = 195, Height = 110, CheckOnClick = true };
    private readonly Button btnCalisanEkle = new() { Name = "btnCalisanEkle", Text = "Çalışan ekle", Left = 755, Top = 222, Width = 195 };

    public YoneticiPaneli(Kullanici kullanici, UygulamaVerisi veri)
        : base(kullanici, veri, "Yönetici Paneli")
    {
        _yonetici = kullanici as KurumYoneticisi
            ?? throw new ArgumentException("Yönetici paneli sadece kurum yöneticisi içindir.", nameof(kullanici));

        ClientSize = new Size(1000, 520);
        lblMesaj.Top = 450;
        lblMesaj.Width = 970;
        lblMesaj.Height = 55;

        Controls.Add(new Label
        {
            Text = $"Kurum: {_yonetici.Kurum.Ad}",
            Left = 400, Top = 12, Width = 350, Font = new Font(Font, FontStyle.Bold)
        });
        Controls.Add(sekmeler);
        sekmeler.TabPages.Add(sekmeSube);
        sekmeler.TabPages.Add(sekmeBolum);
        sekmeler.TabPages.Add(sekmeCalisan);

        SubeSekmesiniKur();
        BolumSekmesiniKur();
        CalisanSekmesiniKur();

        SubeleriYenile();
    }

    private void SubeSekmesiniKur()
    {
        EtiketEkle("Şubeler", 10, 8, true, sekmeSube);
        EtiketEkle("Seçili şubenin çalışma saatleri", 490, 8, true, sekmeSube);
        EtiketEkle("Şube adı", 10, 208, false, sekmeSube);
        EtiketEkle("Adres", 10, 238, false, sekmeSube);
        EtiketEkle("Gün", 490, 208, false, sekmeSube);
        EtiketEkle("Açılış", 490, 240, false, sekmeSube);
        EtiketEkle("Kapanış", 490, 272, false, sekmeSube);

        GunKutusunuHazirla(cmbSaatGun);
        sekmeSube.Controls.AddRange(new Control[]
        {
            lstSubeler, txtSubeAd, txtSubeAdres, btnSubeEkle,
            lstSaatler, cmbSaatGun, dtpSaatBas, dtpSaatBit, btnSaatEkle
        });

        lstSubeler.SelectedIndexChanged += (_, _) => SaatleriYenile();

        btnSubeEkle.Click += (_, _) => Dene(() =>
        {
            Veri.YonetimServisi.SubeOlustur(_yonetici, txtSubeAd.Text, txtSubeAdres.Text);
            txtSubeAd.Clear();
            txtSubeAdres.Clear();
            SubeleriYenile();
        }, "Şube eklendi. Şimdi çalışma saatlerini ekleyin.");

        btnSaatEkle.Click += (_, _) =>
        {
            if (lstSubeler.SelectedItem is not Sube sube || cmbSaatGun.SelectedItem is not DayOfWeek gun)
            {
                Mesaj("Önce listeden bir şube seçin.", false);
                return;
            }

            Dene(() =>
            {
                var acilis = TimeOnly.FromDateTime(dtpSaatBas.Value);
                var kapanis = TimeOnly.FromDateTime(dtpSaatBit.Value);
                int id = sube.CalismaSaatleri.Select(s => s.Id).DefaultIfEmpty(0).Max() + 1;

                Veri.YonetimServisi.CalismaSaatiEkle(_yonetici, sube.Id, new UygunlukAraligi(id, gun, acilis, kapanis));
                SaatleriYenile();
            }, "Çalışma saati eklendi.");
        };
    }

    private void BolumSekmesiniKur()
    {
        EtiketEkle("Şube", 10, 13, false, sekmeBolum);
        EtiketEkle("Bölümler", 10, 38, true, sekmeBolum);
        EtiketEkle("Seçili bölümün hizmetleri", 490, 38, true, sekmeBolum);
        EtiketEkle("Bölüm adı", 10, 218, false, sekmeBolum);
        EtiketEkle("Hizmet adı", 490, 218, false, sekmeBolum);
        EtiketEkle("Süre (dk)", 490, 250, false, sekmeBolum);
        EtiketEkle("Ücret (TL)", 490, 282, false, sekmeBolum);

        sekmeBolum.Controls.AddRange(new Control[]
        {
            cmbSube2, lstBolumler, txtBolumAd, btnBolumEkle,
            lstHizmetler, txtHizmetAd, numSure, numUcret, btnHizmetEkle
        });

        cmbSube2.SelectedIndexChanged += (_, _) => BolumleriYenile();
        lstBolumler.SelectedIndexChanged += (_, _) => HizmetleriYenile();

        btnBolumEkle.Click += (_, _) =>
        {
            if (cmbSube2.SelectedItem is not Sube sube)
            {
                Mesaj("Önce bir şube seçin.", false);
                return;
            }

            Dene(() =>
            {
                Veri.YonetimServisi.BolumOlustur(_yonetici, sube.Id, txtBolumAd.Text);
                txtBolumAd.Clear();
                SubeleriYenile();   // sube listesindeki "(n bolum)" yazisi da guncellensin
            }, "Bölüm eklendi.");
        };

        btnHizmetEkle.Click += (_, _) =>
        {
            if (lstBolumler.SelectedItem is not Bolum bolum)
            {
                Mesaj("Önce listeden bir bölüm seçin.", false);
                return;
            }

            Dene(() =>
            {
                var sure = TimeSpan.FromMinutes((double)numSure.Value);
                Veri.YonetimServisi.HizmetOlustur(_yonetici, bolum.Id, txtHizmetAd.Text, sure, numUcret.Value);
                txtHizmetAd.Clear();
                BolumleriYenile();   // bolum listesindeki "(n hizmet)" yazisi da guncellensin
            }, "Hizmet eklendi.");
        };
    }

    // Kurumun subelerini iki yerde birden gosterir (sekme 1 listesi, sekme 2 kutusu); secimleri korur.
    private void SubeleriYenile()
    {
        int sec1 = lstSubeler.SelectedIndex;
        int sec2 = cmbSube2.SelectedIndex;
        int sec3 = cmbSube3.SelectedIndex;
        object[] subeler = _yonetici.Kurum.Subeler.Cast<object>().ToArray();

        lstSubeler.Items.Clear();
        lstSubeler.Items.AddRange(subeler);
        cmbSube2.Items.Clear();
        cmbSube2.Items.AddRange(subeler);
        cmbSube3.Items.Clear();
        cmbSube3.Items.AddRange(subeler);

        if (subeler.Length > 0)
        {
            lstSubeler.SelectedIndex = sec1 >= 0 && sec1 < subeler.Length ? sec1 : 0;
            cmbSube2.SelectedIndex = sec2 >= 0 && sec2 < subeler.Length ? sec2 : 0;
            cmbSube3.SelectedIndex = sec3 >= 0 && sec3 < subeler.Length ? sec3 : 0;
        }
    }

    private void CalisanSekmesiniKur()
    {
        EtiketEkle("Şube", 10, 13, false, sekmeCalisan);
        EtiketEkle("Şubenin çalışanları", 10, 38, true, sekmeCalisan);
        EtiketEkle("Yeni çalışan", 350, 13, true, sekmeCalisan);
        EtiketEkle("Tür", 350, 40, false, sekmeCalisan);
        EtiketEkle("Ad", 350, 72, false, sekmeCalisan);
        EtiketEkle("Soyad", 350, 104, false, sekmeCalisan);
        EtiketEkle("E-posta", 350, 136, false, sekmeCalisan);
        EtiketEkle("Şifre", 350, 168, false, sekmeCalisan);
        EtiketEkle("Bölüm", 650, 40, false, sekmeCalisan);
        EtiketEkle("Hizmetler", 650, 104, false, sekmeCalisan);

        cmbTur.Items.AddRange(new object[] { "Doktor", "Sağlık Personeli" });
        cmbTur.SelectedIndex = 0;

        sekmeCalisan.Controls.AddRange(new Control[]
        {
            cmbSube3, lstCalisanlar, cmbTur, txtCalAd, txtCalSoyad, txtCalEmail, txtCalSifre,
            cmbBolum3, lblOzellik, txtOzellik, clbHizmetler, btnCalisanEkle
        });

        // Tur degisince ek alanin adi degisir: doktorun "uzmanlik alani", personelin "unvani" vardir.
        cmbTur.SelectedIndexChanged += (_, _) =>
            lblOzellik.Text = cmbTur.SelectedIndex == 0 ? "Uzmanlık alanı" : "Unvan";
        cmbSube3.SelectedIndexChanged += (_, _) => SubeDegistiCalisan();
        cmbBolum3.SelectedIndexChanged += (_, _) => HizmetKutusunuDoldur();
        btnCalisanEkle.Click += CalisanEkle_Click;
    }

    private void CalisanEkle_Click(object? sender, EventArgs e)
    {
        if (cmbSube3.SelectedItem is not Sube sube || cmbBolum3.SelectedItem is not Bolum bolum)
        {
            Mesaj("Önce şube ve bölüm seçin.", false);
            return;
        }

        // Isaretlenen hizmetlerin numaralari; servis bunlari calisanin bolumunden bulur.
        List<int> hizmetNumaralari = clbHizmetler.CheckedItems.Cast<Hizmet>().Select(h => h.Id).ToList();
        bool doktorMu = cmbTur.SelectedIndex == 0;

        Dene(() =>
        {
            if (doktorMu)
                Veri.YonetimServisi.DoktorEkle(_yonetici, sube.Id, bolum.Id, txtCalAd.Text, txtCalSoyad.Text,
                    txtCalEmail.Text, txtCalSifre.Text, txtOzellik.Text, hizmetNumaralari);
            else
                Veri.YonetimServisi.PersonelEkle(_yonetici, sube.Id, bolum.Id, txtCalAd.Text, txtCalSoyad.Text,
                    txtCalEmail.Text, txtCalSifre.Text, txtOzellik.Text, hizmetNumaralari);

            foreach (var kutu in new[] { txtCalAd, txtCalSoyad, txtCalEmail, txtCalSifre, txtOzellik })
                kutu.Clear();
            SubeleriYenile();   // sube listesindeki "(n calisan)" yazisi ve calisan listesi guncellensin
        }, doktorMu ? "Doktor eklendi; e-posta ve şifresiyle giriş yapabilir."
                    : "Sağlık personeli eklendi; e-posta ve şifresiyle giriş yapabilir.");
    }

    // Sube secilince hem o subenin calisanlari hem bolumleri yenilenir.
    private void SubeDegistiCalisan()
    {
        lstCalisanlar.Items.Clear();
        int sec = cmbBolum3.SelectedIndex;
        cmbBolum3.Items.Clear();

        if (cmbSube3.SelectedItem is Sube sube)
        {
            lstCalisanlar.Items.AddRange(sube.Calisanlar.Cast<object>().ToArray());
            cmbBolum3.Items.AddRange(sube.Bolumler.Cast<object>().ToArray());
            if (cmbBolum3.Items.Count > 0)
                cmbBolum3.SelectedIndex = sec >= 0 && sec < cmbBolum3.Items.Count ? sec : 0;
        }
    }

    private void HizmetKutusunuDoldur()
    {
        clbHizmetler.Items.Clear();
        if (cmbBolum3.SelectedItem is Bolum bolum)
            foreach (Hizmet hizmet in bolum.Hizmetler)
                clbHizmetler.Items.Add(hizmet);
    }

    private void SaatleriYenile()
    {
        lstSaatler.Items.Clear();
        if (lstSubeler.SelectedItem is Sube sube)
            lstSaatler.Items.AddRange(sube.CalismaSaatleri
                .OrderBy(s => GunSirasi(s.Gun)).ThenBy(s => s.Baslangic).Cast<object>().ToArray());
    }

    private void BolumleriYenile()
    {
        int sec = lstBolumler.SelectedIndex;
        lstBolumler.Items.Clear();
        if (cmbSube2.SelectedItem is Sube sube)
        {
            lstBolumler.Items.AddRange(sube.Bolumler.Cast<object>().ToArray());
            if (lstBolumler.Items.Count > 0)
                lstBolumler.SelectedIndex = sec >= 0 && sec < lstBolumler.Items.Count ? sec : 0;
        }
    }

    private void HizmetleriYenile()
    {
        lstHizmetler.Items.Clear();
        if (lstBolumler.SelectedItem is Bolum bolum)
            lstHizmetler.Items.AddRange(bolum.Hizmetler.Cast<object>().ToArray());
    }
}
