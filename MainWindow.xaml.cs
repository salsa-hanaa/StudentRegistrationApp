using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace StudentRegistrationApp
{
    public class Mahasiswa
    {
        public string Nim { get; set; }
        public string Nama { get; set; }
        public string Prodi { get; set; }
        public string JenisKelamin { get; set; }
        public DateTime? TanggalLahir { get; set; }
        public string NoTelepon { get; set; }
        public string Alamat { get; set; }

        public override string ToString()
        {
            return $"{Nim} | {Nama} | {Prodi} | {JenisKelamin} | {TanggalLahir?.ToString("dd/MM/yyyy")} | {NoTelepon}";
        }
    }

    public partial class MainWindow : Window
    {
        private List<Mahasiswa> listMahasiswa = new List<Mahasiswa>();
        private Mahasiswa selectedMahasiswaEdit = null;

        public MainWindow()
        {
            InitializeComponent();
            UpdateCounter();
        }

        private void BtnSimpan_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput()) return;

            if (listMahasiswa.Any(m => m.Nim.Equals(txtNim.Text.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("NIM sudah terdaftar!", "Peringatan", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Mahasiswa mhs = new Mahasiswa
            {
                Nim = txtNim.Text.Trim(),
                Nama = txtNama.Text.Trim(),
                Prodi = (cmbProdi.SelectedItem as ComboBoxItem)?.Content.ToString(),
                JenisKelamin = rbLaki.IsChecked == true ? "Laki-laki" : "Perempuan",
                TanggalLahir = dpTanggalLahir.SelectedDate,
                NoTelepon = txtNoTelepon.Text.Trim(),
                Alamat = txtAlamat.Text.Trim()
            };

            listMahasiswa.Add(mhs);
            RefreshListBox();
            MessageBox.Show("Data mahasiswa berhasil disimpan!", "Informasi", MessageBoxButton.OK, MessageBoxImage.Information);
            ResetForm();
        }

        private void LstMahasiswa_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstMahasiswa.SelectedItem is Mahasiswa mhs)
            {
                selectedMahasiswaEdit = mhs;

                txtNim.Text = mhs.Nim;
                txtNim.IsEnabled = false; 
                txtNama.Text = mhs.Nama;

                foreach (ComboBoxItem item in cmbProdi.Items)
                {
                    if (item.Content.ToString() == mhs.Prodi)
                    {
                        cmbProdi.SelectedItem = item;
                        break;
                    }
                }

                rbLaki.IsChecked = mhs.JenisKelamin == "Laki-laki";
                rbPerempuan.IsChecked = mhs.JenisKelamin == "Perempuan";
                dpTanggalLahir.SelectedDate = mhs.TanggalLahir;
                txtNoTelepon.Text = mhs.NoTelepon;
                txtAlamat.Text = mhs.Alamat;
            }
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (selectedMahasiswaEdit == null)
            {
                MessageBox.Show("Pilih data yang ingin di-edit dari daftar terlebih dahulu!", "Peringatan", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!ValidateInput()) return;

            selectedMahasiswaEdit.Nama = txtNama.Text.Trim();
            selectedMahasiswaEdit.Prodi = (cmbProdi.SelectedItem as ComboBoxItem)?.Content.ToString();
            selectedMahasiswaEdit.JenisKelamin = rbLaki.IsChecked == true ? "Laki-laki" : "Perempuan";
            selectedMahasiswaEdit.TanggalLahir = dpTanggalLahir.SelectedDate;
            selectedMahasiswaEdit.NoTelepon = txtNoTelepon.Text.Trim();
            selectedMahasiswaEdit.Alamat = txtAlamat.Text.Trim();

            RefreshListBox();
            MessageBox.Show("Data mahasiswa berhasil diperbarui!", "Informasi", MessageBoxButton.OK, MessageBoxImage.Information);
            ResetForm();
        }

        private void BtnHapus_Click(object sender, RoutedEventArgs e)
        {
            if (lstMahasiswa.SelectedItem is Mahasiswa mhs)
            {
                MessageBoxResult confirm = MessageBox.Show(
                    $"Apakah Anda yakin ingin menghapus data {mhs.Nama} ({mhs.Nim})?",
                    "Konfirmasi Hapus",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (confirm == MessageBoxResult.Yes)
                {
                    listMahasiswa.Remove(mhs);
                    RefreshListBox();
                    ResetForm();
                    MessageBox.Show("Data berhasil dihapus!", "Informasi", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Pilih data yang ingin dihapus!", "Peringatan", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string keyword = txtSearch.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(keyword))
            {
                RefreshListBox();
            }
            else
            {
                var hasilCari = listMahasiswa.Where(m =>
                    m.Nim.ToLower().Contains(keyword) ||
                    m.Nama.ToLower().Contains(keyword) ||
                    m.Prodi.ToLower().Contains(keyword)
                ).ToList();

                lstMahasiswa.ItemsSource = null;
                lstMahasiswa.ItemsSource = hasilCari;
            }
        }

        private void UpdateCounter()
        {
            if (lblCounter != null)
            {
                lblCounter.Text = $"Jumlah Mahasiswa: {listMahasiswa.Count}";
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtNim.Text)) { MessageBox.Show("NIM harus diisi!", "Peringatan", MessageBoxButton.OK, MessageBoxImage.Warning); txtNim.Focus(); return false; }
            if (string.IsNullOrWhiteSpace(txtNama.Text)) { MessageBox.Show("Nama harus diisi!", "Peringatan", MessageBoxButton.OK, MessageBoxImage.Warning); txtNama.Focus(); return false; }
            if (cmbProdi.SelectedItem == null) { MessageBox.Show("Pilih program studi!", "Peringatan", MessageBoxButton.OK, MessageBoxImage.Warning); return false; }
            if (rbLaki.IsChecked != true && rbPerempuan.IsChecked != true) { MessageBox.Show("Pilih jenis kelamin!", "Peringatan", MessageBoxButton.OK, MessageBoxImage.Warning); return false; }
            if (dpTanggalLahir.SelectedDate == null) { MessageBox.Show("Pilih tanggal lahir!", "Peringatan", MessageBoxButton.OK, MessageBoxImage.Warning); return false; }
            if (string.IsNullOrWhiteSpace(txtNoTelepon.Text)) { MessageBox.Show("Nomor Telepon harus diisi!", "Peringatan", MessageBoxButton.OK, MessageBoxImage.Warning); txtNoTelepon.Focus(); return false; }
            if (string.IsNullOrWhiteSpace(txtAlamat.Text)) { MessageBox.Show("Alamat harus diisi!", "Peringatan", MessageBoxButton.OK, MessageBoxImage.Warning); txtAlamat.Focus(); return false; }
            return true;
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            ResetForm();
        }

        private void ResetForm()
        {
            txtNim.Clear();
            txtNim.IsEnabled = true;
            txtNama.Clear();
            cmbProdi.SelectedIndex = -1;
            rbLaki.IsChecked = false;
            rbPerempuan.IsChecked = false;
            dpTanggalLahir.SelectedDate = null;
            txtNoTelepon.Clear();
            txtAlamat.Clear();

            selectedMahasiswaEdit = null;
            lstMahasiswa.SelectedItem = null;
            txtNim.Focus();
        }

        private void RefreshListBox()
        {
            lstMahasiswa.ItemsSource = null;
            lstMahasiswa.ItemsSource = listMahasiswa;
            UpdateCounter();
        }
    }
}