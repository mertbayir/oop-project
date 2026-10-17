using System.Globalization;
using HastaneRandevu.Core.Models;

namespace HastaneRandevu.App;

// Doktor / saglik personelinin ekrani: sol tarafta kendisine gelen randevular, sag tarafta uygunluk saatleri.
// Burada da is kurali yok; her dugme sadece bir servis metodunu cagirir.
public class CalisanPaneli : PanelFormu
{
    private readonly Calisan _calisan;

    private readonly ListBox lstRandevular = new() { Name = "lstRandevular", Left = 15, Top = 80, Width = 575, Height = 320, HorizontalScrollbar = true };
    private readonly Button btnOnayla = new() { Name = "btnOnayla", Text = "Onayla", Left = 15, Top = 410, Width = 130 };
    private readonly Button btnReddet = new() { Name = "btnReddet", Text = "Reddet", Left = 155, Top = 410, Width = 130 };
    private readonly Button btnTamamla = new() { Name = "btnTamamla", Text = "Tamamla", Left = 295, Top = 410, Width = 130 };

    private readonly ListBox lstUygunluk = new() { Name = "lstUygunluk", Left = 610, Top = 80, Width = 375, Height = 150 };
    private readonly ComboBox cmbGun = new() { Name = "cmbGun", Left = 690, Top = 275, Width = 200, DropDownStyle = ComboBoxStyle.DropDownList, FormattingEnabled = true };
    private readonly DateTimePicker dtpBaslangic = SaatKutusu("dtpBaslangic", 307, 9);
    private readonly DateTimePicker dtpBitis = SaatKutusu("dtpBitis", 339, 12);
    private readonly Button btnEkle = new() { Name = "btnEkle", Text = "Uygunluk ekle", Left = 690, Top = 375, Width = 200 };

    public CalisanPaneli(Kullanici kullanici, UygulamaVerisi veri)
        : base(kullanici, veri, "Çalışan Paneli")
    {
        // Bu panel sadece calisan (doktor / saglik personeli) icin.
        _calisan = kullanici as Calisan
            ?? throw new ArgumentException("Çalışan paneli sadece çalışan içindir.", nameof(kullanici));

        ClientSize = new Size(1000, 520);
        lblMesaj.Top = 450;
        lblMesaj.Width = 970;
        lblMesaj.Height = 55;

        EtiketEkle("Gelen randevularım", 15, 52, true);
        EtiketEkle("Uygunluk saatlerim", 610, 52, true);
        EtiketEkle("Yeni uygunluk ekle", 610, 245, true);
        EtiketEkle("Gün", 610, 278); EtiketEkle("Başlangıç", 610, 310); EtiketEkle("Bitiş", 610, 342);

        Controls.AddRange(new Control[]
        {
            lstRandevular, btnOnayla, btnReddet, btnTamamla,
            lstUygunluk, cmbGun, dtpBaslangic, dtpBitis, btnEkle
        });

        // Haftanin gunleri Pazartesi'den baslayarak; ekranda Turkce adlariyla gorunur.
        cmbGun.Items.AddRange(new object[]
        {
            DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday,
            DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday
        });
        cmbGun.Format += (_, e) =>
        {
            if (e.ListItem is DayOfWeek gun)
                e.Value = CultureInfo.GetCultureInfo("tr-TR").DateTimeFormat.GetDayName(gun);
        };
        cmbGun.SelectedIndex = 0;

        btnOnayla.Click += (_, _) => RandevuIslemi((id, c) => Veri.RandevuServisi.Onayla(id, c), "Randevu onaylandı.");
        btnReddet.Click += (_, _) => RandevuIslemi((id, c) => Veri.RandevuServisi.Reddet(id, c), "Randevu reddedildi.");
        btnTamamla.Click += (_, _) => RandevuIslemi((id, c) => Veri.RandevuServisi.Tamamla(id, c), "Randevu tamamlandı olarak işaretlendi.");
        btnEkle.Click += UygunlukEkle_Click;

        RandevulariYenile();
        UygunluklariYenile();
    }

    // Onayla / Reddet / Tamamla ayni kalibi izler: secili randevuyu al, servise ver, listeyi yenile.
    // Hangi servis metodunun cagrilacagini "islem" parametresi belirler.
    private void RandevuIslemi(Action<int, Calisan> islem, string basariMesaji)
    {
        if (lstRandevular.SelectedItem is not Randevu randevu)
        {
            Mesaj("Önce listeden bir randevu seçin.", false);
            return;
        }

        try
        {
            islem(randevu.Id, _calisan);
            RandevulariYenile();
            Mesaj(basariMesaji, true);
        }
        catch (Exception ex) when (ex is UnauthorizedAccessException or InvalidOperationException)
        {
            Mesaj(HataMetni.Al(ex), false);
        }
    }

    private void UygunlukEkle_Click(object? sender, EventArgs e)
    {
        if (cmbGun.SelectedItem is not DayOfWeek gun)
        {
            Mesaj("Bir gün seçin.", false);
            return;
        }

        try
        {
            var baslangic = TimeOnly.FromDateTime(dtpBaslangic.Value);
            var bitis = TimeOnly.FromDateTime(dtpBitis.Value);
            int id = _calisan.Uygunluklar.Select(u => u.Id).DefaultIfEmpty(0).Max() + 1;

            Veri.YonetimServisi.UygunlukEkle(_calisan, _calisan, new UygunlukAraligi(id, gun, baslangic, bitis));

            UygunluklariYenile();
            Mesaj("Uygunluk saati eklendi.", true);
        }
        catch (Exception ex) when (ex is UnauthorizedAccessException or InvalidOperationException or ArgumentException)
        {
            Mesaj(HataMetni.Al(ex), false);
        }
    }

    private void RandevulariYenile()
    {
        lstRandevular.Items.Clear();
        lstRandevular.Items.AddRange(Veri.RandevuServisi.CalisaninRandevulari(_calisan)
            .OrderBy(r => r.Baslangic).Cast<object>().ToArray());
    }

    private void UygunluklariYenile()
    {
        lstUygunluk.Items.Clear();
        lstUygunluk.Items.AddRange(_calisan.Uygunluklar
            .OrderBy(u => u.Gun == DayOfWeek.Sunday ? 7 : (int)u.Gun)   // Pazar en sona
            .ThenBy(u => u.Baslangic).Cast<object>().ToArray());
    }

    private static DateTimePicker SaatKutusu(string ad, int ust, int saat)
        => new()
        {
            Name = ad, Left = 690, Top = ust, Width = 100,
            Format = DateTimePickerFormat.Custom, CustomFormat = "HH:mm", ShowUpDown = true,
            Value = DateTime.Today.AddHours(saat)
        };
}
