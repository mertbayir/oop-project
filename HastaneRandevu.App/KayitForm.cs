namespace HastaneRandevu.App;

public class KayitForm : Form
{
    private readonly UygulamaVerisi _veri;

    private readonly TextBox txtAd = new() { Name = "txtAd", Left = 120, Top = 20, Width = 200 };
    private readonly TextBox txtSoyad = new() { Name = "txtSoyad", Left = 120, Top = 55, Width = 200 };
    private readonly TextBox txtEmail = new() { Name = "txtEmail", Left = 120, Top = 90, Width = 200 };
    private readonly TextBox txtSifre = new() { Name = "txtSifre", Left = 120, Top = 125, Width = 200, UseSystemPasswordChar = true };
    private readonly TextBox txtTelefon = new() { Name = "txtTelefon", Left = 120, Top = 160, Width = 200 };
    private readonly Button btnKayit = new() { Name = "btnKayit", Text = "Kayıt ol", Left = 120, Top = 200, Width = 200 };
    private readonly Label lblMesaj = new() { Name = "lblMesaj", Left = 20, Top = 235, Width = 300, Height = 40, ForeColor = Color.Firebrick };

    public string KayitEmail { get; private set; } = "";

    public KayitForm(UygulamaVerisi veri)
    {
        _veri = veri;

        Text = "Hasta Kaydı";
        ClientSize = new Size(350, 285);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;

        string[] etiketler = { "Ad", "Soyad", "E-posta", "Şifre", "Telefon" };
        for (int i = 0; i < etiketler.Length; i++)
            Controls.Add(new Label { Text = etiketler[i], Left = 20, Top = 23 + i * 35, Width = 95 });

        Controls.AddRange(new Control[] { txtAd, txtSoyad, txtEmail, txtSifre, txtTelefon, btnKayit, lblMesaj });
        btnKayit.Click += Kayit_Click;
        AcceptButton = btnKayit;
    }

    private void Kayit_Click(object? sender, EventArgs e)
    {
        try
        {
            var hasta = _veri.KullaniciServisi.HastaKayitOl(
                txtAd.Text, txtSoyad.Text, txtEmail.Text, txtSifre.Text, txtTelefon.Text);

            KayitEmail = hasta.Email;
            DialogResult = DialogResult.OK;
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            lblMesaj.Text = HataMetni.Al(ex);
        }
    }
}
