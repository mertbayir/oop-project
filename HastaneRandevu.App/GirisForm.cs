using HastaneRandevu.Core.Models;

namespace HastaneRandevu.App;

public class GirisForm : Form
{
    private readonly UygulamaVerisi _veri;

    private readonly TextBox txtEmail = new() { Name = "txtEmail", Left = 30, Top = 50, Width = 280 };
    private readonly TextBox txtSifre = new() { Name = "txtSifre", Left = 30, Top = 105, Width = 280, UseSystemPasswordChar = true };
    private readonly Button btnGiris = new() { Name = "btnGiris", Text = "Giriş yap", Left = 30, Top = 145, Width = 135 };
    private readonly Button btnKayit = new() { Name = "btnKayit", Text = "Hasta kaydı", Left = 175, Top = 145, Width = 135 };
    private readonly Label lblMesaj = new() { Name = "lblMesaj", Left = 30, Top = 185, Width = 280, Height = 40, ForeColor = Color.Firebrick };

    // Giris basarili olunca dolar; Program hangi paneli acacagina buna bakarak karar verir.
    public Kullanici? GirisYapan { get; private set; }

    public GirisForm(UygulamaVerisi veri)
    {
        _veri = veri;

        Text = "Hastane Randevu Sistemi - Giriş";
        ClientSize = new Size(340, 240);
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;

        Controls.Add(new Label { Text = "E-posta", Left = 30, Top = 30, Width = 280 });
        Controls.Add(new Label { Text = "Şifre", Left = 30, Top = 85, Width = 280 });
        Controls.AddRange(new Control[] { txtEmail, txtSifre, btnGiris, btnKayit, lblMesaj });

        btnGiris.Click += GirisYap_Click;
        btnKayit.Click += Kayit_Click;
        AcceptButton = btnGiris;   // Enter tusu giris yapar
    }

    private void GirisYap_Click(object? sender, EventArgs e)
    {
        try
        {
            GirisYapan = _veri.KullaniciServisi.GirisYap(txtEmail.Text.Trim(), txtSifre.Text);
            DialogResult = DialogResult.OK;   // formu kapatir, Program devam eder
        }
        catch (Exception ex) when (ex is UnauthorizedAccessException or ArgumentException)
        {
            lblMesaj.ForeColor = Color.Firebrick;
            lblMesaj.Text = HataMetni.Al(ex);
        }
    }

    private void Kayit_Click(object? sender, EventArgs e)
    {
        using var kayit = new KayitForm(_veri);
        if (kayit.ShowDialog(this) == DialogResult.OK)
        {
            txtEmail.Text = kayit.KayitEmail;
            txtSifre.Clear();
            lblMesaj.ForeColor = Color.DarkGreen;
            lblMesaj.Text = "Kayıt başarılı. Şifrenizle giriş yapabilirsiniz.";
        }
    }
}
