﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Diagnostics.Tracing;
using System.Diagnostics.Eventing.Reader;

namespace DigitalSignarute
{
    public partial class GiaoDien : Form
    {
        private RSA rsa = new RSA(); // Khai báo đối tượng và khởi tạo RSA
        public GiaoDien()
        {
            InitializeComponent();
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void GiaoDien_Load(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void txtP_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtQ_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnRandom_Click_Click(object sender, EventArgs e) // Tạo khóa từ giá trị ngẫu nhiên
        {
            try
            {
                rsa = new RSA();
                txtP.Text = rsa.P_test.ToString();
                txtQ.Text = rsa.Q_test.ToString();
                txtN.Text = rsa.N_test.ToString();
                txtPhiN.Text = rsa.Phi_n_test.ToString();
                txtE.Text = rsa.E_test.ToString();
                txtD.Text = rsa.D_test.ToString();

                txtPrivate.Text = string.Format("({0}, {1})", rsa.N_test.ToString(), rsa.D_test.ToString());
                txtPublic.Text = string.Format("({0}, {1})", rsa.N_test.ToString(), rsa.E_test.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnTinhToan_Click(object sender, EventArgs e) // Tạo khóa từ giá trị nhập từ bàn phímm
        {
            try
            {
                // Lấy dữ liệu từ TextBox
                int p = int.Parse(txtP.Text);
                int q = int.Parse(txtQ.Text);

                // Tạo đối tượng RSA và kiểm tra số nguyên tố
                RSA rsa = new RSA(p, q);

                // Hiển thị kết quả lên giao diện
                txtN.Text = rsa.N_test.ToString();
                txtPhiN.Text = rsa.Phi_n_test.ToString();
                txtE.Text = rsa.E_test.ToString();
                txtD.Text = rsa.D_test.ToString();

                txtPrivate.Text = string.Format("({0}, {1})", rsa.N_test.ToString(), rsa.D_test.ToString());
                txtPublic.Text = string.Format("({0}, {1})", rsa.N_test.ToString(), rsa.E_test.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtE_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void txtNhap_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnKySo_Click(object sender, EventArgs e)
        {
            string inputText = txtNhap.Text; // Lấy dữ liệu từ TextBox đầu vào

            // Băm dữ liệu đầu vào
            string hashedData = rsa.HashData(inputText);

            // Tạo chữ ký số
            string digitalSignature = rsa.taoChuKy(hashedData);

            // Hiển thị giá trị băm
            txtBam.Text = hashedData;

            // Hiển thị chữ ký số
            txtChuKySo.Text = digitalSignature;
        }

        private void btnBamXacMinh_Click(object sender, EventArgs e)
        {
            string inputText = txtNhapXacMinh.Text;

            // Băm dữ liệu cần xác minh
            string hashedDataCheck = rsa.HashData(inputText);

            txtBamXacMinh.Text = hashedDataCheck;
        }

        private void btnXacMinh_Click(object sender, EventArgs e)
        {
            string inputText = txtNhapXacMinh.Text; // Dữ liệu cần xác minh
            string digitalSignature = txtChuKySoXacMinh.Text; // Chữ ký số

            try
            {
                // Xác thực chữ ký số
                bool isValid = rsa.xacThucChuKy(inputText, digitalSignature);
                string chukyso_gui = txtChuKySo.Text;
                string chukyso_nhan = txtChuKySoXacMinh.Text;

                // Hiển thị kết quả xác thực
                if (isValid)
                {
                    MessageBox.Show("Nội dung không bị thay đổi!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                else if (chukyso_gui != chukyso_nhan)
                {
                    MessageBox.Show("Chữ ký số không hợp lệ!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                else
                {
                    MessageBox.Show("Nội dung bị thay đổi!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi xác thực: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtChuKySoXacMinh_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(txtChuKySo.Text);
        }


        private void txtChuKySo_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtGiaiMaChuKySo_TextChanged(object sender, EventArgs e)
        {

        }

        private void label17_Click(object sender, EventArgs e)
        {

        }

        private void btnGiaiMa_Click(object sender, EventArgs e)
        {
            string inputText = txtChuKySoXacMinh.Text;

            try
            {
                if (string.IsNullOrEmpty(inputText))
                {
                    MessageBox.Show("Chữ ký số không được để trống!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Giải mã chữ ký số
                string decryptedValue = rsa.giaiMaChuKy(inputText);

                // Hiển thị giá trị đã giải mã
                txtGiaiMaChuKySo.Text = decryptedValue;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi giải mã: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtPrivate_TextChanged(object sender, EventArgs e)
        {

        }

        private void Button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();
            if (file.ShowDialog() == DialogResult.OK)
            {
                string fileName = file.FileName;
                string fileDuLieu = File.ReadAllText(fileName);
                txtNhap.Text = fileDuLieu;
            }
        }

        private void btnTaiFile_XacMinh_Click(object sender, EventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();
            if (file.ShowDialog() == DialogResult.OK)
            {
                string fileName = file.FileName;
                string fileDuLieu = File.ReadAllText(fileName);
                txtNhapXacMinh.Text = fileDuLieu;
            }
        }

        private void btnLamMoi_Click_1(object sender, EventArgs e)
        {
            txtP.Text = "";
            txtQ.Text = "";
            txtN.Text = "";
            txtPhiN.Text = "";
            txtE.Text = "";
            txtD.Text = "";
            txtPrivate.Text = "";
            txtPublic.Text = "";
            txtNhap.Text = "";
            txtNhapXacMinh.Text = "";
            txtBam.Text = "";
            txtBamXacMinh.Text = "";
            txtChuKySo.Text = "";
            txtChuKySoXacMinh.Text = "";
            txtGiaiMaChuKySo.Text = "";
        }
    }
}
