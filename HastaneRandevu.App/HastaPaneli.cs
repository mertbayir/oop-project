using HastaneRandevu.Core.Models;

namespace HastaneRandevu.App;

// Hastanin ekrani: sol tarafta yeni randevu alma, sag tarafta kendi randevulari.
// Burada HICBIR is kurali yok; her dugme sadece bir servis metodunu cagirir.
public class HastaPaneli : PanelFormu
{
    private readonly Hasta _hasta;

    private readonly ComboBox cmbSube = YeniKutu("cmbSube", 80);
    private readonly ComboBox cmbBolum = YeniKutu("cmbBolum", 112);
    private readonly ComboBox cmbHizmet = YeniKutu("cmbHizmet", 144);
    private readonly ComboBox cmbCalisan = YeniKutu("cmbCalisan", 176);
    private readonly DateTimePicker dtpTarih = new()
    {
        Name = "dtpTarih", Left = 100, Top = 208, Width = 280,
        Format = DateTimePickerFormat.Short, MinDate = DateTime.Today, Value = DateTime.Today.AddDays(1)
    };
    private readonly Button btnSaatler = new() { Name = "btnSaatler", Text = "Boş saatleri göster", Left = 100, Top = 240, Width = 280 };
    private readonly ListBox lstSaatler = new() { Name = "lstSaatler", Left = 100, Top = 272, Width = 280, Height = 110, FormattingEnabled = true, FormatString = "HH:mm" };
    private readonly Button btnAl = new() { Name = "btnAl", Text = "Randevu al", Left = 100, Top = 390, Width = 280 };

    private readonly ListBox lstRandevular = new() { Name = "lstRandevular", Left = 410, Top = 80, Width = 575, Height = 320, HorizontalScrollbar = true };
    private readonly Button btnIptal = new() { Name = "btnIptal", Text = "Seçili randevuyu iptal et", Left = 410, Top = 410, Width = 220 };

    public HastaPaneli(Kullanici kullanici, UygulamaVerisi veri)
        : base(kullanici, veri, "Hasta Paneli")
    {
        // Bu panel sadece hasta icin; servise verecegimiz kisi gercekten Hasta mi diye bakiyoruz.
        _hasta = kullanici as Hasta
            ?? throw new ArgumentException("Hasta paneli sadece hasta icindir.", nameof(kullanici));

        ClientSize = new Size(1000, 520);
        lblMesaj.Width = 365;

        EtiketEkle("Yeni randevu", 15, 52, true);
        EtiketEkle("Şube", 15, 83); EtiketEkle("Bölüm", 15, 115); EtiketEkle("Hizmet", 15, 147);
        EtiketEkle("Çalışan", 15, 179); EtiketEkle("Tarih", 15, 211); EtiketEkle("Boş saatler", 15, 275);
        EtiketEkle("Randevularım", 410, 52, true);

        Controls.AddRange(new Control[]
        {
            cmbSube, cmbBolum, cmbHizmet, cmbCalisan, dtpTarih, btnSaatler, lstSaatler, btnAl,
            lstRandevular, btnIptal
        });

        // Her kutu degisince bir alttaki kutu yeniden dolar (siralama onemli: once olaylar, sonra ilk dolum).
        cmbSube.SelectedIndexChanged += (_, _) =>
            Doldur(cmbBolum, cmbSube.SelectedItem is Sube s ? s.Bolumler : Array.Empty<Bolum>());
        cmbBolum.SelectedIndexChanged += (_, _) =>
            Doldur(cmbHizmet, cmbBolum.SelectedItem is Bolum b ? b.Hizmetler : Array.Empty<Hizmet>());
        cmbHizmet.SelectedIndexChanged += (_, _) => HizmetDegisti();
        cmbCalisan.SelectedIndexChanged += (_, _) => lstSaatler.Items.Clear();
        dtpTarih.ValueChanged += (_, _) => lstSaatler.Items.Clear();

        btnSaatler.Click += SaatleriGoster_Click;
        btnAl.Click += RandevuAl_Click;
        btnIptal.Click += Iptal_Click;

        Doldur(cmbSube, Veri.YonetimServisi.Kurumlar.SelectMany(k => k.Subeler));
        RandevulariYenile();
    }

    // Sectigin hizmeti sunan calisanlar: Sube.Calisanlar icinden, Calisan.HizmetSunuyorMu ile suzulur.
    private void HizmetDegisti()
    {
        if (cmbSube.SelectedItem is Sube sube && cmbHizmet.SelectedItem is Hizmet hizmet)
            Doldur(cmbCalisan, sube.Calisanlar.Where(c => c.HizmetSunuyorMu(hizmet)));
        else
            Doldur(cmbCalisan, Array.Empty<Calisan>());
    }

    private void SaatleriGoster_Click(object? sender, EventArgs e)
    {
        if (cmbCalisan.SelectedItem is not Calisan calisan || cmbHizmet.SelectedItem is not Hizmet hizmet)
        {
            Mesaj("Önce hizmet ve çalışan seçin.", false);
            return;
        }

        try
        {
            int adet = SaatleriDoldur(calisan, hizmet);
            Mesaj(adet == 0 ? "Bu tarihte boş saat yok." : $"{adet} boş saat bulundu. Birini seçin.", adet > 0);
        }
        catch (InvalidOperationException ex)
        {
            Mesaj(HataMetni.Al(ex), false);
        }
    }

    private void RandevuAl_Click(object? sender, EventArgs e)
    {
        if (cmbCalisan.SelectedItem is not Calisan calisan
            || cmbHizmet.SelectedItem is not Hizmet hizmet
            || lstSaatler.SelectedItem is not DateTime saat)
        {
            Mesaj("Hizmet, çalışan ve bir saat seçin.", false);
            return;
        }

        try
        {
            Randevu randevu = Veri.RandevuServisi.RandevuOlustur(_hasta, calisan, hizmet, saat);
            RandevulariYenile();
            SaatleriDoldur(calisan, hizmet);
            Mesaj($"Randevu oluşturuldu (#{randevu.Id}). Çalışanın onayı bekleniyor.", true);
        }
        catch (Exception ex) when (ex is UnauthorizedAccessException or InvalidOperationException or ArgumentException)
        {
            Mesaj(HataMetni.Al(ex), false);
        }
    }

    private void Iptal_Click(object? sender, EventArgs e)
    {
        if (lstRandevular.SelectedItem is not Randevu randevu)
        {
            Mesaj("Önce listeden bir randevu seçin.", false);
            return;
        }

        try
        {
            Veri.RandevuServisi.IptalEt(randevu.Id, _hasta);
            RandevulariYenile();
            if (cmbCalisan.SelectedItem is Calisan c && cmbHizmet.SelectedItem is Hizmet h)
                SaatleriDoldur(c, h);
            Mesaj("Randevu iptal edildi.", true);
        }
        catch (Exception ex) when (ex is UnauthorizedAccessException or InvalidOperationException)
        {
            Mesaj(HataMetni.Al(ex), false);
        }
    }

    private int SaatleriDoldur(Calisan calisan, Hizmet hizmet)
    {
        var saatler = Veri.RandevuServisi.MusaitSlotlariGetir(calisan, hizmet, dtpTarih.Value.Date);
        lstSaatler.Items.Clear();
        lstSaatler.Items.AddRange(saatler.Cast<object>().ToArray());
        return saatler.Count;
    }

    private void RandevulariYenile()
    {
        lstRandevular.Items.Clear();
        lstRandevular.Items.AddRange(Veri.RandevuServisi.HastaninRandevulari(_hasta)
            .OrderBy(r => r.Baslangic).Cast<object>().ToArray());
    }

    // Kutunun icerigini yeniden doldurur ve ilkini secer (secim, alttaki kutunun dolmasini tetikler).
    private static void Doldur(ComboBox kutu, IEnumerable<object> ogeler)
    {
        kutu.Items.Clear();
        kutu.Items.AddRange(ogeler.ToArray());
        if (kutu.Items.Count > 0)
            kutu.SelectedIndex = 0;
    }



    private static ComboBox YeniKutu(string ad, int ust)
        => new() { Name = ad, Left = 100, Top = ust, Width = 280, DropDownStyle = ComboBoxStyle.DropDownList };
}
